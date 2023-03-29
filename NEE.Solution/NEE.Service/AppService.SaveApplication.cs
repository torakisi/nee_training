using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Service.Authorization;
using NEE.Service.Core;
using NEE.Core.BO;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.ModelBinding;

namespace NEE.Service
{
    public partial class AppService
    {

        public async Task<SaveApplicationResponse> SaveApplicationAsync(SaveApplicationRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.SaveApplication,
                ReferencedApplicationId = req.Id,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            SaveApplicationResponse resp = new SaveApplicationResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                var currentAppRevision = req.Revision;

                var getAppResp = await this.GetApplicationAsync(new GetApplicationRequest()
                {
                    Id = req.Id,
                    Revision = currentAppRevision,
                    ForWhatUse = GetApplicationUse.SaveApplication
                // pass the Remote Host IP for the IBAN Validation WS 
                ,
                    RemoteHostIP = req.RemoteHostIP
                });

                if (!getAppResp._IsSuccessful)
                {
                    resp.AddErrors(getAppResp.UIDisplayedErrors);

                    return resp;
                }                              

                var application = getAppResp.Application;

                if (application.State == AppState.Approved)
                {
                    List<string> requestValidationReason = req.IsValid();
                    if (requestValidationReason.Any())
                        return SaveApplicationResponse.RequestNotValid(requestValidationReason);

                    if (!string.IsNullOrEmpty(req.Iban) && application.IBAN != req.Iban)
                    {

                        var ibanResp = await this.ValidateIBAN(new ValidateIBANRequest()
                        {
                            Application = application,
                            AFM = application.AFM,
                            IBAN = req.Iban,
                            auditProtocol = application.Id,
                            auditUnit = "OPEKA",
                            auditUserId = _currentUserContext.UserName,
                            auditUserIp = req.RemoteHostIP
                        });

                        if (ibanResp.Result == IbanValidationServiceResult.IncorrectCombination)
                        {
                            return SaveApplicationResponse.NotOwnIbanInput(req.Iban);
                        }
                    }

                    if (!Email.IsValid(req.Email))
                    {
                        return SaveApplicationResponse.NotValidInput("Email", req.Email);
                    }

                    if (!MobilePhone.IsValid(req.MobilePhone))
                    {
                        return SaveApplicationResponse.NotValidInput("Κινητό Τηλέφωνο", req.MobilePhone);
                    }
                }

                if (!ServiceAuthorization.AccessApplicationShouldBeApplicantAuthorized(_currentUserContext, application))
                    throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_MESSAGE);

                if (application == null)
                    return SaveApplicationResponse.NotFound(req.Id);

                application.LoadFurtherChecks(_currentUserContext);
                if (!application.IsEditableApplication)
                    return SaveApplicationResponse.NotEditable(application.Id);
                                
                application.Email = req.Email;
                application.IBAN = req.Iban;
                application.HomePhone = req.HomePhone;
                application.MobilePhone = req.MobilePhone;
                application.ProvidedFEKDocument = req.ProvidedFEKDocument == "Ναι";
                application.DeclarationLaw1599 = req.DeclarationLaw1599;
                var applicant = application.Applicant;
                if (applicant != null)
                {
                    applicant.Street = req.Street;
                    applicant.StreetNumber = req.StreetNumber;
                    applicant.Zip = req.Zip;
                    applicant.PostalNumber = req.PostalNumber;
                    applicant.MaritalStatus = req.MaritalStatus;
                    applicant.MobilePhone = req.MobilePhone;
                    applicant.Email = req.Email;
                    applicant.HomePhone = req.HomePhone;
                    applicant.PensionAmount= req.PensionAmount;
                    applicant.PensionFromAlbania = req.PensionFromAlbania;
                    applicant.PensionAmountAlbania = req.PensionAmountAlbania;
                    applicant.Currency = req.ApplicantCurrency;
                    applicant.PensionStartDateAlbania = req.PensionStartDateAlbania;
                }
                var spouse = application.Spouse;
                if (spouse != null)
                {
                    spouse.PensionFromAlbania = req.SpousePensionFromAlbania;
                    spouse.PensionAmountAlbania = req.SpousePensionAmountAlbania;
                    spouse.Currency = req.SpouseCurrency;
                    spouse.PensionStartDateAlbania = req.SpousePensionStartDateAlbania;
                }

                if (req.HasFEKDocumentDecision) application.IsFEKDocumentApproved = req.IsFEKDocumentApproved;
                if (req.HasPensionAlbaniaDocumentDecision) application.IsPensionAlbaniaDocumentApproved = req.IsPensionAlbaniaDocumentApproved;
                if (req.HasSpousePensionDocumentDecision) application.IsSpousePensionDocumentApproved = req.IsSpousePensionDocumentApproved;
                if (req.HasMaritalStatusDocumentDecision) application.IsMaritalStatusDocumentApproved = req.IsMaritalStatusDocumentApproved;
                
                application.RemoteHostIP = req.RemoteHostIP;  // pass remote host ID for the IBAN Validation WS

                if (application.State != AppState.Approved)
                    await CreateValidationRemarksForApplicationAsync(application);

                await repository.Save(application, req.IsModelStateValid);

                if (req.ReturnApplication)
                {
                    application = await repository.Load(application.Id, application.Revision);
                    application.LoadFurtherChecks(_currentUserContext);
                }
                return SaveApplicationResponse.Saved(application, req.ReturnApplication);
            }
            catch (Exception ex)
            {
                resp.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return resp;
        }

    }

    public class SaveApplicationRequest : ApplicationIdentityRequest
    {
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Zip { get; set; }
        public string PostalNumber { get; set; }
        public string Email { get; set; }
        public string Iban { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string ProvidedFEKDocument { get; set; }
        public string FEKDocumentName { get; set; }
        public string MaritalStatusDocumentName { get; set; }
        public string PensionDocumentAlbaniaName { get; set; }
        public string SpousePensDocumentName { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public bool UnknownMaritalStatus { get; set; }
        public bool ReturnApplication { get; set; }
        public bool ShouldValidate { get; set; }
        public bool ShouldSaveRemarks { get; set; } = false;
        public List<Remark> ChangedRemarks { get; set; }
        public decimal? PensionAmount { get; set; }
        public string PensionFromAlbania { get; set; }
        public decimal? PensionAmountAlbania { get; set; }
        public string ApplicantCurrency { get; set; }
        public DateTime? PensionStartDateAlbania { get; set; }
        public string SpousePensionFromAlbania { get; set; }
        public decimal? SpousePensionAmountAlbania { get; set; }
        public string SpouseCurrency { get; set; }
        public DateTime? SpousePensionStartDateAlbania { get; set; }
        public bool? IsSpousePensionDocumentApproved { get; set; }
        public bool HasSpousePensionDocumentDecision
        {
            get
            {
                return IsSpousePensionDocumentApproved != null;
            }
        }
        public bool? IsPensionAlbaniaDocumentApproved { get; set; }
        public bool HasPensionAlbaniaDocumentDecision
        {
            get
            {
                return IsPensionAlbaniaDocumentApproved != null;
            }
        }
        public bool? IsMaritalStatusDocumentApproved { get; set; }
        public bool HasMaritalStatusDocumentDecision
        {
            get
            {
                return IsMaritalStatusDocumentApproved != null;
            }
        }
        public bool? IsFEKDocumentApproved { get; set; }
        public bool HasFEKDocumentDecision
        {
            get
            {
                return IsFEKDocumentApproved != null;
            }
        }
        public bool DeclarationLaw1599 { get; set; }
        public bool IsModelStateValid { get; set; }

        public override List<string> IsValid()
        {
            string genericMessage = "Η αποθήκευση δεν πραγματοποιήθηκε";
            List<string> ret = new List<string>();

            if (!NEE.Core.Helpers.Iban.IsValid(Iban))
            {
                ret.Add($"To ΙΒΑΝ που προσπαθήσατε να εισάγετε δεν είναι έγκυρο. {genericMessage}");
            }

            if (!NEEUserHelper.IsEmailValid(Email) || string.IsNullOrEmpty(Email))
            {
                ret.Add($"Το Email που προσπαθήσατε να εισάγετε δεν είναι έγκυρο. {genericMessage}");
            }

            if (string.IsNullOrEmpty(MobilePhone) || MobilePhone.Length != 10)
            {
                ret.Add($"Το Κινητό Τηλέφωνο που προσπαθήσατε να εισάγετε πρέπει να έχει τιμή. {genericMessage}");
            }
            return ret;
        }

        /// <summary>
        /// get remote host ip to pass to the Validate IBAN WS
        /// </summary>
        public string RemoteHostIP { get; set; }

    }

    public class SaveApplicationResponse : NEEServiceResponseBase
    {
        public SaveApplicationResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public string FEKDocumentId { get; set; }
        public string PensionDocumentAlbaniaId { get; set; }
        public string MaritalStatusId { get; set; }
        public string SpousePensionDocumentAlbaniaId { get; set; }
        public Application Application { get; set; }

        public static SaveApplicationResponse NotFound(string id)
        {
            var res = new SaveApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.Not_Found, $"Δε βρέθηκε αίτηση με κωδικό {id}");
            return res;
        }

        public static SaveApplicationResponse PersonNotFound()
        {
            var res = new SaveApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.Not_Found, $"Δε βρέθηκε ο αιτών");
            return res;
        }

        public static SaveApplicationResponse NotEditable(string id)
        {
            var res = new SaveApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.Not_Found, $"Δεν έχετε δικαίωμα επεξεργασίας στην αίτηση με κωδικό: {id}");
            return res;
        }

        public static SaveApplicationResponse NotOwnIbanInput(string iban)
        {
            var res = new SaveApplicationResponse();
            res.AddError(ErrorCategory.UIDisplayed, $"Tο IBAN [{iban}] που εισάγατε δεν αντιστοιχεί στον αιτούντα. Η αποθήκευση δεν πραγματοποιήθηκε.");
            return res;
        }

        public static SaveApplicationResponse NotValidInput(string inputName, string inputValue)
        {
            var res = new SaveApplicationResponse();
            res.AddError(ErrorCategory.UIDisplayed, $"Για το πεδίο '{inputName}' εισαγάγατε την μη έγκυρη τιμή '{inputValue}'. Η αποθήκευση δεν πραγματοποιήθηκε.");
            return res;
        }

        public static SaveApplicationResponse Saved(Application application, bool returnApplication)
        {
            var res = new SaveApplicationResponse
            {
                _IsSuccessful = true,
                FEKDocumentId = application.HasFEKDocument2 ? application.FEKDocumentId2 : application.FEKDocumentId,
                PensionDocumentAlbaniaId = application.HasPensionAlbaniaDocument2 ? application.PensionDocumentAlbaniaId2 : application.PensionDocumentAlbaniaId,
                MaritalStatusId = application.HasMaritalStatusDocument2 ? application.MaritalStatusId2 : application.MaritalStatusId,
                SpousePensionDocumentAlbaniaId = application.HasSpousePensionDocument2 ? application.SpousePensDocumentAlbaniaId2 : application.SpousePensionDocumentAlbaniaId
            };

            if (returnApplication)
                res.Application = application;

            return res;
        }

        public static SaveApplicationResponse RequestNotValid(List<string> notValidReasons)
        {
            var res = new SaveApplicationResponse();

            foreach (string notValidReason in notValidReasons)
            {
                res.AddError(ErrorCategory.UIDisplayed, $"{notValidReason}");
            }

            return res;
        }
    }
}
