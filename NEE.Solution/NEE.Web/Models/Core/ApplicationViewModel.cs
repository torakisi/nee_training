using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NEE.Core.BO;
using System.Web.Mvc;
using NEE.Core.Validation;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Web.Mvc.Html;
using System.Web;

namespace NEE.Web.Models.Core
{
    public class ApplicationViewModel : ViewModelBase
    {
        public class ApplicantPersonalInfoViewModel
        {
            [Display(Name = "ΑΜΚΑ")]
            public string AMKA { get; set; }
            [Display(Name = "ΑΦΜ")]
            public string AFM { get; set; }
            [Display(Name = "ΑΔΤ")]
            public string IdDocument { get; set; }
            [Display(Name = "Αριθμός Διαβατηρίου")]
            public string Passport { get; set; }
            [Display(Name = "Επώνυμο")]
            public string LastName { get; set; }
            [Display(Name = "Όνομα")]
            public string FirstName { get; set; }
            [Display(Name = "Επώνυμο (Λατινικά)")]
            public string LastNameEn { get; set; }
            [Display(Name = "Όνομα (Λατινικά)")]
            public string FirstNameEn { get; set; }
            [Display(Name = "Πατρώνυμο")]
            public string FatherName { get; set; }
            [Display(Name = "Μητρώνυμο")]
            public string MotherName { get; set; }
            [Display(Name = "Πατρώνυμο (Λατινικά)")]
            public string FatherNameEn { get; set; }
            [Display(Name = "Μητρώνυμο (Λατινικά)")]
            public string MotherNameEn { get; set; }
            [Display(Name = "Επώνυμο (κατά τη γέννηση)")]
            public string LastNameBirth { get; set; }
            [Display(Name = "Υπηκοότητα")]
            public string Nationality { get; set; }
            [Display(Name = "Σταθερό τηλέφωνο")]
            [StringLength(10, ErrorMessage = "Τηλ. Κατοικίας (10 ψηφία)")]
            public string HomePhone { get; set; }
            [Display(Name = "Ημ/νία Γέννησης")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public string DateOfBirth { get; set; }
            [Display(Name = "Χώρα Γέννησης")]
            public string BirthCountry { get; set; }
            [Display(Name = "Οικογενειακή Κατάσταση"), NEEValidationAttributes]
            public MaritalStatus? MaritalStatus { get; set; }
            [Display(Name = "Άγνωστη Οικογενειακή Κατάσταση")]
            public bool UnknownMaritalStatus { get; set; }
            public bool HasSpouse { get; set; }
            public string MaritalStatusDocumentName { get; set; }
        }
        public class ApplicationContactInfoViewModel
        {
            [Display(Name = "ΙΒΑΝ"), NEEValidationAttributes]
            [StringLength(27, ErrorMessage = "IBAN (27 ψηφία)")]
            public string IBAN { get; set; }
            public ChangeLog LatestIbanChange { get; set; }
            [Display(Name = "Email"), NEEValidationAttributes]
            [EmailAddress]
            public string Email { get; set; }
            [Display(Name = "Τηλ. Κατοικίας")]
            [StringLength(10, ErrorMessage = "Τηλ. Κατοικίας (10 ψηφία)")]
            public string HomePhone { get; set; }
            [Display(Name = "Κινητό Τηλέφωνο"), NEEValidationAttributes]
            [StringLength(10, ErrorMessage = "Κινητό (10 ψηφία)")]
            public string MobilePhone { get; set; }
        }
        public class ApplicationAddressInfoViewModel
        {
            [Display(Name = "Οδός")]
            public string Street { get; set; }
            [Display(Name = "Αριθμός")]
            public string StreetNumber { get; set; }
            [Display(Name = "ΤΚ"), NEEValidationAttributes]
            public string Zip { get; set; }
            [Display(Name = "Ταχ.Θυρίδα")]
            public string PostalNumber { get; set; }
            [Display(Name = "Δήμος")]
            public string Municipality { get; set; }
            [Display(Name = "Πόλη/Χωριό")]
            public string City { get; set; }
            [Display(Name = "Δημοτική Ενότητα")]
            public string MunicipalUnit { get; set; }
            [Display(Name = "Περιφέρεια")]
            public string Region { get; set; }
            [Display(Name = "Περιφερειακή Ενότητα")]
            public string RegionalUnit { get; set; }
            [Display(Name = "Δημοτική Κοινότητα")]
            public string Commune { get; set; }

            private IEnumerable<SelectListItem> cityList = ZipCodes.Cities.Select(c => new SelectListItem { Text = (string.IsNullOrEmpty(c) ? "" : c), Value = (string.IsNullOrEmpty(c) ? null : c) }).ToArray();
            public IEnumerable<SelectListItem> CityList => cityList;

            private IEnumerable<SelectListItem> districtList = ZipCodes.Districts.Select(c => new SelectListItem { Text = (string.IsNullOrEmpty(c) ? "" : c), Value = (string.IsNullOrEmpty(c) ? null : c) }).ToArray();
            public IEnumerable<SelectListItem> DistrictList => districtList;

            private IEnumerable<SelectListItem> maritalStatusList = EnumHelper.GetSelectList(typeof(MaritalStatus));

            public IEnumerable<SelectListItem> MaritalStatusList => maritalStatusList;

        }
        public class SpouseInfoViewModel
        {
            [Display(Name = "Επώνυμο")]
            public string LastName { get; set; }
            [Display(Name = "Όνομα")]
            public string FirstName { get; set; }
            [Display(Name = "Πατρώνυμο")]
            public string FatherName { get; set; }
            [Display(Name = "Μητρώνυμο")]
            public string MotherName { get; set; }
            [Display(Name = "ΑΜΚΑ")]
            public string AMKA { get; set; }
            [Display(Name = "ΑΦΜ")]
            public string AFM { get; set; }

            //financial info
            [Display(Name = "Λαμβάνω σύνταξη από Ελλάδα")]
            public bool PensionFromGreece { get; set; }
            [Display(Name = "Μηνιαίο ποσό σύνταξης")]
            public decimal? PensionAmount { get; set; }
            [Display(Name = "Λαμβάνω σύνταξη από Αλβανία")]
            public string PensionFromAlbania { get; set; }
            [Display(Name = "Ημ/νία έναρξης συνταξιοδότησης")]
            public string PensionStartDateAlbania { get; set; }
            [Display(Name = "Μηνιαίο ποσό σύνταξης")]
            [RegularExpression(@"^[0-9]*([.,][0-9]{1,2})?$", ErrorMessage = "Παρακαλώ εισάγετε έγκυρο αριθμό (με , για δεκαδικά ψηφία)")]
            public string PensionAmountAlbania { get; set; }
            [Display(Name = "Νόμισμα")]
            public string Currency { get; set; }
            [Display(Name = "Βεβαίωση φορέα συνταξιοδότησης")]
            public bool PensionCertification { get; set; }
        }
        public class ApplicantFinancialInfoViewModel
        {
            [Display(Name = "Λαμβάνω σύνταξη από Ελλάδα")]
            public bool PensionFromGreece { get; set; }
            //[Display(Name = "Ημ/νία έναρξης συνταξιοδότησης")]
            //public DateTime? PensionStartDate { get; set; }
            [Display(Name = "Μηνιαίο ποσό σύνταξης")]
            public decimal? PensionAmount { get; set; }
            [Display(Name = "Εισόδημα")]
            public decimal? Income { get; set; }
            [Display(Name = "Εισόδημα ζεύγους")]
            public decimal? FamilyIncome { get; set; }
            [Display(Name = "Αξία ακινήτων")]
            public decimal? AssetsValue { get; set; }
            [Display(Name = "Αξία οχημάτων")]
            public decimal? VehiclesValue { get; set; }
            [Display(Name = "Λαμβάνω σύνταξη από Αλβανία")]
            public string PensionFromAlbania { get; set; }
            [Display(Name = "Ημ/νία έναρξης συνταξιοδότησης")]
            //[DataType(DataType.Date)]
            public string PensionStartDateAlbania { get; set; }
            [Display(Name = "Μηνιαίο ποσό σύνταξης")]
            [RegularExpression(@"^[0-9]*([.,][0-9]{1,2})?$", ErrorMessage = "Παρακαλώ εισάγετε έγκυρο αριθμό (με , για δεκαδικά ψηφία)")]
            public string PensionAmountAlbania { get; set; }
            [Display(Name = "Νόμισμα")]
            public string Currency { get; set; }
            [Display(Name = "Βεβαίωση φορέα συνταξιοδότησης")]
            public bool PensionCertification { get; set; }

            //benefits
            [Display(Name = "Επίδομα στέγασης")]
            public decimal? HousingBenefit { get; set; }
            [DisplayName("Επίδομα στεγαστικής συνδρομής")]
            public decimal? HousingAssistanceBenefit { get; set; }
            [DisplayName("Επίδομα ομογενών")]
            public decimal? BenefitForOmogeneis { get; set; }
            [Display(Name = "Αναπηρικά επιδόματα")]
            public decimal? DisabilityBenefits { get; set; }
            [Display(Name = "Επίδομα παιδιού (A21)")]
            public decimal? A21Benefit { get; set; }
        }
        public class ApplicationEDTOInfoViewModel
        {
            [Display(Name = "Αριθμός ΕΔΤΟ")]
            public string EDTONumber { get; set; }
            [Display(Name = "Ημ/νία έκδοσης ΕΔΤΟ")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime? EDTOIssuingDate { get; set; }
            [Display(Name = "Διαθέτω ΦΕΚ / Απόφαση Πολιτογράφησης")]
            public string ProvidedFEKDocument { get; set; }
            public string FEKDocumentName { get; set; }            
        }

        public List<string> YesNoList = new List<string> { "Ναι", "Όχι" };
        public List<string> EurLekList = new List<string> { "LEK", "EUR" };

        #region application fields
        [Display(Name = "Κωδικός Αίτησης")]
        public string Id { get; set; }
        [Display(Name = "Ημ/νία υποβολής αίτησης")]
        public string SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        [Display(Name = "Κατάσταση Αίτησης")]
        public AppState State { get; set; }
        public int Revision { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DraftAt { get; set; }
        public string DraftBy { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string CanceledBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
        public string ArchiveReason { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string FullUsername { get; set; }
        public bool ShowUserFullName { get; set; }
        public bool CanDeleteMember { get; set; }
        public bool CanHandleRemarks { get; set; }
        public bool IsReadOnlyApplication { get; set; }
        public bool IsEditableApplication { get; set; }
        public bool CanViewOnlyApplication { get; set; }
        public bool LockedByKK { get; set; }        
        public bool CanEditResults { get; set; }        
        public bool CanBeSuspendedApplication { get; set; }
        public bool CanBeRecalledApplication { get; set; }
        public bool CanUndoSuspendApplication { get; set; }
        public bool CanUndoRecallApplication { get; set; }
        public bool CanPrintApplication { get; set; }
        public bool CanPrintRecallAct { get; set; }
        public bool CanPrintWholeApplication { get; set; }
        public bool CanBeApproved { get; set; }
        public bool CanViewApplicationSubmittedDetails { get; set; }
        [Display(Name = "Αιτία Αναστολής")]
        public bool IsRequiredRejectReason { get; set; } = false;
        public string ChangeSuspendReason { get; set; }
        public string FEKRejectionReason { get; set; }
        public string PensionRejectionReason { get; set; }
        public string MaritalStatusRejectionReason { get; set; }
        public string SpouseRejectionReason { get; set; }
        [Display(Name = "Αιτία Ανάκλησης")]
        public string ChangeRecallReason { get; set; }
        public bool IsApprovable { get; set; }
        public bool CanBeSubmitted { get; set; }
        [Display(Name = "Η οριστική υποβολή της αίτησής μου υπέχει θέση υπεύθυνης δήλωσής μου σύμφωνα με τις διατάξεις του ν. 1599/1986 (ΦΕΚ Α΄75) ως προς την ορθότητα των στοιχείων και την πλήρωση των προϋποθέσεων χορήγησης του επιδόματος, όπως αυτές προβλέπονται από το οικείο νομοθετικό πλαίσιο. «Όποιος εν γνώσει του δηλώνει ψευδή γεγονότα ή αρνείται ή αποκρύπτει τα αληθινά με έγγραφη υπεύθυνη δήλωση του άρθρου 8 τιμωρείται με φυλάκιση τουλάχιστον τριών μηνών. Εάν ο υπαίτιος αυτών των πράξεων σκόπευε να προσπορίσει στον εαυτόν του ή σε άλλον περιουσιακό όφελος βλάπτοντας τρίτον ή σκόπευε να βλάψει άλλον, τιμωρείται με κάθειρξη μέχρι 10 ετών».")]
        [NEEMustBeTrue]
        public bool DeclarationLaw1599 { get; set; }
        [DisplayFormat(DataFormatString = "{0:n} €")]
        [Display(Name = "Ποσό Επιδόματος Κοινωνικής Αλληλεγγύης Ανασφάλιστων Υπερηλίκων σε μέλη της ελληνικής μειονότητας της Αλβανίας")]
        public decimal? BenefitAmount { get; set; }
        public decimal? EntitledAmount { get; set; }
        public bool IsAdminUser { get; set; }
        public bool IsAFMUser { get; set; }
        public bool IsApplicationCreatedByKK { get; set; }
        public bool IsApplicationModifiedByKK { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public DateTime? PayFrom { get; set; }
        public DateTime? PayTo { get; set; }
        public DateTime? InitialPayTo { get; set; }
        public DateTime? InitialPeriodTo { get; set; }
        #endregion

        #region document fields
        public DocumentCategory DocumentCategory { get; set; }
        public string PensionDocumentAlbaniaId { get; set; }
        public string PensionDocumentAlbaniaName { get; set; }
        public string SpousePensionDocumentAlbaniaId { get; set; }
        public string SpousePensDocumentName { get; set; }
        public string MaritalStatusId { get; set; }
        public string FEKDocumentId { get; set; }
        public bool? IsPensionAlbaniaDocumentApproved { get; set; }
        public bool? IsSpousePensionDocumentApproved { get; set; }
        public bool? IsMaritalStatusDocumentApproved { get; set; }
        public bool? IsFEKDocumentApproved { get; set; }
        #endregion

        public ApplicantPersonalInfoViewModel ApplicantPersonalInfo { get; set; }
        public SpouseInfoViewModel SpouseInfo { get; set; }
        public ApplicantFinancialInfoViewModel ApplicantFinancialInfo { get; set; }
        public ApplicationContactInfoViewModel ApplicationContactInfo { get; set; }
        public ApplicationAddressInfoViewModel AddressInfo { get; set; }
        public ApplicationEDTOInfoViewModel EDTOInfo { get; set; }
        public List<PersonViewModel> Members { get; set; }
        public List<ApplicationLogViewModel> ApplicationLog { get; set; }
        public List<RemarkViewModel> Remarks { get; set; }
        public List<ChangeStateHistoryModel> ChangeStateLog { get; set; }

        
        
        public ApplicationViewModel()
        {
            EDTOInfo = new ApplicationEDTOInfoViewModel();
            ApplicantPersonalInfo = new ApplicantPersonalInfoViewModel();
            ApplicantFinancialInfo = new ApplicantFinancialInfoViewModel();
            SpouseInfo = new SpouseInfoViewModel();
            ApplicationContactInfo = new ApplicationContactInfoViewModel();
            AddressInfo = new ApplicationAddressInfoViewModel();
            Remarks = new List<RemarkViewModel>();
            Members = new List<PersonViewModel>();
            ApplicationLog = new List<ApplicationLogViewModel>();
            ChangeStateLog = new List<ChangeStateHistoryModel>();
        }

        public ApplicationEditCommands Command { get; set; }

        public enum ApplicationEditCommands
        {
            MemberAction,
            ValidateSave,
            Save,
            ValidateSaveFinalSubmit,
            CancelReturn,
            AddMember,
            ViewMember,
            EditMember,
            SaveRemoveMember,
            SaveSetMemberStateDeleted,
            SaveSetMemberStateNormal,
            SaveAddMember,
            SaveEditMember,
            SavePartialEditMember,
            ViewApplication,
            ViewApplicationReadOnly,
            EditApplication,
            ConcentToApplication,
            CreateNewApplication,
            SaveViewMember,
            RedirectToManage,
            PartialEditApplication,
            ValidatePartialSave,
            ForeasConcentToApplication,
            PrintApplication,
            NoAction,
            RemarkReferenceAnchor,
            SuspendApplication,
            RecallApplication,
            UndoSuspendApplication,
            ApplicationSubmitted,
            PrintWholeApplication,
            UndoRecallApplication,
            GoToPayments,
            FileDownload,
            PensionAlbaniaUpload,
            SpousePensionAlbaniaUpload,
            MaritalStatusUpload,
            FEKUpload,
            ApproveMaritalStatus,
            RejectMaritalStatus,
            ApprovePensionAlbania,
            RejectPensionAlbania,
            ApproveSpousePensionAlbania,
            RejectSpousePensionAlbania,
            ApproveFEK,
            RejectFEK,

        }

        public enum MemberActions
        {
            View,
            Delete,
            Remove
        }

        public bool CanBeFinalSubmitted
        {
            get
            {
                if (this.State != AppState.Rejected && this.State != AppState.Approved && this.State != AppState.Canceled)
                {
                    if (IsAdminUser || (!IsAdminUser && this.State != AppState.Submitted))
                        return true;
                }

                return false;
            }
        }

        public bool FileUploadDisabled
        {
            get
            {
                return !CanBeFinalSubmitted || !CanEditResults || LockedByKK;
            }
        }
        
        public bool IsInFinalStatus { get; set; }        
    }
}