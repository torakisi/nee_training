namespace NEE.Database.Entities
{
    public class SUP_Faq
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SectionTitle { get; set; }
        public long Order { get; set; }
        public long SectionOrder { get; set; }
        public bool DisableQ { get; set; }
        public bool? IsForAdmin { get; set; }
    }
}
