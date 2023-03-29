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
		public async Task<SearchApplicationsResponse> SearchApplicationAsync(SearchApplicationsRequest req)
		{
			ServiceContext context = new ServiceContext()
			{
				ServiceAction = ServiceAction.SearchApplication,
				ReferencedApplicationId = req.Id,
				InitialRequest = JsonHelper.Serialize(req, false)
			};

			this.SetServiceContext(context);

			SearchApplicationsResponse response = new SearchApplicationsResponse(_errorLogger, _currentUserContext.UserName);

			try
			{
				var searchResult = await repository.Search(req);
				if (searchResult == null)
					response.AddError(ErrorCategory.Not_Found, $"Δε βρέθηκαν αποτελέσματα");
				else
				{
					response = searchResult;

					if (response.ApplicationSearchResults.Count > 0)
                    {
                        var userDistrict = await GetOpekaDistrict();
                        response.CheckUserActions(_currentUserContext.IsAFMUser, _currentUserContext.IsKKUser, _currentUserContext.IsReadOnlyUser, userDistrict);

					}
				}
			}
			catch (Exception ex)
			{
				response.AddError(ErrorCategory.Unhandled, null, ex);
			}

			return response;
		}
	}

	public class SearchApplicationsRequest : ApplicationIdentityRequest
	{
		public string AMKA { get; set; }            // =
		public string AFM { get; set; }             // =
		public string LastName { get; set; }        // value%
		public string FirstName { get; set; }       // value%
		public string City { get; set; }            // %value%
		public string Zip { get; set; }             // =
		public string IBAN { get; set; }
		public string Email { get; set; }
		public string MobilePhone { get; set; }
		public string HomePhone { get; set; }
		public bool SearchInAppPerson { get; set; }
		public int StateId { get; set; }
		public int DistrictId { get; set; }
		public bool? SubmittedByKK { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }

		public static int MAX_Take = 100;
		public static int DEF_Take = 10;

		public int Take_IfDecisiveCrtiteria { get; set; }

		public static int MAX_Take_IfDecisiveCrtiteria = 1000;
		public static int DEF_Take_IfDecisiveCrtiteria = MAX_Take_IfDecisiveCrtiteria;
	}


	public class SearchApplicationsResponse : NEEServiceResponseBase
	{
		public SearchApplicationsResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
		{
		}

		public List<SearchApplication> ApplicationSearchResults { get; set; }

		public void CheckUserActions(bool isAFMUser, bool isKKUser, bool isReadOnlyUser, int? userDistrict)
		{
			//var userDistrictId = "";

   //         if (userDistrict != null)
			//	userDistrictId = userDistrict.ToString();
				
            foreach (SearchApplication item in ApplicationSearchResults)
			{
				item.IsEditableApplicationSearch = isKKUser && !isReadOnlyUser && item.State != AppState.Canceled && item.State != AppState.Approved && item.State != AppState.Rejected;
				item.CanViewOnlyApplicationSearch = item.State == AppState.Canceled || item.State == AppState.Approved || item.State == AppState.Rejected;
			}
		}

		public int Skip { get; set; }
		public int Take { get; set; }
		public int Total { get; set; }
	}
}
