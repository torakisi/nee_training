using NEE.Core.Contracts;
using System.Collections.Generic;
using NEE.Core.BO;
using System.Threading.Tasks;
using NEE.Service.Core;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Service.Authorization;
using System;
using NEE.Core.Validation;
using static NEE.Service.PersonService;
using System.Globalization;
using XServices.Idika;
using XServices.Gsis;

namespace NEE.Service
{
	partial class AppService
	{
		public async Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest req)
		{
			ServiceContext context = new ServiceContext()
			{
				ServiceAction = ServiceAction.CreateApplication,
				InitialRequest = JsonHelper.Serialize(req, false)
			};

			this.SetServiceContext(context);

			CreateApplicationResponse response = new CreateApplicationResponse(_errorLogger, _currentUserContext.UserName);

			try
			{
				if (!ServiceAuthorization.AccessUnavailableForMaintainanceAuthorized(_currentUserContext))
					throw new UnauthorizedAccessException(ServiceAuthorization.MAINTAINANCE_SHUTDOWN);

				if (!ServiceAuthorization.CreateApplicationAuthorized(_currentUserContext, req))
					throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_CREATE_APPLICATION_MESSAGE);

				var applicationOwnerResp = await this.GetApplicationOwner(new GetApplicationOwnerRequest()
				{
					AFM = req.AFM,
					AMKA = req.AMKA
				});

				if (!applicationOwnerResp._IsSuccessful)
				{
					throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_CREATE_APPLICATION_MESSAGE);
				}

				var applicationOwner = applicationOwnerResp.ApplicationOwner;
				CanCreateNewResult canCreateNew = applicationOwner.CanCreateNew(clock.Now);
				if (!canCreateNew.CanCreateNew)
				{
					throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_CREATE_APPLICATION_MESSAGE);
				}

                var application = applicationOwner.NewApplication(_appIdGenerator);


                var createApplicationForPersonResp = await CreateApplicationForPersonAsync(application, req.AMKA, req.AFM);

                if (!createApplicationForPersonResp._IsSuccessful)
                {
                    response.AddErrors(createApplicationForPersonResp._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }

                application.SetState(AppState.Draft, _currentUserContext);
                application.DraftAt = DateTime.Now;
                application.DraftBy = _currentUserContext?.UserName;

                application.RemoteHostIP = req.RemoteHostIP;  // pass remote host ID for the IBAN Validation WS 

                await CreateValidationRemarksForApplicationAsync(application);
                application.PopulateCreatedByFlag(_currentUserContext);

               await repository.Save(application, true);
                response.CreatedApplication = application;
            }
            catch (UnauthorizedAccessException ex)
            {
                response.AddError(ErrorCategory.UIDisplayed, ex.Message, ex);
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
		}

        private Application CreateApplicant(Application application, string amka, string afm)
        {
            var applicant = new Person
            {
                AMKA = new Amka(amka),
                AFM = new Afm(afm),
            };

            applicant.GetNewEntityId(_appIdGenerator);
            applicant.Relationship = MemberRelationship.Applicant;
            applicant.PersonId = 0;
            application.Applicant = applicant;

            // Προσθέτουμε τα βασικά στοιχεία της νέας αίτησης, που σχετίζονται με τον applicant.
            application.AFM = applicant.AFM;
            application.AMKA = applicant.AMKA;
            return application;
        }

        private Application CreateSpouse(Application application, string amka, string afm)
        {
            var spouse = new Person
            {
                AMKA = new Amka(amka),
                AFM = new Afm(afm),
            };

            spouse.GetNewEntityId(_appIdGenerator);
            spouse.Relationship = MemberRelationship.Spouse;
            spouse.PersonId = 1;
            application.Spouse = spouse;

            return application;
        }        

        private async Task<CreateApplicationForPersonResponse> CreateApplicationForPersonAsync(Application application, string amka, string afm)
        {
            CreateApplicationForPersonResponse response = new CreateApplicationForPersonResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                // Applicant
                // create Applicant
                application = CreateApplicant(application, amka, afm);

                // add applicant data from web services
                var respProvideApplicantData = await ProvideApplicantData(application, application.Applicant);
                if (!respProvideApplicantData._IsSuccessful)
                {
                    response.AddErrors(respProvideApplicantData._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);
                    return response;
                }

                // Spouse
                if (application.Applicant.HasSpouse && application.SpouseAMKA != null && application.SpouseAFM != null)
                {
                    //create spouse
                    application = CreateSpouse(application, application.SpouseAMKA, application.SpouseAFM);

                    // add spouse data from web services
                    var respProvideSpouseData = await ProvideSpouseData(application, application.Spouse);
                    if (!respProvideSpouseData._IsSuccessful)
                    {
                        response.AddErrors(respProvideSpouseData._Errors);
                        response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);
                        return response;
                    }
                }

                response.CreatedApplication = application;
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }

        private async Task<ProvideMembersDataResponse> ProvideApplicantData(Application application, Person person)
        {
            ProvideMembersDataResponse response = new ProvideMembersDataResponse(_errorLogger, _currentUserContext.UserName);
            try
            {
                ProvideMemberDataResponse respApplicantData = await TryProvideApplicantData(application, person);

                if (!respApplicantData._IsSuccessful)
                {
                    response.AddErrors(respApplicantData._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }
            return response;
        }

        private async Task<ProvideMembersDataResponse> ProvideSpouseData(Application application, Person person)
        {
            ProvideMembersDataResponse response = new ProvideMembersDataResponse(_errorLogger, _currentUserContext.UserName);
            try
            {
                ProvideMemberDataResponse respSpouseData = await TryProvideSpouseData(application, person);

                if (!respSpouseData._IsSuccessful)
                {
                    response.AddErrors(respSpouseData._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }
            return response;
        }

        private async Task<ProvideMemberDataResponse> TryProvideApplicantData(Application application, Person member)
        {
            ProvideMemberDataResponse response = new ProvideMemberDataResponse(_errorLogger, _currentUserContext.UserName);
            try
            {
                #region 1. Get data from AMKA
                var provideMemberDataResponse = await ProvideMemberData(member);

                if (!provideMemberDataResponse._IsSuccessful)
                {
                    response.AddErrors(provideMemberDataResponse._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }
                #endregion

                #region 2. get edto data with latin characters
                GetEDTOResponse xsRes_EDTO = null;
                xsRes_EDTO = await _personService.GetEDTOInfoAsync(new GetEDTORequest()
                {
                    FirstName = member.FirstNameEN,
                    LastName = member.LastNameEN,
                    FathersName = member.FatherNameEN,
                    MothersName = member.MotherNameEN,
                    BirthDate = member.DOB != null ? ((DateTime)member.DOB).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null,
                    ApplicationId = application.Id,
                    Afm = member.AFM,
                    Amka = member.AMKA
                });

                if (!xsRes_EDTO._IsSuccessful)
                {
                    response.AddErrors(xsRes_EDTO._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }

                member.PermitNumber = xsRes_EDTO.PermitNumber;
                member.AdministrationDate = xsRes_EDTO.AdministrationDate;
                
                // retry with greek characters
                if (xsRes_EDTO.PermitNumber == null)
                {
                    GetEDTOResponse xsRes_EDTOGr = null;
                    xsRes_EDTOGr = await _personService.GetEDTOInfoAsync(new GetEDTORequest()
                    {
                        FirstName = member.FirstName,
                        LastName = member.LastName,
                        FathersName = member.FatherName,
                        MothersName = member.MotherName,
                        BirthDate = member.DOB != null ? ((DateTime)member.DOB).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null,
                        ApplicationId = application.Id,
                        Afm = member.AFM,
                        Amka = member.AMKA
                    });

                    if (!xsRes_EDTOGr._IsSuccessful)
                    {
                        response.AddErrors(xsRes_EDTOGr._Errors);
                        response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                        return response;
                    }

                    member.PermitNumber = xsRes_EDTOGr.PermitNumber;
                    member.AdministrationDate = xsRes_EDTOGr.AdministrationDate;
                }
                #endregion

                #region 3. get aade property data

                GetPropertyValueInfoResponse xsRes_Property = null;
                xsRes_Property = await _personService.GetPropertyValueInfoAsync(new GetPropertyValueInfoRequest()
                {
                    ApplicationId = application.Id,
                    Afm = member.AFM,
                    Amka = member.AMKA,
                    ReferenceYear = DateTime.Now.Year - 2
                });

                if (!xsRes_Property._IsSuccessful)
                {
                    response.AddErrors(xsRes_Property._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }

                member.AssetsValue = xsRes_Property.AssetsValue != -1 ? xsRes_Property.AssetsValue : null;
                #endregion

                #region 4. get aade info data

                GetIncomeMobileValueInfoResponse xsRes_GsisInfo = null;
                xsRes_GsisInfo = await _personService.GetIncomeMobileValueAsync(new GetIncomeMobileValueInfoRequest()
                {
                    ApplicationId = application.Id,
                    Afm = member.AFM,
                    Amka = member.AMKA,
                    ReferenceYear = DateTime.Now.Year - 2
                });

                if (!xsRes_GsisInfo._IsSuccessful)
                {
                    response.AddErrors(xsRes_GsisInfo._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }

                application.SpouseAFM = xsRes_GsisInfo.SpouseAfm;
                application.SpouseAMKA = xsRes_GsisInfo.SpouseAmka;
                member.FamilyIncome = xsRes_GsisInfo.FamilyIncome;
                member.Income = xsRes_GsisInfo.Income;
                member.VehiclesValue = xsRes_GsisInfo.VehiclesValue;
                #endregion

                #region 5. get citizen registry info

                GetMaritalStatusInfoResponse xsRes_citizenRegistryInfo = null;
                xsRes_citizenRegistryInfo = await _personService.GetMaritalStatusAsync(new GetMaritalStatusInfoRequest()
                {
                    ApplicationId = application.Id,
                    Afm = member.AFM,
                    Amka = member.AMKA,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    FatherName = member.FatherName,
                    MotherName = member.MotherName,
                    BirthDate = member.DOB != null ? ((DateTime)member.DOB).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null
                });

                if (!xsRes_citizenRegistryInfo._IsSuccessful)
                {
                    response.AddErrors(xsRes_citizenRegistryInfo._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }

                member.MaritalStatus = xsRes_citizenRegistryInfo.MaritalStatus;
                member.UnknownMaritalStatus = xsRes_citizenRegistryInfo.MaritalStatus == MaritalStatus.Unknown;

                #endregion

                #region 6. get other benefits
                GetOtherBenefitsInfoResponse xsRes_OtherBenefits = null;
                DateTime lastMonth = DateTime.Parse(DateTime.Today.AddDays(-(DateTime.Today.Day - 1)).AddMonths(-1).ToString("dd/MM/yyyy"));
                xsRes_OtherBenefits = await _personService.GetOtherBenefitsAsync(new GetOtherBenefitsInfoRequest()
                {
                    AMKA = member.AMKA,
                    YearMonth = lastMonth
                });
                if (!xsRes_OtherBenefits._IsSuccessful)
                {

                    response.AddErrors(xsRes_OtherBenefits._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }
                decimal disabilityBenefitsSums = 0;
                foreach (var benefit in xsRes_OtherBenefits.OtherBenefits)
                {
                    switch (benefit.BenefitCategoryCode)
                    {
                        case "100":
                            disabilityBenefitsSums += benefit.Amount;
                            break;
                        case "200":
                            member.A21Benefit = benefit.Amount;
                            break;
                        case "300":
                            member.HousingBenefit = benefit.Amount;
                            break;
                        case "400":
                            member.KAYBenefit = benefit.Amount;
                            break;
                        case "109":
                            member.HousingAssistanceBenefit = benefit.Amount;
                            break;
                        case "103":
                            member.BenefitForOmogeneis = benefit.Amount;
                            break;
                    }
                }
                member.DisabilityBenefits = disabilityBenefitsSums > 0 ? (decimal?)disabilityBenefitsSums : null;

                #endregion

                #region 7. get notification center info
                GetNotificationCenterInfoResponse xsRes_NotificationCenter = null;
                xsRes_NotificationCenter = await _personService.GetNotificationCenterInfoAsync(new GetNotificationCenterInfoRequest()
                {
                    ApplicationId = application.Id,
                    Afm = member.AFM,
                    Amka = member.AMKA
                });

                if (!xsRes_NotificationCenter._IsSuccessful)
                {
                    response.AddErrors(xsRes_NotificationCenter._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }

                member.Email = xsRes_NotificationCenter.Email ?? member.Email;
                member.MobilePhone = xsRes_NotificationCenter.MobilePhone ?? member.MobilePhone;
                member.HomePhone = xsRes_NotificationCenter.HomePhone ?? member.HomePhone;
                member.Street = xsRes_NotificationCenter.AddressStreet ?? member.Street;
                member.StreetNumber = xsRes_NotificationCenter.AddressNumber ?? member.StreetNumber;
                member.City = xsRes_NotificationCenter.AddressCity ?? member.City;
                member.Zip = xsRes_NotificationCenter.AddressZip ?? member.Zip;
                member.PostalNumber = xsRes_NotificationCenter.AddressPostalNumber ?? member.PostalNumber;
                member.Region = xsRes_NotificationCenter.Region ?? member.Region;
                member.RegionalUnit = xsRes_NotificationCenter.RegionalUnit ?? member.RegionalUnit;
                member.Municipality = xsRes_NotificationCenter.Municipality ?? member.Municipality;
                member.MunicipalUnit = xsRes_NotificationCenter.MunicipalUnit ?? member.MunicipalUnit;
                member.Commune = xsRes_NotificationCenter.Commune ?? member.Commune;
                #endregion

                #region 8. get ergani info
                GetErganiInfoResponse xsRes_Ergani = null;
                xsRes_Ergani = await _personService.GetErganiInfoAsync(new GetErganiInfoRequest()
                {
                    ApplicationId = application.Id,
                    Afm = member.AFM,
                    Amka = member.AMKA,
                    RefDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                });

                if (!xsRes_Ergani._IsSuccessful)
                {
                    response.AddErrors(xsRes_Ergani._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }

                member.IsEmployed = xsRes_Ergani.IsEmployed;
                #endregion

                #region 9. get efka info
                GetEfkaInfoResponse xsRes_Efka = null;
                xsRes_Efka = await _personService.GetEfkaInfoAsync(new GetEfkaInfoRequest()
                {
                    ApplicationId = application.Id,
                    AFM = member.AFM,
                    AMKA = member.AMKA,
                    DateFrom = new DateTime(2016,05,01).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DateTo = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                });

                if (!xsRes_Efka._IsSuccessful)
                {
                    response.AddErrors(xsRes_Efka._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }
                decimal? currentMonthPensionsSum = 0;
                if (xsRes_Efka.Pensions.Count > 0) { 
                    foreach (var pension in xsRes_Efka.Pensions)
                    {
                        if (pension.Month == DateTime.Now.Month)
                        {
                            currentMonthPensionsSum = currentMonthPensionsSum + pension.GrossAmountBasic + pension.GrossAmountAdditional;
                        }
                    }
                }
                member.PensionAmount = currentMonthPensionsSum != 0 ? currentMonthPensionsSum : null;

                #endregion

            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }

        private async Task<ProvideMemberDataResponse> TryProvideSpouseData(Application application, Person member)
        {
            ProvideMemberDataResponse response = new ProvideMemberDataResponse(_errorLogger, _currentUserContext.UserName);
            try
            {
                #region 1. Get data from AMKA
                var provideMemberDataResponse = await ProvideMemberData(member);

                if (!provideMemberDataResponse._IsSuccessful)
                {
                    response.AddErrors(provideMemberDataResponse._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }
                #endregion

                #region 2. Get efka info
                GetEfkaInfoResponse xsRes_Efka = null;
                xsRes_Efka = await _personService.GetEfkaInfoAsync(new GetEfkaInfoRequest()
                {
                    ApplicationId = application.Id,
                    AFM = member.AFM,
                    AMKA = member.AMKA,
                    DateFrom = new DateTime(2016, 05, 01).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DateTo = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                });

                if (!xsRes_Efka._IsSuccessful)
                {
                    response.AddErrors(xsRes_Efka._Errors);
                    response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

                    return response;
                }
                decimal? currentMonthPensionsSum = 0;
                if (xsRes_Efka.Pensions.Count > 0)
                {
                    foreach (var pension in xsRes_Efka.Pensions)
                    {
                        if (pension.Month == DateTime.Now.Month)
                        {
                            currentMonthPensionsSum = currentMonthPensionsSum + pension.GrossAmountBasic + pension.GrossAmountAdditional;
                        }
                    }
                }
                member.PensionAmount = currentMonthPensionsSum != 0 ? currentMonthPensionsSum : null;

                #endregion

            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }

        public async Task<ProvideMemberDataResponse> ProvideMemberData(Person person)
		{
			ProvideMemberDataResponse response = new ProvideMemberDataResponse(_errorLogger, _currentUserContext.UserName);

			try
			{
				var xsRes_Amka = await _personService.GetAmkaRegistryAsync(new PersonService.GetAmkaRegistryRequest()
				{
					Person = person
				});

				if (!xsRes_Amka._IsSuccessful)
				{
					response.AddErrors(xsRes_Amka._Errors);
					response.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);

					return response;
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }
    }

	public class CreateApplicationRequest : NEEServiceRequestBase
	{
		public string AFM { get; set; }
		public string AMKA { get; set; }

		public override List<string> IsValid()
		{
			return null;
		}
		/// <summary>
		/// get remote host ip to pass to the Validate IBAN WS
		/// </summary>
		public string RemoteHostIP { get; set; }
	}

	public class CreateApplicationResponse : NEEServiceResponseBase
	{
		public CreateApplicationResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
		{
		}
		public Application CreatedApplication { get; set; }
	}

    public class CreateApplicationForPersonResponse : NEEServiceResponseBase
    {
        public CreateApplicationForPersonResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public Application CreatedApplication { get; set; } = null;
    }

    public class ProvideMembersDataResponse : NEEServiceResponseBase
    {
        public ProvideMembersDataResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }
    }

    public class ProvideMemberDataResponse : NEEServiceResponseBase
    {
        public ProvideMemberDataResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public List<Person> GsisMembers { get; set; } = null;
    }
}
