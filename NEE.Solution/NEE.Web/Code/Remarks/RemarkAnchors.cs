using NEE.Core.Contracts.Enumerations;
using NEE.Web.Code.Remarks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEE.Code.Remarks
{
    public class RemarkAnchors
    {
        public static Dictionary<RemarkType, RemarkAnchor> RemarkAnchorCollection = new Dictionary<RemarkType, RemarkAnchor>()
        {
            { RemarkType.MaritalStatusNotFound, RemarkAnchor.MemberSocialInfoAnchor() }
        };
    }
}