using NEE.Core.Contracts;

namespace XServices.Idika
{
    public class GetAmkaRegistryInfoRequest : XServiceRequestBase
    {
        public string AMKA { get; set; }
        public string AFM { get; set; }
    }
}
