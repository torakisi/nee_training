namespace NEE.Web.Models
{
    public class AsyncActionResult
    {
        public string PartialViewResult { get; set; }
        public bool ResultSuccess { get; set; } = false;
        public string ResultMessage { get; set; }
    }
}