using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NEE.Core.BO
{
    public class AppFile
    {
        public DocumentCategory DocumentType { get; set; }
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

    }
}
