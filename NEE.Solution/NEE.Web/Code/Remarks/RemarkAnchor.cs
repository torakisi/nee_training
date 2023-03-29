namespace NEE.Web.Code.Remarks
{
    public class RemarkAnchor
    {
        public string Anchor { get; set; }
        public static RemarkAnchor AddressInfo()
        {
            return new RemarkAnchor()
            {
                Anchor = AvailableRemarkAnchors.AddressInfo
            };
        }
        public static RemarkAnchor MemberSocialInfoAnchor()
        {
            return new RemarkAnchor()
            {
                Anchor = AvailableRemarkAnchors.MemberSocialInfo
            };
        }
    }
    public class AvailableRemarkAnchors
    {
        public static string AddressInfo = "addressInfo";
        public static string MemberSocialInfo = "memberSocialInfo";
    }
}