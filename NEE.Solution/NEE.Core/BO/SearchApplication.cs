using NEE.Core.Contracts.Enumerations;

namespace NEE.Core.BO
{
    public class SearchApplication
    {
        public string Id { get; set; }
        public AppState State { get; set; }
        public string Applicant_LastName { get; set; }
        public string Applicant_FirstName { get; set; }
        public string Applicant_LastNameEN { get; set; }
        public string Applicant_FirstNameEN { get; set; }
        public string Applicant_CitizenCountry { get; set; }
        public string Municipality { get; set; }
        public string Zip { get; set; }
        public string IBAN { get; set; }

        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }

        // Found Person Info
        public long PersonId { get; set; }
        public string AMKA { get; set; }
        public string AFM { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string LastNameEN { get; set; }
        public string FirstNameEN { get; set; }
        public string CitizenCountry { get; set; }

        public string Applicant_FullName
        {
            get
            {
                if (Applicant_CitizenCountry != "ΕΛΛΑΔΑ")
                {
                    return $"{this.Applicant_LastNameEN} {this.Applicant_FirstNameEN}".Trim();
                }

                return $"{this.Applicant_LastName} {this.Applicant_FirstName}".Trim();
            }
        }

        public string FullName
        {
            get
            {
                if (CitizenCountry != "ΕΛΛΑΔΑ")
                {
                    return $"{this.LastNameEN} {this.FirstNameEN}".Trim();
                }

                return $"{this.LastName} {this.FirstName}".Trim();
            }
        }
        public bool IsEditableApplicationSearch { get; set; }
        public bool CanViewOnlyApplicationSearch { get; set; }
        public string DistrictId { get; set; }

    }
}
