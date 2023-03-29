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
        public async Task<CancelApplicationResponse> CancelApplicationAsync(CancelApplicationRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.CancelApplication,
                ReferencedApplicationId = req.Id,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            SetServiceContext(context);

            CancelApplicationResponse response = new CancelApplicationResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                var getAppResp = await GetApplicationAsync(new GetApplicationRequest()
                {
                    Id = req.Id,
                    ForWhatUse = GetApplicationUse.CancelApplication,
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
                if (_currentUserContext.IsNormalUser)
                {
                    if (_currentUserContext.IsReadOnlyUser)
                        throw new UnauthorizedAccessException();
                }
                else
                {
                    if (!UserIsApplicant(application) && !UserIsSpouse(application))
                        throw new UnauthorizedAccessException();
                }

                if (!application.CanCancel())
                    return CancelApplicationResponse.Invalid();

                if (!application.IsEditableApplication)
                    return CancelApplicationResponse.NotEditable(application.Id);

                bool wasApproved = (application.State == AppState.Approved);

                application.Cancel(_currentUserContext);
                
                await repository.Save(application, req.IsModelStateValid);

                if (req.ReturnApplication)
                    application = await repository.Load(application.Id);

                response.Application = application;
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, "Υπήρξε σφάλμα κατά την επεξεργασία του αιτήματος.", ex);
            }

            return response;
        }

        private bool UserIsApplicant(Application application) =>
            application.Applicant.AFM == UserInfo.AFM;
        private bool UserIsSpouse(Application application)
        {
            var spouse = application.Members
                .Where(m => m.Relationship == MemberRelationship.Spouse)
                .SingleOrDefault();
            return spouse != null && spouse.AFM == UserInfo.AFM;
        }
    }

    public class CancelApplicationRequest
    {
        public string Id { get; set; }
        public int Revision { get; set; }

        public bool ReturnApplication { get; set; } = false;
        /// <summary>
        /// get remote host ip to pass to the Validate IBAN WS
        /// </summary>
        public string RemoteHostIP { get; set; }
        public bool IsModelStateValid { get; set; }
    }

    public class CancelApplicationResponse : NEEServiceResponseBase
    {
        public CancelApplicationResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public Application Application { get; set; }

        public static CancelApplicationResponse NotFound(string id)
        {
            var res = new CancelApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.Not_Found, $"Δε βρέθηκε αίτηση με αριθμό {id}");
            return res;
        }

        public static CancelApplicationResponse NotEditable(string id)
        {
            var res = new CancelApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.Not_Found, $"Δεν έχετε δικαίωμα επεξεργασίας στην αίτηση με κωδικό: {id}");
            return res;
        }

        public static CancelApplicationResponse Invalid()
        {
            var res = new CancelApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.Error, $"Η αίτηση δε μπορεί να ακυρωθεί");
            return res;
        }
    }
}
