using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.BO
{
    public class Person
    {
        public string EntityId { get; set; }
        public int ApplicationRevision { get; set; }
        public int Revision { get; set; }
        public long? PersonId { get; set; }
        public string EDTONumber { get; set; }
        public string AFM { get; set; }
        public string AMKA { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; }

        [MaxLength(40)]
        public string LastNameBirth { get; set; }

        [MaxLength(40)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string LastNameEN { get; set; }

        [MaxLength(40)]
        public string FirstNameEN { get; set; }

        [MaxLength(40)]
        public string FatherName { get; set; }
        [MaxLength(40)]
        public string MotherName { get; set; }
        [MaxLength(40)]
        public string FatherNameEN { get; set; }
        [MaxLength(40)]
        public string MotherNameEN { get; set; }
        public Gender Gender { get; set; }
        public string CitizenCountry { get; set; }
        public string BirthCountry { get; set; }

        public DateTime? DOB { get; set; }
        public DateTime? DOD { get; set; }
        public DeathStatus? DeathStatus { get; set; }
        public MemberRelationship? Relationship { get; set; }
        public int TaxisReferenceYear { get; set; }

        #region other benefits
        public decimal? HousingBenefit { get; set; }
        public decimal? KAYBenefit { get; set; }
        public decimal? HousingAssistanceBenefit { get; set; }
        public decimal? BenefitForOmogeneis { get; set; }
        public decimal? DisabilityBenefits { get; set; }
        public decimal? A21Benefit { get; set; }
        #endregion

        #region contact info
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Zip { get; set; }
        public string PostalNumber { get; set; }
        public string Region { get; set; }
        public string RegionalUnit { get; set; }
        public string Municipality { get; set; }
        public string MunicipalUnit { get; set; }
        public string Commune { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        #endregion

        public decimal? Income { get; set; }
        public decimal? FamilyIncome { get; set; }
        public decimal? AssetsValue { get; set; }
        public decimal? VehiclesValue { get; set; }
        public decimal? PensionAmount { get; set; }
        public string PensionFromAlbania { get; set; }
        public decimal? PensionAmountAlbania { get; set; }
        public string Currency { get; set; }
        public DateTime? PensionStartDateAlbania { get; set; }
        public string PensionDocumentAlbaniaId { get; set; }
        public IdentificationNumberType? IdentificationNumberType { get; set; }
        private string identificationNumber { get; set; }
        public DateTime? IdentificationNumberEndDate { get; set; }
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
                    this.IdentificationNumberEndDate = null;
                    this.IdentificationNumberConfirmed = null;
                }
                else
                {
                    identificationNumber = value;
                    this.IdentificationNumberEndDate = null;
                    this.IdentificationNumberConfirmed = 0;
                }
            }
        }
        public int Age
        {
            get
            {
                if (DOD.HasValue)
                    return GetAge(new YearCounter(DOD.Value));

                return GetAge(YearCounter.UntilToday());
            }
        }
        public string GetFullNameEN()
        {
            return $"{LastNameEN} {FirstNameEN}";
        }
        public bool IsForeignCitizen
        {
            get
            {
                if (CitizenCountry == "ΕΛΛΑΔΑ")
                {
                    return false;
                }

                return true;
            }
        }
        public string DisplayedLastName
        {
            get
            {
                if (IsForeignCitizen)
                {
                    return LastNameEN;
                }
                else
                {
                    return LastName;
                }
            }
        }
        public bool IsDead => (this.DeathStatus != null);
        public MemberState MemberState { get; set; } = MemberState.Normal;

        public DateTime CreatedAt { get; set; }

        [MaxLength(256)]
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }

        [MaxLength(256)]
        public string ModifiedBy { get; set; }

        

        #region edto info
        public string PermitNumber { get; set; }
        public DateTime? AdministrationDate { get; set; }
        #endregion

        #region ergani info
        public bool IsEmployed { get; set; }
        #endregion

        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? RestoredAt { get; set; }
        public string RestoredBy { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public bool UnknownMaritalStatus { get; set; }
        public bool CanViewMember { get; set; }
        public bool CanEditMember { get; set; }
        public List<Remark> RelatedRemarks { get; set; } = new List<Remark>();


        public Person GetNewEntityId(INEEAppIdCreator idGenerator)
        {
            this.EntityId = "p-" + idGenerator.CreateIdFromDateTime(DateTime.Now);

            return this;
        }
        private int GetAge(YearCounter yearCounter)
        {
            if (DOB == null)
                return 0;

            return yearCounter.YearsFrom((DateTime)DOB);
        }

        public bool HasSpouse
        {
            get
            {
                return MaritalStatus == Contracts.Enumerations.MaritalStatus.Married ||
                    MaritalStatus == Contracts.Enumerations.MaritalStatus.CivilUnion;
            }
        }
    }
}
