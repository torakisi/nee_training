using NEE.Core.Contracts.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database
{

    public class NEE_AppFile
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public string ApplicationId { get; set; }

        public string Filename { get; set; }
        public string FileType { get; set; }
        public string FileSize { get; set; }
        public string Comments { get; set; }
        public string Url { get; set; }
        public string UploadedFromIP { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string RejectedBy { get; set; }
        public string RejectionReason { get; set; }
        public bool? IsApproved { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual NEE_App NEE_App { get; set; }
    }
}