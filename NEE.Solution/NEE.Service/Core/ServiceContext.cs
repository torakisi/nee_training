using NEE.Core.Contracts.Enumerations;

namespace NEE.Service.Core
{
    public class ServiceContext
    {
        public ServiceAction? ServiceAction { get; set; } = null;
        public string ReferencedApplicationId { get; set; } = null;
        public string InitialRequest { get; set; } = null;
    }
}
