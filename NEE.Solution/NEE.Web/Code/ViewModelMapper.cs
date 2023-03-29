using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Web.Models.Admin;
using NEE.Web.Models.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NEE.Web.Code
{
    public static class ViewModelMapper
    {
        public static ApplicationViewModel Map(this Application src, ApplicationViewModel dest = null)
        {
            if (dest == null)
                dest = new ApplicationViewModel();

            dest.Id = src.Id;
            dest.Revision = src.Revision;
            dest.State = src.State.Value;
            dest.SubmittedAt = src.SubmittedAt != null ? ((DateTime)src.SubmittedAt).ToString("dd/MM/yyyy") : null;
            dest.ApplicantPersonalInfo.AFM = src.AFM;
            dest.ApplicantPersonalInfo.AMKA = src.AMKA;

            if (src.Applicant.IdentificationNumberType == IdentificationNumberType.ADT)
                dest.ApplicantPersonalInfo.IdDocument = src.Applicant.IdentificationNumber;
            else if (src.Applicant.IdentificationNumberType == IdentificationNumberType.Passport)
                dest.ApplicantPersonalInfo.Passport = src.Applicant.IdentificationNumber;
            dest.ApplicantPersonalInfo.LastName = src.Applicant.LastName;
            dest.ApplicantPersonalInfo.FirstName = src.Applicant.FirstName;
            dest.ApplicantPersonalInfo.LastNameEn = src.Applicant.LastNameEN;
            dest.ApplicantPersonalInfo.FirstNameEn = src.Applicant.FirstNameEN;
            dest.ApplicantPersonalInfo.FatherName = src.Applicant.FatherName;
            dest.ApplicantPersonalInfo.MotherName = src.Applicant.MotherName;
            dest.ApplicantPersonalInfo.FatherNameEn = src.Applicant.FatherNameEN;
            dest.ApplicantPersonalInfo.MotherNameEn = src.Applicant.MotherNameEN;
            dest.ApplicantPersonalInfo.LastNameBirth = src.Applicant.LastNameBirth;
            dest.ApplicantPersonalInfo.DateOfBirth = src.Applicant.DOB != null ? ((DateTime)src.Applicant.DOB).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            dest.ApplicantPersonalInfo.MaritalStatus = src.Applicant.MaritalStatus;
            dest.ApplicantPersonalInfo.UnknownMaritalStatus = src.Applicant.UnknownMaritalStatus;
            dest.ApplicantPersonalInfo.MaritalStatusDocumentName = src.MaritalStatusDocumentName;
            dest.ApplicantPersonalInfo.HasSpouse = src.Applicant.HasSpouse;
            dest.ApplicantPersonalInfo.Nationality = src.Applicant.CitizenCountry;
            dest.ApplicantPersonalInfo.BirthCountry = src.Applicant.BirthCountry;

            dest.ApplicationContactInfo.IBAN = src.IBAN;
            dest.ApplicationContactInfo.Email = src.Email;
            dest.ApplicationContactInfo.HomePhone = src.HomePhone;
            dest.ApplicationContactInfo.MobilePhone = src.MobilePhone;

            dest.AddressInfo.Street = src.Applicant.Street;
            dest.AddressInfo.StreetNumber = src.Applicant.StreetNumber;
            dest.AddressInfo.Zip = src.Applicant.Zip;
            dest.AddressInfo.PostalNumber = src.Applicant.PostalNumber;
            dest.AddressInfo.Municipality = src.Applicant.Municipality;
            dest.AddressInfo.City = src.Applicant.City;
            dest.AddressInfo.MunicipalUnit = src.Applicant.MunicipalUnit;
            dest.AddressInfo.Region = src.Applicant.Region;
            dest.AddressInfo.RegionalUnit = src.Applicant.RegionalUnit;
            dest.AddressInfo.Commune = src.Applicant.Commune;

            #region applicant financial info
            dest.ApplicantFinancialInfo.Income = src.Applicant.Income;
            dest.ApplicantFinancialInfo.FamilyIncome = src.Applicant.FamilyIncome;
            dest.ApplicantFinancialInfo.AssetsValue = src.Applicant.AssetsValue;
            dest.ApplicantFinancialInfo.VehiclesValue = src.Applicant.VehiclesValue;
            dest.ApplicantFinancialInfo.PensionFromGreece = src.Applicant.PensionAmount != null;
            dest.ApplicantFinancialInfo.PensionAmount = src.Applicant.PensionAmount;
            dest.ApplicantFinancialInfo.PensionFromAlbania = src.Applicant.PensionFromAlbania;
            dest.ApplicantFinancialInfo.PensionAmountAlbania = src.Applicant.PensionAmountAlbania != null ? src.Applicant.PensionAmountAlbania.ToString() : null;
            dest.ApplicantFinancialInfo.Currency = src.Applicant.Currency;
            dest.ApplicantFinancialInfo.PensionStartDateAlbania = src.Applicant.PensionStartDateAlbania != null ? ((DateTime)src.Applicant.PensionStartDateAlbania).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            #endregion

            dest.EDTOInfo.EDTONumber = src.Applicant.PermitNumber;
            dest.EDTOInfo.EDTOIssuingDate = src.Applicant.AdministrationDate;
            dest.EDTOInfo.ProvidedFEKDocument = src.ProvidedFEKDocument ? "Ναι" : "Όχι";
            dest.PensionDocumentAlbaniaId = src.HasPensionAlbaniaDocument2? src.PensionDocumentAlbaniaId2: src.PensionDocumentAlbaniaId;
            dest.SpousePensionDocumentAlbaniaId = src.HasSpousePensionDocument2 ? src.SpousePensDocumentAlbaniaId2 : src.SpousePensionDocumentAlbaniaId;
            dest.SpousePensDocumentName = src.SpousePensDocumentName;
            dest.MaritalStatusId = src.HasMaritalStatusDocument2 ? src.MaritalStatusId2 : src.MaritalStatusId;
            dest.FEKDocumentId = src.HasFEKDocument2 ? src.FEKDocumentId2 : src.FEKDocumentId;
            dest.PensionDocumentAlbaniaName = src.PensionDocumentAlbaniaName;
            dest.EDTOInfo.FEKDocumentName = src.FEKDocumentName;
            dest.IsPensionAlbaniaDocumentApproved = src.IsPensionAlbaniaDocumentApproved;
            dest.IsSpousePensionDocumentApproved = src.IsSpousePensionDocumentApproved;
            dest.IsMaritalStatusDocumentApproved = src.IsMaritalStatusDocumentApproved;
            dest.IsFEKDocumentApproved = src.IsFEKDocumentApproved;

            dest.BenefitAmount = src.Amount;

            #region applicant benefits info

            dest.ApplicantFinancialInfo.DisabilityBenefits = src.Applicant.DisabilityBenefits;
            dest.ApplicantFinancialInfo.A21Benefit = src.Applicant.A21Benefit;
            dest.ApplicantFinancialInfo.HousingBenefit = src.Applicant.HousingBenefit;
            dest.ApplicantFinancialInfo.HousingAssistanceBenefit = src.Applicant.HousingAssistanceBenefit;
            dest.ApplicantFinancialInfo.BenefitForOmogeneis = src.Applicant.BenefitForOmogeneis;

            #endregion

            #region spouse info
            if (src.Spouse!=null)
            {
                dest.SpouseInfo.AMKA = src.Spouse.AMKA;
                dest.SpouseInfo.AFM = src.Spouse.AFM;
                dest.SpouseInfo.FirstName = src.Spouse.FirstName;
                dest.SpouseInfo.LastName = src.Spouse.LastName;
                dest.SpouseInfo.FatherName = src.Spouse.FatherName;
                dest.SpouseInfo.MotherName = src.Spouse.MotherName;

                dest.SpouseInfo.PensionFromGreece = src.Spouse.PensionAmount != null;
                dest.SpouseInfo.PensionAmount = src.Spouse.PensionAmount;
                dest.SpouseInfo.PensionFromAlbania = src.Spouse.PensionFromAlbania;
                dest.SpouseInfo.PensionAmountAlbania = src.Spouse.PensionAmountAlbania != null ? src.Spouse.PensionAmountAlbania.ToString() : null;
                dest.SpouseInfo.Currency = src.Spouse.Currency;
                dest.SpouseInfo.PensionStartDateAlbania = src.Spouse.PensionStartDateAlbania != null ? ((DateTime)src.Spouse.PensionStartDateAlbania).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            }
            #endregion

            dest.CanHandleRemarks = src.CanHandleRemarks;
            dest.IsReadOnlyApplication = src.IsReadOnlyApplication;
            dest.IsEditableApplication = src.IsEditableApplication;
            dest.CanViewOnlyApplication = src.CanViewOnlyApplication;
            dest.LockedByKK = src.LockedByKK;
            dest.IsInFinalStatus = src.IsInFinalStatus;
            dest.IsApplicationCreatedByKK = src.IsApplicationCreatedByKK;
            dest.IsApplicationModifiedByKK = src.IsApplicationModifiedByKK;
            dest.CanBeApproved = src.CanBeApproved;
            dest.CanBeSubmitted = src.CanBeSubmitted;
            dest.IsApprovable = src.IsApprovable;
            dest.IsAdminUser = src.IsAdminUser;
            dest.IsAFMUser = src.IsAFMUser;
            dest.DeclarationLaw1599 = src.DeclarationLaw1599;

            foreach (Person member in src.Members)
            {
                dest.Members.Add(member.Map(src.CanHandleRemarks, src.IsAdminUser));
            }
            foreach (Remark remark in src.Remarks)
            {
                dest.Remarks.Add(remark.Map());
            }
            return dest;
        }

        public static List<SearchViewModel.SearchResultTableViewModel> MapList(this List<SearchApplication> srcList)
        {
            List<SearchViewModel.SearchResultTableViewModel> destList = new List<SearchViewModel.SearchResultTableViewModel>();

            foreach (SearchApplication src in srcList)
            {
                destList.Add(src.Map());
            }

            return destList;
        }

        public static SearchViewModel.SearchResultTableViewModel Map(this SearchApplication src, SearchViewModel.SearchResultTableViewModel dest = null)
        {
            if (dest == null)
                dest = new SearchViewModel.SearchResultTableViewModel();

            dest.Id = src.Id;
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.Municipality = src.Municipality;
            dest.FirstName = src.FirstName;
            dest.LastName = src.LastName;
            dest.FirstNameEN = src.FirstNameEN;
            dest.LastNameEN = src.LastNameEN;
            dest.CitizenCountry = src.CitizenCountry;
            dest.Zip = src.Zip;
            dest.IBAN = src.IBAN;
            dest.Email = src.Email;
            dest.Applicant_FirstName = src.Applicant_FirstName;
            dest.Applicant_LastName = src.Applicant_LastName;
            dest.Applicant_FirstNameEN = src.Applicant_FirstNameEN;
            dest.Applicant_LastNameEN = src.Applicant_LastNameEN;
            dest.Applicant_CitizenCountry = src.Applicant_CitizenCountry;
            dest.HomePhone = src.HomePhone;
            dest.MobilePhone = src.MobilePhone;
            dest.State = src.State;
            dest.IsEditableApplicationSearch = src.IsEditableApplicationSearch;
            dest.CanViewOnlyApplicationSearch = src.CanViewOnlyApplicationSearch;
            Enum.TryParse(src.DistrictId, out OpekaDistricts district);
            dest.DistrictId = district;
            return dest;
        }

        public static Remark Map(this RemarkViewModel src, Remark dest = null)
        {
            if (dest == null)
                dest = new Remark();

            dest.RemarkCode = src.RemarkCode;
            dest.Description = src.Description;
            dest.Status = src.Status;
            dest.Severity = (NEERemarkSeverity) src.Severity;
            dest.Message = src.Message;
            dest.RelatedAMKA = src.RelatedAMKA;
            dest.RelatedAFM = src.RelatedAFM;
            dest.Released = src.Released;
            dest.ReleaseText = src.ReleaseText;
            dest.ReleasedBy = src.ReleasedBy;
            dest.ReleasedAt = src.ReleasedAt;
            dest.ReleaseSelection = src.ReleaseSelection;
            dest.ApprovedByKK = src.ApprovedByKK;

            return dest;
        }
        public static RemarkViewModel Map(this Remark src, RemarkViewModel dest = null)
        {
            if (dest == null)
                dest = new RemarkViewModel();

            dest.RemarkCode = src.RemarkCode;
            dest.Description = src.Description;
            dest.Status = src.Status;
            dest.Severity = (int)src.Severity;
            dest.Message = src.Message;
            dest.RelatedAMKA = src.RelatedAMKA;
            dest.RelatedAFM = src.RelatedAFM;
            dest.Released = src.Released;
            dest.ReleaseText = src.ReleaseText;
            dest.ReleasedBy = src.ReleasedBy;
            dest.ReleasedAt = src.ReleasedAt;
            dest.ReleaseSelection = src.ReleaseSelection;
            dest.ReleaseTextDescription = src.ReleaseTextDescription;
            dest.ReferToMember = src.ReferToMember;

            if (src.ReferToMember)
                dest.FullNameDescr = src.FullNameWithAMKA;

            dest.ReasonForNoApproval = src.ReasonForNoApproval;
            dest.ReasonForNoSubmitted = src.ReasonForNoSubmitted;


            return dest;
        }
        public static PersonViewModel Map(this Person src, bool canHandleRemarks, bool isNormalUser, PersonViewModel dest = null)
        {
            if (dest == null)
                dest = new PersonViewModel();

            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.DOB = src.DOB;
            dest.MemberState = src.MemberState;
            dest.Relationship = src.Relationship;
            dest.LastName = src.LastName;
            dest.FirstName = src.FirstName;
            dest.LastNameEN = src.LastNameEN;
            dest.FirstNameEN = src.FirstNameEN;
            dest.FatherName = src.FatherName;
            dest.MotherName = src.MotherName;
            dest.FatherNameEN = src.FatherNameEN;
            dest.MotherNameEN = src.MotherNameEN;
            dest.Gender = src.Gender;
            dest.CitizenCountry = src.CitizenCountry;
            dest.CreatedAt = src.CreatedAt;
            dest.CreatedBy = src.CreatedBy;
            dest.ModifiedAt = src.ModifiedAt;
            dest.ModifiedBy = src.ModifiedBy;

            dest.CanViewMember = src.CanViewMember;
            dest.CanEditMember = src.CanEditMember;
            dest.IdentificationNumber = src.IdentificationNumber;
            dest.IdentificationNumberEndDate = src.IdentificationNumberEndDate;
            dest.IdentificationNumberType = src.IdentificationNumberType;
            dest.MaritalStatus = src.MaritalStatus;
            dest.UnknownMaritalStatus = src.UnknownMaritalStatus;
            dest.CanHandleRemark = canHandleRemarks;
            dest.IsNormalUser = isNormalUser;

            dest.ApplicationRevision = src.ApplicationRevision;


            foreach (Remark remark in src.RelatedRemarks)
            {
                dest.RelatedRemarks.Add(remark.Map(src));
            }
            return dest;
        }
        public static RemarkViewModel Map(this Remark src, Person person, RemarkViewModel dest = null)
        {
            if (dest == null)
                dest = new RemarkViewModel();

            dest.RemarkCode = src.RemarkCode;
            dest.Description = src.Description;
            dest.Status = src.Status;
            dest.Severity = (int)src.Severity;
            dest.Message = src.Message;
            dest.RelatedAMKA = src.RelatedAMKA;
            dest.RelatedAFM = src.RelatedAFM;
            dest.Released = src.Released;
            dest.ReleaseText = src.ReleaseText;
            dest.ReleasedBy = src.ReleasedBy;
            dest.ReleasedAt = src.ReleasedAt;
            dest.ReleaseSelection = src.ReleaseSelection;
            dest.ReleaseTextDescription = src.ReleaseTextDescription;
            dest.ReferToMember = src.ReferToMember;
            if (src.ReleaseSelection != null && src.ReleaseSelection == Core.Contracts.Enumerations.RemarkSelection.Approve)
                dest.ApprovedByKK = true;
            else
                dest.ApprovedByKK = false;

            dest.ReasonForNoApproval = src.ReasonForNoApproval;
            dest.ReasonForNoSubmitted = src.ReasonForNoSubmitted;
            return dest;
        }
        public static PaymentsWebViewViewModel Map(this PaymentsWebView src, PaymentsWebViewViewModel dest = null)
        {
            if (dest == null)
                dest = new PaymentsWebViewViewModel();

            dest.Id = src.Id;
            dest.Amount = src.Amount;
            dest.IBAN = src.IBAN;
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.CreatedAt = src.CreatedAt;
            dest.Reason = src.Reason;
            dest.MasterTransactionId = src.MasterTransactionId;
            dest.Confirmed = src.Confirmed;
            dest.Processed = src.Processed;
            dest.ProcessedInPayment = src.ProcessedInPayment;
            dest.ProcessedAt = src.ProcessedAt;
            dest.ProcessResult = src.ProcessResult;
            dest.State = src.State;
            dest.Description = src.Description;

            foreach (var paymentTransaction in src.PaymentTransactions)
            {
                dest.PaymentTransactions.Add(paymentTransaction.Map());
            }

            return dest;
        }
        public static PaymentTransactionsViewModel Map(this PaymentTransactions src, PaymentTransactionsViewModel dest = null)
        {
            if (dest == null)
                dest = new PaymentTransactionsViewModel();

            dest.Id = src.Id;
            dest.Amount = src.Amount;
            dest.IBAN = src.IBAN;
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.ReferenceMonth = src.ReferenceMonth;
            dest.CreatedAt = src.CreatedAt;
            dest.Type = src.Type;
            dest.Reason = src.Reason;
            dest.TransactionId = src.TransactionId;
            dest.Confirmed = src.Confirmed;
            dest.Processed = src.Processed;
            dest.ProcessedInPayment = src.ProcessedInPayment;
            dest.ProcessedAt = src.ProcessedAt;
            dest.CreatedBy = src.CreatedBy;

            return dest;
        }
        public static List<PaymentTransactionsViewModel> MapList(this List<PaymentTransactions> srcList)
        {
            List<PaymentTransactionsViewModel> destList = new List<PaymentTransactionsViewModel>();

            foreach (PaymentTransactions src in srcList)
            {
                destList.Add(src.Map());
            }

            return destList;
        }
    }
}