using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Service.Authorization;
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
        public async Task<GetApplicationResponse> GetApplicationAsync(GetApplicationRequest req)
        {
            List<GetApplicationUse> onlyKKUses = new List<GetApplicationUse>()
            {
                GetApplicationUse.UndoSuspendApplication,
                GetApplicationUse.SuspendApplication,
            };

            List<GetApplicationUse> applicantUses = new List<GetApplicationUse>()
            {
                GetApplicationUse.CancelApplication,
                GetApplicationUse.DeleteApplication,
                GetApplicationUse.EditApplication,
                GetApplicationUse.ViewApplication,
                GetApplicationUse.PrintApplication,
                GetApplicationUse.AddMember,
                GetApplicationUse.ApplicationSubmitted,
                GetApplicationUse.PrintApproval,
                GetApplicationUse.FinalSubmitApplication,
                GetApplicationUse.RecallApplication,
                GetApplicationUse.PrintRecallApplication,
                GetApplicationUse.PrintReject,
                GetApplicationUse.SubmittedDetails
            };

            List<GetApplicationUse> applicationMemberUses = new List<GetApplicationUse>()
            {
                GetApplicationUse.GetMember,
                GetApplicationUse.SetPersonConsent,
                GetApplicationUse.SaveApplication,
                GetApplicationUse.ViewMember
            };

            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.GetApplication,
                ReferencedApplicationId = req.Id,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            GetApplicationResponse response = new GetApplicationResponse(_errorLogger, _currentUserContext.UserName);


            try
            {
                var application = await repository.Load(req.Id);

                if (application == null)
                    return GetApplicationResponse.NotFound(req.Id);

                if (req.Revision != 0 && application.Revision != req.Revision)
                {
                    return GetApplicationResponse.HasChanged(req.Id);
                }

                if (applicantUses.Contains(req.ForWhatUse))
                {
                    if (!ServiceAuthorization.AccessApplicationShouldBeApplicantAuthorized(_currentUserContext, application))
                        throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_MESSAGE);
                }

                if (applicationMemberUses.Contains(req.ForWhatUse))
                {
                    if (!ServiceAuthorization.AccessApplicationShouldBeMemberAuthorized(_currentUserContext, application))
                        throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_MESSAGE);
                }

                if (onlyKKUses.Contains(req.ForWhatUse))
                {
                    if (!ServiceAuthorization.AccessApplicationShouldBeMemberAuthorized(_currentUserContext, application))
                        throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_MESSAGE);
                }

                //var paymentsResp = await _gsPaymentService.GetPaymentsDataAsync(new GetPaymentsDataRequest()
                //{
                //    AFM = application.AFM,
                //    Id = application.Id
                //});

                //if (paymentsResp._IsSuccessful)
                //{
                //    application.PaymentsWebView = paymentsResp.PaymentsWebView;
                //}

                //var lastIbanDateChange = await repository.GetLatestIbanChange(application.Id);
                //application.LatestIbanChange = lastIbanDateChange;

                response.Application = application;
                response.Application.LoadFurtherChecks(_currentUserContext);
                response.Application.RemoteHostIP = req.RemoteHostIP;  // pass remote host ID for the IBAN Validation WS
                if (req.ForWhatUse == GetApplicationUse.EditApplication && application.State == AppState.Draft)
                    await CreateValidationRemarksForApplicationAsync(response.Application);								//δημιουργία remarks
            }
            catch (UnauthorizedAccessException ex)
            {
                response.AddError(ErrorCategory.UIDisplayed, ex.Message, ex);
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }
            return response;
        }
    }
    public class GetApplicationRequest
    {
        public string Id { get; set; }
        public int Revision { get; set; }
        public GetApplicationUse ForWhatUse { get; set; } = GetApplicationUse.Default;
        /// <summary>
        /// get remote host ip to pass to the Validate IBAN WS
        /// </summary>
        public string RemoteHostIP { get; set; }
    }

    public class GetApplicationResponse : NEEServiceResponseBase
    {
        public GetApplicationResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public static GetApplicationResponse NotFound(string id)
        {
            var res = new GetApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Η αίτηση με κωδικό {id} δεν υπάρχει.");
            return res;
        }

        public static GetApplicationResponse HasChanged(string id)
        {
            var res = new GetApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Η αίτηση με κωδικό {id} έχει υποστεί αλλαγές απο την τελευταία ενημέρωση της σελίδας. Παρακαλώ δοκιμάστε ξανά.");
            return res;
        }
        public Application Application { get; set; }
    }
}
