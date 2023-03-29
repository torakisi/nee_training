using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Database;
using NEE.Database.Entities;
using NEE.Database.Entities_Extra;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Service
{
    internal static class ΒΟMapper
    {
        public static NEE_ErrorLog Map(this ErrorLog src, NEE_ErrorLog dest = null)
        {
            if (dest == null)
                dest = new NEE_ErrorLog();

            dest.User = src.User;
            dest.CreatedAt = src.CreatedAt;
            dest.Exception = src.Exception;
            dest.ApplicationId = src.ApplicationId;
            dest.ErrorLogSource = src.ErrorLogSource;
            dest.StackTrace = src.StackTrace;
            dest.InnerException = src.InnerException;

            return dest;
        }

        public static Application Map(this NEE_App src, Application dest = null)
        {
            if (dest == null)
                dest = new Application();

            // Map here
            dest.Id = src.Id;
            dest.Revision = src.Revision;
            dest.Version = src.Version;
            dest.SubmittedAt = src.SubmittedAt;
            dest.SubmittedBy = src.SubmittedBy;
            dest.CanceledAt = src.CanceledAt;
            dest.CanceledBy = src.CanceledBy;
            dest.RejectedAt = src.RejectedAt;
            dest.RejectedBy = src.RejectedBy;
            dest.ApprovedAt = src.ApprovedAt;
            dest.ApprovedBy = src.ApprovedBy;
            dest.IsCreatedByKK = src.IsCreatedByKK;
            dest.IsModifiedByKK = src.IsModifiedByKK;
            dest.SetState(src.State, null);
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.SpouseAFM = src.SpouseAFM;
            dest.SpouseAMKA = src.SpouseAMKA;

            dest.IBAN = src.IBAN;
            dest.Email = src.Email;
            dest.HomePhone = src.HomePhone;
            dest.MobilePhone = src.MobilePhone;
            dest.Applicant.City = src.City;
            dest.Applicant.District = src.District;
            dest.Amount = src.Amount;
            dest.DeclarationLaw1599 = src.DeclarationLaw1599;
            dest.PensionDocumentAlbaniaId = src.PensionDocumentAlbaniaId;
            dest.PensionDocumentAlbaniaId2 = src.PensionDocumentAlbaniaId2;
            dest.SpousePensionDocumentAlbaniaId = src.SpousePensionDocumentAlbaniaId;
            dest.SpousePensDocumentAlbaniaId2 = src.SpousePensDocumentAlbaniaId2;
            dest.MaritalStatusId = src.MaritalStatusId;
            dest.MaritalStatusId2 = src.MaritalStatusId2;
            dest.FEKDocumentId = src.FEKDocumentId;
            dest.FEKDocumentId2 = src.FEKDocumentId2;
            dest.ProvidedFEKDocument = src.ProvidedFEKDocument;

            var applicant = src.Members.Where(w => w.Relationship == MemberRelationship.Applicant).FirstOrDefault();
            dest.Applicant = applicant.Map();

            var spouse = src.Members.Where(w => w.Relationship == MemberRelationship.Spouse).FirstOrDefault();
            if (spouse != null)
            {
                dest.Spouse = spouse.Map();
            }

            foreach (NEE_AppRemark remark in src.Remarks)
            {
                var mapped = remark.Map();

                if (mapped.ReferToMember)
                {
                    var member = src.Members.Where(x => x.AMKA == mapped.RelatedAMKA).FirstOrDefault();
                    if (member != null)
                    {
                        var memberMap = member.Map();
                        mapped.MemberFirstName = memberMap.FirstName;
                        mapped.MemberLastName = memberMap.LastName;
                    }
                }

                dest.Remarks.Add(mapped);
            }

            foreach (NEE_AppPerson member in src.Members.OrderBy(x => x.PersonId))
            {
                var mapped = member.Map();
                mapped.RelatedRemarks = dest.Remarks.Where(x => x.RelatedAMKA == mapped.AMKA && x.ReferToMember).ToList();
                dest.Members.Add(mapped);
            }

            foreach (NEE_AppStateChange appStateChange in src.ApplicationStateChange)
            {
                var mapped = appStateChange.Map();
                dest.ApplicationStateChange.Add(mapped);
            }

            foreach (NEE_AppFile file in src.Files)
            {
                var mapped = file.Map();
                dest.Files.Add(mapped);

                if (dest.HasFEKDocument2)
                {
                    if (dest.FEKDocumentId2 == file.Url)
                    {
                        dest.FEKDocumentRejectionReason2 = file.RejectionReason;
                        dest.IsFEKDocumentApproved = file.IsApproved;
                        dest.FEKDocumentName = file.Filename;
                    }
                }
                else
                {
                    if (dest.FEKDocumentId == file.Url)
                    {
                        dest.FEKDocumentRejectionReason = file.RejectionReason;
                        dest.IsFEKDocumentApproved = file.IsApproved;
                        dest.FEKDocumentName = file.Filename;
                    }
                }               

                if (dest.HasMaritalStatusDocument2)
                {
                    if (dest.MaritalStatusId2 == file.Url)
                    {
                        dest.MaritalStatusDocumentRejectionReason2 = file.RejectionReason;
                        dest.IsMaritalStatusDocumentApproved = file.IsApproved;
                        dest.MaritalStatusDocumentName = file.Filename;
                    }
                }
                else
                {
                    if (dest.MaritalStatusId == file.Url)
                    {
                        dest.MaritalStatusDocumentRejectionReason = file.RejectionReason;
                        dest.IsMaritalStatusDocumentApproved = file.IsApproved;
                        dest.MaritalStatusDocumentName = file.Filename;
                    }
                }

                if (dest.HasPensionAlbaniaDocument2)
                {
                    if (dest.PensionDocumentAlbaniaId2 == file.Url)
                    {
                        dest.PensionAlbaniaDocumentRejectionReason2 = file.RejectionReason;
                        dest.IsPensionAlbaniaDocumentApproved = file.IsApproved;
                        dest.PensionDocumentAlbaniaName = file.Filename;
                    }
                }
                else
                {
                    if (dest.PensionDocumentAlbaniaId == file.Url)
                    {
                        dest.PensionAlbaniaDocumentRejectionReason = file.RejectionReason;
                        dest.IsPensionAlbaniaDocumentApproved = file.IsApproved;
                        dest.PensionDocumentAlbaniaName = file.Filename;
                    }
                }

                if (dest.HasSpousePensionDocument2)
                {
                    if (dest.SpousePensDocumentAlbaniaId2 == file.Url)
                    {
                        dest.SpousePensionDocumentRejectionReason2 = file.RejectionReason;
                        dest.IsSpousePensionDocumentApproved = file.IsApproved;
                        dest.SpousePensDocumentName = file.Filename;
                    }
                }
                else
                {
                    if (dest.SpousePensionDocumentAlbaniaId == file.Url)
                    {
                        dest.SpousePensionDocumentRejectionReason = file.RejectionReason;
                        dest.IsSpousePensionDocumentApproved = file.IsApproved;
                        dest.SpousePensDocumentName = file.Filename;
                    }
                }
            }

            return dest;
        }

        public static NEE_App Map(this Application src, NEE_App dest = null)
        {
            if (dest == null)
                dest = new NEE_App();

            // Map here
            dest.Id = src.Id;
            dest.State = (AppState)src.State;
            dest.Revision = src.Revision;
            dest.Version = src.Version;
            dest.SubmittedAt = src.SubmittedAt;
            dest.SubmittedBy = src.SubmittedBy;
            dest.CanceledAt = src.CanceledAt;
            dest.CanceledBy = src.CanceledBy;
            dest.RejectedAt = src.RejectedAt;
            dest.RejectedBy = src.RejectedBy;
            dest.IsCreatedByKK = src.IsCreatedByKK;
            dest.IsModifiedByKK = src.IsModifiedByKK;
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.SpouseAFM = src.SpouseAFM;
            dest.SpouseAMKA = src.SpouseAMKA; 
            
            dest.IBAN = src.IBAN;
            dest.Amount = src.Amount;
            dest.DeclarationLaw1599 = src.DeclarationLaw1599;

            dest.Email = src.Applicant.Email ?? src.Email;
            dest.HomePhone = src.Applicant.HomePhone ?? src.HomePhone;
            dest.MobilePhone = src.Applicant.MobilePhone ?? src.MobilePhone;        

            // documents
            dest.PensionDocumentAlbaniaId = src.PensionDocumentAlbaniaId;
            dest.PensionDocumentAlbaniaId2 = src.PensionDocumentAlbaniaId2;
            dest.SpousePensionDocumentAlbaniaId = src.SpousePensionDocumentAlbaniaId;
            dest.SpousePensDocumentAlbaniaId2 = src.SpousePensDocumentAlbaniaId2;
            dest.MaritalStatusId = src.MaritalStatusId;
            dest.MaritalStatusId2 = src.MaritalStatusId2;
            dest.FEKDocumentId = src.FEKDocumentId;
            dest.FEKDocumentId2 = src.FEKDocumentId2;
            dest.ProvidedFEKDocument = src.ProvidedFEKDocument;

            //applicant
            var dbApplicant = dest.Members.Where(m => m.Relationship == MemberRelationship.Applicant).FirstOrDefault();
            if (dbApplicant != null)
            {
                src.Applicant.Map(dbApplicant);
            }
            else
            {
                var dbMember = src.Applicant.Map();
                dbMember.Id = src.Id;
                dest.Members.Add(dbMember);
            }
            //spouse
            if (src.Spouse != null)
            {
                var dbSpouse = dest.Members.Where(m => m.Relationship == MemberRelationship.Spouse).FirstOrDefault();
                if (dbSpouse != null)
                {
                    src.Spouse.Map(dbSpouse);
                }
                else
                {
                    var dbMember = src.Spouse.Map();
                    dbMember.Id = src.Id;
                    dest.Members.Add(dbMember);
                }
            }

            //int i = dest.Remarks.Count;
            //foreach (var srcRemark in src.Remarks)
            //{
            //    if (!dest.Remarks.Any(s => s.Message == srcRemark.Message)) //srcRemark is new
            //    {
            //        i++;
            //        var dbRemark = new NEE_AppRemark();
            //        srcRemark.Map(dbRemark);
            //        dbRemark.Id = dest.Id;
            //        dbRemark.Name = "default";
            //        dbRemark.Index = i;
            //        dest.Remarks.Add(dbRemark);
            //    }
            //}
            dest.Remarks.Clear();
            int i = 0;
            foreach (var srcRemark in src.Remarks)
            {
                i++;
                var dbRemark = new NEE_AppRemark();
                srcRemark.Map(dbRemark);
                dbRemark.Id = dest.Id;
                dbRemark.Name = "default";
                dbRemark.Index = i;
                dest.Remarks.Add(dbRemark);
            }

            return dest;
        }

        public static ZipCode Map(this DIC_ZipCodes src, ZipCode dest = null)
        {
            if (dest == null)
                dest = new ZipCode();

            // Map here
            dest.Code = src.Code;
            dest.City = src.City;
            dest.District = src.District;

            return dest;
        }

        public static Faq Map(this SUP_Faq src, Faq dest = null)
        {
            if (dest == null)
                dest = new Faq();

            // Map here
            dest.Id = src.Id;
            dest.Title = src.Title;
            dest.Description = src.Description;
            dest.Order = src.Order;
            dest.SectionTitle = src.SectionTitle;
            dest.SectionOrder = src.SectionOrder;
            dest.DisableQ = src.DisableQ;
            dest.IsForAdmin = src.IsForAdmin;

            return dest;
        }

        public static NEE_AppFile Map(this AppFileSave src, NEE_AppFile dest = null)
        {
            if (dest == null)
                dest = new NEE_AppFile();

            // Map here 
            dest.ApplicationId = src.ApplicationId;
            dest.Filename = src.FileName;
            dest.FileSize = src.FileSize;
            dest.FileType = src.FileType;
            dest.Url = src.Url;
            dest.UploadedFromIP = src.UploadedFromIP;
            dest.CreatedAt = src.CreatedAt;
            dest.CreatedBy = src.CreatedBy;
            return dest;
        }

        public static NEE_AppPerson Map(this Person src, NEE_AppPerson dest = null)
        {
            if (dest == null)
                dest = new NEE_AppPerson();

            // Map here
            dest.PersonId = src.PersonId.HasValue ? src.PersonId.Value : -1;
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.DOB = src.DOB ?? null;
            dest.DOD = src.DOD;
            dest.DeathStatus = src.DeathStatus;
            dest.MemberState = src.MemberState;
            dest.Relationship = src.Relationship;

            dest.LastName = src.LastName;
            dest.LastNameBirth = src.LastNameBirth;
            dest.FirstName = src.FirstName;
            dest.LastNameEN = src.LastNameEN;
            dest.FirstNameEN = src.FirstNameEN;

            dest.FatherName = src.FatherName;
            dest.MotherName = src.MotherName;
            dest.FatherNameEN = src.FatherNameEN;
            dest.MotherNameEN = src.MotherNameEN;

            dest.Gender = src.Gender;
            dest.BirthCountry = src.BirthCountry;
            dest.CitizenCountry = src.CitizenCountry;

            //notifications center
            dest.Street = src.Street;
            dest.StreetNumber = src.StreetNumber;
            dest.City = src.City;
            dest.Zip = src.Zip;
            dest.PostalNumber = src.PostalNumber;
            dest.Region = src.Region;
            dest.RegionalUnit = src.RegionalUnit;
            dest.Municipality = src.Municipality;
            dest.MunicipalUnit = src.MunicipalUnit;
            dest.Commune = src.Commune;

            if (src.CreatedAt != null && src.CreatedAt != DateTime.MinValue)
                dest.CreatedAt = src.CreatedAt;
            if (src.CreatedBy != null)
                dest.CreatedBy = src.CreatedBy;
            dest.ModifiedAt = src.ModifiedAt;
            dest.ModifiedBy = src.ModifiedBy;
            dest.EntityId = src.EntityId;
            dest.Revision = src.Revision;
            dest.IdentificationNumber = src.IdentificationNumber;
            dest.IdentificationType = src.IdentificationNumberType;
            dest.IdentificationEndDate = src.IdentificationNumberEndDate;
            dest.IdentificationNumberConfirmed = src.IdentificationNumberConfirmed;
            dest.DeletedAt = src.DeletedAt;
            dest.DeletedBy = src.DeletedBy;
            dest.RestoredAt = src.RestoredAt;
            dest.RestoredBy = src.RestoredBy;
            dest.MaritalStatus = src.MaritalStatus;
            dest.UnknownMaritalStatus = src.UnknownMaritalStatus;

            //edto
            dest.PermitNumber = src.PermitNumber;
            dest.AdministrationDate = src.AdministrationDate;

            //ergani
            dest.IsEmployed = src.IsEmployed;

            //financial
            dest.Income = src.Income;
            dest.FamilyIncome = src.FamilyIncome;
            dest.AssetsValue = src.AssetsValue;
            dest.VehiclesValue = src.VehiclesValue;
            dest.PensionAmount = src.PensionAmount;
            dest.PensionFromAlbania = src.PensionFromAlbania == "Ναι";
            dest.PensionAmountAlbania = src.PensionAmountAlbania;
            dest.Currency = src.Currency;
            dest.PensionStartDateAlbania = src.PensionStartDateAlbania;

            //benefits
            dest.HousingBenefit = src.HousingBenefit;
            dest.KAYBenefit = src.KAYBenefit;
            dest.HousingAssistanceBenefit = src.HousingAssistanceBenefit;
            dest.A21Benefit = src.A21Benefit;
            dest.DisabilityBenefits = src.DisabilityBenefits;
            dest.BenefitForOmogeneis = src.BenefitForOmogeneis;
            dest.PensionDocumentAlbaniaId = src.PensionDocumentAlbaniaId;
            return dest;
        }

        public static Person Map(this NEE_AppPerson src, Person dest = null)
        {
            if (dest == null)
                dest = new Person();

            // Map here
            dest.PersonId = src.PersonId;
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.DOB = src.DOB;
            dest.DOD = src.DOD;
            dest.DeathStatus = src.DeathStatus;
            dest.MemberState = src.MemberState;
            dest.Relationship = src.Relationship;

            dest.LastName = src.LastName;
            dest.LastNameBirth = src.LastNameBirth;
            dest.FirstName = src.FirstName;
            dest.LastNameEN = src.LastNameEN;
            dest.FirstNameEN = src.FirstNameEN;

            dest.FatherName = src.FatherName;
            dest.MotherName = src.MotherName;
            dest.FatherNameEN = src.FatherNameEN;
            dest.MotherNameEN = src.MotherNameEN;

            dest.Gender = src.Gender;
            dest.CitizenCountry = src.CitizenCountry;
            dest.BirthCountry = src.BirthCountry;

            dest.Street = src.Street;
            dest.StreetNumber = src.StreetNumber;
            dest.Zip = src.Zip;
            dest.City = src.City;
            dest.PostalNumber= src.PostalNumber;
            dest.Region = src.Region;
            dest.RegionalUnit = src.RegionalUnit;
            dest.Municipality = src.Municipality;
            dest.MunicipalUnit = src.MunicipalUnit;
            dest.Commune = src.Commune;
            dest.CreatedAt = src.CreatedAt;
            dest.CreatedBy = src.CreatedBy;
            dest.ModifiedAt = src.ModifiedAt;
            dest.ModifiedBy = src.ModifiedBy;
            dest.EntityId = src.EntityId;
            dest.Revision = src.Revision;
            dest.IdentificationNumber = src.IdentificationNumber;
            dest.IdentificationNumberEndDate = src.IdentificationEndDate;
            dest.IdentificationNumberType = src.IdentificationType;
            dest.IdentificationNumberConfirmed = src.IdentificationNumberConfirmed;
            dest.DeletedAt = src.DeletedAt;
            dest.DeletedBy = src.DeletedBy;
            dest.RestoredAt = src.RestoredAt;
            dest.RestoredBy = src.RestoredBy;
            dest.MaritalStatus = src.MaritalStatus;
            dest.UnknownMaritalStatus = src.UnknownMaritalStatus;
            //edto
            dest.PermitNumber = src.PermitNumber;
            dest.AdministrationDate = src.AdministrationDate;
            //ergani
            dest.IsEmployed = src.IsEmployed;
            //financial
            dest.TaxisReferenceYear = src.TaxisReferenceYear;
            dest.Income = src.Income;
            dest.FamilyIncome = src.FamilyIncome;
            dest.AssetsValue = src.AssetsValue;
            dest.VehiclesValue = src.VehiclesValue;
            dest.PensionAmount = src.PensionAmount;
            dest.PensionFromAlbania = src.PensionFromAlbania ? "Ναι" : "Όχι";
            dest.PensionAmountAlbania = src.PensionAmountAlbania;
            dest.Currency = src.Currency;
            dest.PensionStartDateAlbania = src.PensionStartDateAlbania;
            //benefits
            dest.DisabilityBenefits = src.DisabilityBenefits;
            dest.BenefitForOmogeneis = src.BenefitForOmogeneis;
            dest.A21Benefit = src.A21Benefit;
            dest.HousingBenefit = src.HousingBenefit;
            dest.KAYBenefit = src.KAYBenefit;
            dest.HousingAssistanceBenefit = src.HousingAssistanceBenefit;
            dest.PensionDocumentAlbaniaId = src.PensionDocumentAlbaniaId;

            return dest;
        }

        public static StateChange Map(this NEE_AppStateChange src, StateChange dest = null)
        {
            if (dest == null)
                dest = new StateChange();

            dest.Id = src.Id;
            dest.ChangedAt = src.ChangedAt;
            dest.ChangedBy = src.ChangedBy;
            dest.ChangeId = src.ChangeId;
            dest.StateFrom = src.StateFrom;
            dest.StateTo = src.StateTo;
            dest.ChangeReason = src.ChangeReason;
            dest.IsFromDb = true;
            dest.ReferenceDate = src.ReferenceDate;
            dest.SearchForAxreostitos = src.SearchForAxreostitos;

            return dest;
        }

        public static AppFile Map(this NEE_AppFile src, AppFile dest = null)
        {
            if (dest == null)
                dest = new AppFile();

            dest.ApplicationId = src.ApplicationId;
            dest.IsApproved = src.IsApproved;
            dest.RejectionReason = src.RejectionReason;
            dest.ApprovedBy = src.ApprovedBy;
            dest.ApprovedAt = src.ApprovedAt;
            dest.RejectedBy = src.RejectedBy;
            dest.RejectedAt = src.RejectedAt;
            dest.CreatedBy = src.CreatedBy;
            dest.CreatedAt = src.CreatedAt;
            dest.ModifiedBy = src.ModifiedBy;
            dest.ModifiedAt = src.ModifiedAt;
            dest.Url = src.Url;
            dest.Filename = src.Filename;
            return dest;
        }

        public static int Map(this GMI_UserDetails src)
        {
            var dest = 0;
            switch (src.Municipality)
            {
                case "Athens":
                    dest = 0;
                    break;
                case "KentrikiMakedonia":
                    dest = 1;
                    break;
                case "DitikiMakedonia":
                    dest = 3;
                    break;
                case "AnatolikiMakedonia":
                    dest = 4;
                    break;
                case "DitikiEllada":
                    dest = 5;
                    break;
                case "Crete":
                    dest = 6;
                    break;
                case "Thessalia":
                    dest = 7;
                    break;
                case "StereaEllada":
                    dest = 10;
                    break;
                case "Hpeiros":
                    dest = 11;
                    break;
                case "Peloponnisos":
                    dest = 12;
                    break;
            }
            return dest;
        }

        public static Remark Map(this NEE_AppRemark src, Remark dest = null)
        {
            if (dest == null)
                dest = new Remark();

            // Map here
            dest.RemarkCode = src.RemarkCode;
            dest.Description = src.Description;
            dest.Status = src.Status;
            dest.Severity = src.Severity;
            dest.Message = src.Message;
            dest.RelatedAMKA = src.RelatedAMKA;
            dest.RelatedAFM = src.RelatedAFM;
            dest.Released = src.Released;
            dest.ReleaseText = src.ReleaseText;
            dest.ReleasedAt = src.ReleasedAt;
            dest.ReleasedBy = src.ReleasedBy;
            dest.EntityId = src.EntityId;
            dest.Revision = src.Revision;

            dest.IsFromDB = true;
            dest.ReferToMember = src.ReferToMember;

            return dest;
        }

        public static NEE_AppRemark Map(this Remark src, NEE_AppRemark dest = null)
        {
            if (dest == null)
                dest = new NEE_AppRemark();

            // Map here
            dest.RemarkCode = src.RemarkCode;
            dest.Description = src.Description;
            dest.Status = src.Status;
            dest.Severity = src.Severity;
            dest.Message = src.Message;
            dest.RelatedAMKA = src.RelatedAMKA;
            dest.RelatedAFM = src.RelatedAFM;
            dest.Released = src.Released;
            dest.ReleaseText = src.ReleaseText;
            dest.ReleasedAt = src.ReleasedAt;
            dest.ReleasedBy = src.ReleasedBy;
            dest.ReferToMember = src.ReferToMember;

            return dest;
        }

        public static PaymentTransactions Map(this NEE_PaymentTransactions src, PaymentTransactions dest = null)
        {
            if (dest == null)
                dest = new PaymentTransactions();

            // Map here
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

        public static PaymentsWebView Map(this NEE_PaymentsWebView src, PaymentsWebView dest = null)
        {
            if (dest == null)
                dest = new PaymentsWebView();

            // Map here
            dest.Id = src.Id;
            dest.Amount = src.Amount;
            dest.IBAN = src.IBAN;
            dest.AFM = src.AFM;
            dest.AMKA = src.AMKA;
            dest.CreatedAt = src.CreatedAt;
            dest.Reason = src.Reason;
            dest.Confirmed = src.Confirmed;
            dest.Processed = src.Processed;
            dest.ProcessedInPayment = src.ProcessedInPayment;
            dest.ProcessedAt = src.ProcessedAt;
            dest.ProcessResult = src.ProcessResult;
            dest.MasterTransactionId = src.MasterTransactionId;
            dest.State = src.State;
            dest.Description = src.Description;

            foreach (var paymentTransaction in src.PaymentTransactions)
            {
                dest.PaymentTransactions.Add(paymentTransaction.Map());
            }

            return dest;
        }
    }
}
