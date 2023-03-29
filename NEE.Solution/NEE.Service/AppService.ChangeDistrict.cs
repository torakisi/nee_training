using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Service.Authorization;
using NEE.Service.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using NEE.Core.BO;

namespace NEE.Service
{
    partial class AppService
    {
        public async Task<ChangeDistrictResponse> ChangeDistrictAsync(ChangeDistrictRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.ChangeApplicationDistrict,
                ReferencedApplicationId = req.AppId,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            SetServiceContext(context);

            ChangeDistrictResponse response = new ChangeDistrictResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                await repository.Save(req);

            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, "Υπήρξε σφάλμα κατά την επεξεργασία του αιτήματος.", ex);
            }

            return response;
        }
    }

    public class ChangeDistrictRequest
    {
        public string AppId { get; set; }
        public int DistrictId { get; set; }
    }

    public class ChangeDistrictResponse : NEEServiceResponseBase
    {
        public ChangeDistrictResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }        
    }
}
