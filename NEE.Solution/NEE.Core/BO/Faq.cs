using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.BO
{
    public class Faq
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SectionTitle { get; set; }
        public long Order { get; set; }
        public long SectionOrder { get; set; }
        public bool DisableQ { get; set; }
        public bool? IsForAdmin { get; set; }
        public bool FAQForAdmin
        {
            get
            {
                return IsForAdmin == null || (IsForAdmin.HasValue && IsForAdmin.Value);
            }
        }

        public bool FAQForPublic
        {
            get
            {
                return IsForAdmin == null || (IsForAdmin.HasValue && !IsForAdmin.Value);
            }
        }
    }
}

