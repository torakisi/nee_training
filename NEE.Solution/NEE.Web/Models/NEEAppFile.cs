using System;
using System.ComponentModel.DataAnnotations;

namespace NEE.Web.Models
{
    public class NEEAppFile
    {
        public string Id { get; set; }
        public string ApplicationId { get; set; }

        [Display(Name = "Όνομα Αρχείου")]
        public string Filename { get; set; }
        [Display(Name = "Τύπος Αρχείου")]
        public string FileType { get; set; }
        [Display(Name = "Μέγεθος Αρχείου")]
        public string FileSize { get; set; }
        [Display(Name = "Σχόλια σχετικά με το Αρχείο")]
        public string Comments { get; set; }
        [Display(Name = "Url")]
        public string Url { get; set; }
        [Display(Name = "Από IP")]
        public string UploadedFromIP { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
    }
}