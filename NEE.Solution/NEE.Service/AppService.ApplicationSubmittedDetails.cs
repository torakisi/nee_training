using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Service.Authorization;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEE.Core.BO;

namespace NEE.Service
{
    partial class AppService
    {
        public async Task<ApplicationSubmittedDetailsResponse> ApplicationSubmittedDetailsAsync(ApplicationSubmittedDetailsRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.SubmittedDetails,
                ReferencedApplicationId = req.Id,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            ApplicationSubmittedDetailsResponse response = new ApplicationSubmittedDetailsResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                var getAppResp = await this.GetApplicationAsync(new GetApplicationRequest()
                {
                    Id = req.Id,
                    ForWhatUse = GetApplicationUse.SubmittedDetails
                // pass the Remote Host IP for the IBAN Validation WS 
                ,
                    RemoteHostIP = req.RemoteHostIP
                });

                if (!getAppResp._IsSuccessful)
                {
                    response.AddErrors(getAppResp.UIDisplayedErrors);

                    return response;
                }

                var application = getAppResp.Application;

                if (!ServiceAuthorization.AccessApplicationShouldBeApplicantAuthorized(_currentUserContext, application))
                    throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_MESSAGE);


                application.LoadFurtherChecks(_currentUserContext);


                if (!application.CanViewApplicationSubmittedDetails)
                    return ApplicationSubmittedDetailsResponse.NoViewApplicationSubmitted();


                response.Application = application;
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, "Υπήρξε σφάλμα κατά την επεξεργασία του αιτήματος.", ex);
            }

            return response;
        }
    }

    public class ApplicationSubmittedDetailsRequest
    {
        public string Id { get; set; }
        public int Revision { get; set; }
        public string ChangeReason { get; set; }
        public GetApplicationUse ForWhatUse { get; set; }
        /// <summary>
        /// get remote host ip to pass to the Validate IBAN WS
        /// </summary>
        public string RemoteHostIP { get; set; }
    }

    public class ApplicationSubmittedDetailsResponse : NEEServiceResponseBase
    {
        public ApplicationSubmittedDetailsResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public Application Application { get; set; }

        public static ApplicationSubmittedDetailsResponse NotFound(string id)
        {
            var res = new ApplicationSubmittedDetailsResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Δε βρέθηκε αίτηση με αριθμό {id}");
            return res;
        }



        public static ApplicationSubmittedDetailsResponse NoViewApplicationSubmitted()
        {
            var res = new ApplicationSubmittedDetailsResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Δεν υπάρχει εξουσιοδότηση για την συγκεκριμένη ενέργεια");
            return res;
        }


    }
}
