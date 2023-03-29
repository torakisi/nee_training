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

namespace NEE.Service
{
	public partial class AppService
	{
		/// <summary>
		/// Επιστρέφει για τα στοιχεία που δίνονται στο GetApplicationSetStateRequest (πχ ΑΜΚΑ), 
		/// την τρέχουσα κατάσταση του χρήστη ως προς τις αιτήσεις του.
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public async Task<GetApplicationOwnerResponse> GetApplicationOwner(GetApplicationOwnerRequest req)
		{
			ServiceContext context = new ServiceContext()
			{
				ServiceAction = ServiceAction.GetApplicationOwner,
				InitialRequest = JsonHelper.Serialize(req, false)
			};

			this.SetServiceContext(context);

			GetApplicationOwnerResponse response = new GetApplicationOwnerResponse(_errorLogger, _currentUserContext.UserName);

			try
			{
				var applicationOwner = await repository.LoadApplicationOwner(req.AFM, req.AMKA);

				Person owner = new Person();

				var ownerResp = await _personService.GetAmkaRegistryAsync(new PersonService.GetAmkaRegistryRequest()
				{
					AFM = req.AFM,
					AMKA = req.AMKA
				});

				if (!ownerResp._IsSuccessful)
				{
					response.AddErrors(ownerResp._Errors);

					return response;
				}

				owner = ownerResp.Person;

				applicationOwner.Owner = owner;
				response.ApplicationOwner = applicationOwner;
			}
			catch (Exception ex)
			{
				response.AddError(ErrorCategory.Unhandled, null, ex);
			}

			return response;
		}

	}

	public class GetApplicationOwnerRequest
	{
		public string AFM { get; set; }
		public string AMKA { get; set; }
		public bool ShouldCheckForMinedu { get; set; } = false;
	}
	public class GetApplicationOwnerResponse : NEEServiceResponseBase
	{
		public GetApplicationOwnerResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
		{
		}

		public ApplicationOwner ApplicationOwner { get; set; }

	}
}
