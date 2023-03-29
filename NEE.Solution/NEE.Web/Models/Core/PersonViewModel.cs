using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using NEE.Core;
using NEE.Core.BO;

namespace NEE.Web.Models.Core
{
    public class PersonViewModel : ViewModelBase
    {
        public class AdditionalData
        {
            [Display(Name = "Οικογενειακή Κατάσταση")]
            public MaritalStatus? MaritalStatus { get; set; }
        }

        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }
        [Display(Name = "ΑΜΚΑ")]
        public string AMKA { get; set; }
        [Display(Name = "Επώνυμο")]
        public string LastName { get; set; }
        [Display(Name = "Όνομα")]
        public string FirstName { get; set; }
        [Display(Name = "Επώνυμο (Λατινικά)")]
        public string LastNameEN { get; set; }
        [Display(Name = "Όνομα (Λατινικά)")]
        public string FirstNameEN { get; set; }
        [Display(Name = "Πατρώνυμο")]
        public string FatherName { get; set; }
        [Display(Name = "Μητρώνυμο")]
        public string MotherName { get; set; }
        [Display(Name = "Πατρώνυμο (Λατινικά)")]
        public string FatherNameEN { get; set; }
        [Display(Name = "Μητρώνυμο (Λατινικά)")]
        public string MotherNameEN { get; set; }
        [Display(Name = "Φύλο")]
        public Gender Gender { get; set; }
        [Display(Name = "Ημ. Γέννησης")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        [Display(Name = "Χώρα")]
        public string CitizenCountry { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }

        public int ApplicationRevision { get; set; }
        public bool CanViewMember { get; set; }
        public bool CanEditMember { get; set; }
        public bool CanHandleRemark { get; set; }
        public MemberState MemberState { get; set; } = MemberState.Normal;
        public MemberRelationship? Relationship { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public bool UnknownMaritalStatus { get; set; }
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
        public List<RemarkViewModel> RelatedRemarks { get; set; }
    }
}