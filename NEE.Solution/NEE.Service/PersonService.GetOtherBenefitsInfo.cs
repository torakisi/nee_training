using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XServices.Idika;
using NEE.Core;

namespace NEE.Service
{
	partial class PersonService
	{
		public async Task<GetOtherBenefitsInfoResponse> GetOtherBenefitsAsync(GetOtherBenefitsInfoRequest req)
		{
            GetOtherBenefitsInfoResponse resp = new GetOtherBenefitsInfoResponse();

			try
			{
                var xsRes = await _xsIdikaService.GetOtherBenefitInfoAsync(req);
                resp = xsRes;
                if (!xsRes._IsSuccessful)
					resp.AddErrors(xsRes._Errors);
			}
			catch (Exception ex)
			{

				resp.AddError(ErrorCategory.Unhandled, null, ex);
			}

			return resp;
		}
    }
}
