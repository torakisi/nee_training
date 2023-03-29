using NEE.Core.Contracts.Enumerations;
using NEE.Core.Validation;
using NEE.Web.Models.ApplicationViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace NEE.Web.Models.AdminApplicationViewModels
{
    public class FindViewModel
    {
        [Required]
        [Display(Name = "ΑΜΚΑ", Prompt = @"ΑΜΚΑ (11 ψηφία)"), NEEStringLength(11, 11)]
        [StringLength(11, ErrorMessage = "ΑΜΚΑ (11 ψηφία)")]
        public string Amka { get; set; }

        [Required]
        [Display(Name = "ΑΦΜ", Prompt = @"ΑΦΜ (9 ψηφία)"), NEEStringLength(9, 9)]
        [StringLength(9, ErrorMessage = "ΑΦΜ (9 ψηφία)")]
        public string Afm { get; set; }


        #region FindResults Sub-Class

        public class FindResults
        {
            [Display(Name = "ΑΜΚΑ")]
            public string AMKA { get; set; }

            [Display(Name = "ΑΦΜ")]
            public string AFM { get; set; }

            [Display(Name = "Επώνυμο")]
            public string LastName { get; set; }

            [Display(Name = "Όνομα")]
            public string FirstName { get; set; }

            [Display(Name = "Επώνυμο (Λατινικά)")]
            public string LastNameEN { get; set; }

            [Display(Name = "Όνομα (Λατινικά)")]
            public string FirstNameEN { get; set; }

            [Display(Name = "Φύλο")]
            public Gender? Gender { get; set; }

            [Display(Name = "Ημ. Γέννησης")]
            public DateTime? DOB { get; set; }

            public ApplicationManagementViewModel Applications { get; set; }

            public bool CanCreateNewApplication { get; set; }
        }

        #endregion

        public FindResults Results { get; set; }

        public string GetUrlForFind()
        {
            string originalUrl = HttpContext.Current.Request.Url.AbsolutePath;
            if (HttpContext.Current.Request.Url.Query.Length > 0)
                originalUrl = originalUrl.Replace(HttpContext.Current.Request.Url.Query, string.Empty);

            string urlString = originalUrl + "?" + CreateFindCriteriaQueryParameters();
            return HttpUtility.UrlEncode(urlString);
        }

        public string CreateFindCriteriaQueryParameters()
        {

            string ret = "";

            if (!string.IsNullOrEmpty(this.Amka))
            {
                ret = ret + "AMKA=" + HttpContext.Current.Server.HtmlEncode(this.Amka) + "&";
            }
            if (!string.IsNullOrEmpty(this.Afm))
            {
                ret = ret + "AFM=" + HttpContext.Current.Server.HtmlEncode(this.Afm) + "&";
            }
            ret = ret + "fromReturn=1&";

            if (!string.IsNullOrEmpty(ret))
            {
                ret.Remove(ret.Length - 1);
            }

            return ret;
        }
    }
}