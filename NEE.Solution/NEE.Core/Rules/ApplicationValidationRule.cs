using NEE.Core.BO;

namespace NEE.Core.Rules
{
    public abstract class ApplicationValidationRule : Rule
    {
        public static string IbanNotValid = "Ο αριθμός τραπεζικού λογαριασμού (IBAN) δεν είναι έγκυρος";
        public static string IbanNotFound = "Δεν έχει καταχωρηθεί IBAN";
        public static string IbanNotOwned = "Δεν είστε δικαιούχος/συνδικαιούχος στον τραπεζικό λογαριασμό που αντιστοιχεί ο IBAN που δηλώσατε ή ο λογαριασμός είναι ανενεργός.";
        public static string UnsupportedBank = "Η τράπεζα του λογαριασμού σας δεν υποστηρίζεται.";
        public static string ErganhRecordFound = "Είστε δηλωμένος στο ΕΡΓΑΝΗ σύμφωνα με τον ΑΦΜ σας.";
        public static string AgeBelow67 = "Δεν έχετε συμπληρώσει το 67ο έτος της ηλικίας σας.";
        public static string EdtoNotExpatriate = "Δεν υπάρχουν στοιχεία ομόγενειας με βάση τα προσωπικά σας στοιχεία.";
        public static string PensionExceeded = "Το ποσό των μηνιαίων σας συντάξεων υπερβαίνει το όριο του επιδόματος.";
        public static string AADEIncomeExceeded = "Το εισόδημά σας υπερβαίνει τις 4,320 ευρώ.";
        public static string AADEFamilyIncomeExceeded = "Το εισόδημα ζεύγους υπερβαίνει τις 8,640 ευρώ.";
        public static string AADERealEstateExceeded = "Η αξία της ακίνητης σας περιουσίας υπερβαίνει τις 90,000 ευρώ.";
		public static string AADEVehicleExceeded = "Η αξία των οχημάτων σας υπερβαίνει τις 6,000 ευρώ.";
        public static string HousingBenefitExceeded = "Το ποσό που λαμβάνετε από το επίδομα Στέγασης υπερβαίνει το όριο του επιδόματος.";
        public static string HousingAssistanceBenefitExceeded  = "Το ποσό που λαμβάνετε από το επίδομα Στεγαστικής Συνδρομής υπερβαίνει το όριο του επιδόματος.";
        public static string BenefitForOmogeneisExceeded  = "";
        public static string DisabilityBenefitsExceeded  = "Το συνολικό ποσό που λαμβάνετε από τα Αναπηρικά επιδόματα υπερβαίνει το όριο του επιδόματος.";
        public static string A21BenefitExceeded = "Το ποσό που λαμβάνετε από το επίδομα Παιδιού (Α21) υπερβαίνει το όριο του επιδόματος.";
        public static string SpousePensionExceeded = "Το ποσό σύνταξης του συζύγου σας υπερβαίνει τα 387,9 ευρώ.";
        public static string SpousePensionFromAlbaniaExceeded = "Το ποσό σύνταξης του συζύγου σας από Αλβανία υπερβαίνει τα 387,9 ευρώ.";
        public static string PensionFromAlbaniaExceeded = "Το ποσό των μηνιαίων σας συντάξεων από Αλβανία υπερβαίνει τα 387,9 ευρώ.";
        public static string SpousePensionSumExceeded = "Το άθροισμα των συντάξεων του συζύγου σας υπερβαίνει τα 387,9 ευρώ.";
        public static string PensionSumExceeded = "Το άθροισμα των συντάξεών σας υπερβαίνει τα 387,9 ευρώ.";
        public static string FEKUploaded = "Έχετε επισυνάψει ΦΕΚ/Απόφαση πολιτογράφησης.";
        public static string PensionAlbaniaUploaded = "Έχετε επισυνάψει βεβαίωση Φορέα Συνταξιοδότησης/υπεύθυνη δήλωση.";
        public static string MaritalStatusUploaded = "Έχετε επισυνάψει βεβαίωση οικογενειακής κατάστασης.";
        public static string SpousePensionAlbaniaUploaded = "Έχετε επισυνάψει βεβαίωση Φορέα Συνταξιοδότησης/υπεύθυνη δήλωση για τον/την σύζυγό σας.";

        public Application Application { get; set; }

        protected Remark RelatedRemark { get; set; }

        public ApplicationValidationRule(Application application, string name) : base(name)
        {
            Application = application;
        }

        abstract public Remark GetRelatedRemark();
    }
}
