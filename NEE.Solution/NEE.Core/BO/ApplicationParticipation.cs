using NEE.Core.Contracts.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace NEE.Core.BO
{
    public class ApplicationParticipation
    {
        public ApplicationParticipation()
        {
        }

        public ApplicationParticipation(string applicantAfm, AppState state)
        {
            ApplicantAFM = applicantAfm;
            State = state;
        }

        public long PersonId { get; set; }
        public string ApplicantAFM { get; set; }
        public string ApplicantFirstName { get; set; }
        public string ApplicantLastName { get; set; }
        public string ApplicantFirstNameEN { get; set; }
        public string ApplicantLastNameEN { get; set; }
        public string ApplicantCitizenCountry { get; set; }

        public string GetFullName
        {
            get
            {
                if (ApplicantCitizenCountry != "ΕΛΛΑΔΑ")
                {
                    return $"{ApplicantLastNameEN} {ApplicantFirstNameEN}";
                }

                return $"{ApplicantFirstName} {ApplicantLastName}";
            }
        }
        public AppState? State { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime? RecalledAt { get; set; }

        public DateTime? PayFrom { get; set; }
        public DateTime? PayTo { get; set; }

        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ApplicationId { get; set; }

        public bool IsApplicationCreatedByKK { get; set; }

        public string ModifiedBy { get; set; }
        public bool IsModifiedByKK { get; set; }
        public bool IsApplicationModifiedByKK => IsModifiedByKK;

        public DateTime? InitialPayTo { get; set; }
        public DateTime? InitialPeriodTo { get; set; }
        public AppRenewalStatus? RenewalStatus { get; set; }
        public MemberRelationship? Relationship { get; set; }

        public bool IsEmailValid(string emailAddress)
        {
            try
            {
                return new EmailAddressAttribute().IsValid(emailAddress);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
