using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Web.Code.Remarks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NEE.Code.Remarks;

namespace NEE.Web.Models.Core
{
    public class RemarkViewModel : ViewModelBase
    {
        public RemarkType RemarkCode { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int Severity { get; set; }
        public string Message { get; set; }
        public string RelatedAMKA { get; set; }
        public string RelatedAFM { get; set; }
        public bool? Released { get; set; }
        public string ReleaseText { get; set; }
        public string ReleasedBy { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public RemarkSelection? ReleaseSelection { get; set; }
        public string ReleaseTextDescription { get; set; }
        public bool ApprovedByKK { get; set; }
        public string FullNameDescr { get; set; }
        public bool ReferToMember { get; set; }
        public string ReferencedAnchor
        {
            get
            {
                if (this.Severity == (int) NEERemarkSeverity.Medium)
                {
                    if (RemarkAnchors.RemarkAnchorCollection.ContainsKey(this.RemarkCode))
                    {
                        return RemarkAnchors.RemarkAnchorCollection[this.RemarkCode].Anchor;
                    }
                }

                return null;
            }
        }

        public bool ReasonForNoApproval { get; set; }

        public bool ReasonForNoSubmitted { get; set; }
    }
}