using System.ComponentModel.DataAnnotations;

namespace NEE.Web.Models
{
    public class LoginAMKAViewModel
    {
        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "ΑΜΚΑ", Prompt = @"ΑΜΚΑ (11 ψηφία)")] //, KOTRequired, KOTStringLength(11, 11), AMKA]
        [Required]
        public string AMKA { get; set; }
    }
}