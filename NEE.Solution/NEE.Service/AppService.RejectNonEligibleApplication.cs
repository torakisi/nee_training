using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Application = NEE.Core.BO.Application;

namespace NEE.Service
{
    public partial class AppService
	{
		public async Task<RejectNonEligibleApplicationResponse> RejectNonEligibleApplication(RejectNonEligibleApplicationRequest req)
		{
			ServiceContext context = new ServiceContext()
			{
				ServiceAction = ServiceAction.RejectNonEligibleApplication,
				ReferencedApplicationId = req.Id,
				InitialRequest = JsonHelper.Serialize(req, false)
			};

			this.SetServiceContext(context);

			RejectNonEligibleApplicationResponse response = new RejectNonEligibleApplicationResponse(_errorLogger, _currentUserContext.UserName);

			try
			{
				//temp fix for Georgia
				if (req.Application.AFM != "142818104" && req.Application.AFM != "068933130")
					req.Application.RejectNonEligibleApplication(_currentUserContext);
                await repository.Save(req.Application, true);
            }
			catch (Exception ex)
			{
				response.AddError(ErrorCategory.Unhandled, null, ex);
			}

			return response;
		}
	}

	public class RejectNonEligibleApplicationRequest : ApplicationIdentityRequest
	{		
		public Application Application { get; set; }
	}


	public class RejectNonEligibleApplicationResponse : NEEServiceResponseBase
	{
		public RejectNonEligibleApplicationResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
		{
		}

	}
}
