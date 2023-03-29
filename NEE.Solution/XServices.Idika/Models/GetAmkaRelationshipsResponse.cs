using NEE.Core.Contracts;

namespace XServices.Idika
{
    public class GetAmkaRelationshipsResponse : XServiceResponseBase
    {
        public AmkaRelationhipInfo[] AmkaRelationships { get; set; }


        public enum AmkaRelationship
        {
            Spouse,
            Parent,
            Child,
            Grandchild,
            BrotherOrSister,
            Other,
            Unknown
        }
        public class AmkaRelationhipInfo
        {
            public string PrimaryAMKA { get; set; }
            public string RelatedAMKA { get; set; }
            public AmkaRelationship Relationship { get; set; }
        }
    }
}
