using NEE.Core.Contracts.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database
{
    public class NEE_AppPerson : IProvideCreatedAndModified
    {
        [Key, Column(Order = 0)]
        [Required]
        public string Id { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        public long PersonId { get; set; }

        public string AFM { get; set; }
        public string AMKA { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOD { get; set; }
        public DeathStatus? DeathStatus { get; set; }
        public MemberState MemberState { get; set; }

        public MemberRelationship? Relationship { get; set; }
        public string LastName { get; set; }
        public string LastNameBirth { get; set; }
        public string FirstName { get; set; }
        public string LastNameEN { get; set; }
        public string FirstNameEN { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string FatherNameEN { get; set; }
        public string MotherNameEN { get; set; }
        public Gender Gender { get; set; }
        public string CitizenCountry { get; set; }
        public string BirthCountry { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string EntityId { get; set; }
        public int Revision { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string PostalNumber { get; set; }
        public string Region { get; set; }
        public string RegionalUnit { get; set; }
        public string Municipality { get; set; }
        public string MunicipalUnit { get; set; }
        public string Commune { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? RestoredAt { get; set; }
        public string RestoredBy { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public bool UnknownMaritalStatus { get; set; }
        public string PermitNumber { get; set; }
        public DateTime? AdministrationDate { get; set; }
        public bool IsEmployed { get; set; }
        public decimal? Income { get; set; }
        public decimal? FamilyIncome { get; set; }
        public decimal? AssetsValue { get; set; }
        public decimal? VehiclesValue { get; set; }
        public decimal? PensionAmount { get; set; }
        public bool PensionFromAlbania { get; set; }
        public decimal? PensionAmountAlbania { get; set; }
        public string Currency { get; set; }
        public DateTime? PensionStartDateAlbania { get; set; }
        public int TaxisReferenceYear { get; set; }
        public decimal? DisabilityBenefits { get; set; }
        public decimal? BenefitForOmogeneis { get; set; }
        public decimal? A21Benefit { get; set; }
        public decimal? HousingBenefit { get; set; }
        public decimal? KAYBenefit { get; set; }
        public decimal? HousingAssistanceBenefit { get; set; }
        public string PensionDocumentAlbaniaId { get; set; }
        public IdentificationNumberType? IdentificationType { get; set; }
        private string identificationNumber { get; set; }
        public DateTime? IdentificationEndDate { get; set; }
        public int? IdentificationNumberConfirmed { get; set; }
        public string IdentificationNumber
        {
            get
            {
                return identificationNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    identificationNumber = value;
                    this.IdentificationEndDate = null;
                    this.IdentificationNumberConfirmed = null;
                }
                else
                {
                    identificationNumber = value;
                    this.IdentificationEndDate = null;
                    this.IdentificationNumberConfirmed = 0;
                }
            }
        }
    }
}
