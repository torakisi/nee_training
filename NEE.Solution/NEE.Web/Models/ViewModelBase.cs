namespace NEE.Web.Models
{
    public class ViewModelBase
    {
        public bool IsSuccesfull { get; set; } = false;
        public string ReturnUrl { get; set; }
        public bool IsNormalUser { get; set; }
    }
}