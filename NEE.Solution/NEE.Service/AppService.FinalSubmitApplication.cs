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
using NEE.Core.Validation;

namespace NEE.Service
{
    public partial class AppService
    {
        public async Task<FinalSubmitApplicationResponse> FinalSubmitApplicationAsync(FinalSubmitApplicationRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.FinalSubmitApplication,
                ReferencedApplicationId = req.Id,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            SetServiceContext(context);

            FinalSubmitApplicationResponse resp = new FinalSubmitApplicationResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                var getAppResp = await GetApplicationAsync(new GetApplicationRequest()
                {
                    Id = req.Id,
                    ForWhatUse = GetApplicationUse.FinalSubmitApplication
                // pass the Remote Host IP for the IBAN Validation WS 
                ,
                    RemoteHostIP = req.RemoteHostIP
                });

                if (!getAppResp._IsSuccessful)
                {
                    resp.AddErrors(getAppResp.UIDisplayedErrors);

                    return resp;
                }

                var application = getAppResp.Application;

                if (!ServiceAuthorization.AccessApplicationShouldBeApplicantAuthorized(_currentUserContext, application))
                    throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_MESSAGE);

                application.LoadFurtherChecks(_currentUserContext);
                if (!application.IsEditableApplication)
                    return FinalSubmitApplicationResponse.NotEditable(application.Id);

                if (req.FEKDocumentRequired && !application.HasFEKDocument)
                {
                    return FinalSubmitApplicationResponse.DocumentsMissing("ΦΕΚ Πολιτογράφησης / Απόφαση");
                }

                if (req.MaritalStatusDocumentRequired && !application.HasMaritalStatusDocument)
                {
                    return FinalSubmitApplicationResponse.DocumentsMissing("Βεβαίωση οικογενειακής μερίδας από Αλβανία");
                }

                if (req.PensionAlbaniaDocumentRequired && !application.HasPensionAlbaniaDocument)
                {
                    return FinalSubmitApplicationResponse.DocumentsMissing("Βεβαίωση συνταξιοδότησης από Αλβανία");
                }

                if (req.SpouseAlbaniaDocumentRequired && !application.HasSpousePensionDocument)
                {
                    return FinalSubmitApplicationResponse.DocumentsMissing("Βεβαίωση συνταξιοδότησης συζύγου από Αλβανία");
                }

                if (application.IsAdminUser && req.FEKDocumentRequired && !application.HasFEKDocumentDecision)
                {
                    return FinalSubmitApplicationResponse.ApprovalMissing("ΦΕΚ Πολιτογράφησης / Απόφαση");
                }

                if (application.IsAdminUser && req.MaritalStatusDocumentRequired && !application.HasMaritalStatusDocumentDecision)
                {
                    return FinalSubmitApplicationResponse.ApprovalMissing("Βεβαίωση οικογενειακής μερίδας από Αλβανία");
                }

                if (application.IsAdminUser && req.PensionAlbaniaDocumentRequired && !application.HasPensionAlbaniaDocumentDecision)
                {
                    return FinalSubmitApplicationResponse.ApprovalMissing("Βεβαίωση συνταξιοδότησης από Αλβανία");
                }

                if (application.IsAdminUser && req.SpouseAlbaniaDocumentRequired && !application.HasSpousePensionDocumentDecision)
                {
                    return FinalSubmitApplicationResponse.ApprovalMissing("Βεβαίωση συνταξιοδότησης συζύγου από Αλβανία");
                }

                #region approval flow

                //A. πολίτης, πρώτη υποβολή -> status submitted
                //B. κκ, πρώτη υποβολή -> status submitted
                if (application.State == AppState.Draft)
                {
                    #region get person data from web services
                    var respProvideApplicantData = await ProvideApplicantData(application, application.Applicant);
                    if (!respProvideApplicantData._IsSuccessful)
                    {
                        resp.AddErrors(respProvideApplicantData._Errors);
                        resp.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);
                        return resp;
                    }
                    if (application.Applicant.HasSpouse && application.SpouseAMKA != null && application.SpouseAFM != null)
                    {
                        var respProvideSpouseData = await ProvideSpouseData(application, application.Spouse);
                        if (!respProvideSpouseData._IsSuccessful)
                        {
                            resp.AddErrors(respProvideSpouseData._Errors);
                            resp.AddError(ErrorCategory.UIDisplayedGenericServiceFailure, ServiceErrorMessages.GenericErrorWithExternalService);
                            return resp;
                        }
                    }
                    #endregion

                    application.RemoteHostIP = req.RemoteHostIP;  // pass remote host ID for the IBAN Validation WS
                    await CreateValidationRemarksForApplicationAsync(application);

                    if (!application.CanBeSubmitted)
                    {
                        return FinalSubmitApplicationResponse.CantBeSubmitted();
                    }

                    application.SetState(AppState.Submitted, _currentUserContext);
                    application.SubmittedAt = DateTime.Now;
                    application.SubmittedBy = _currentUserContext.UserName;

                    application.CalculateBenefitAmount();
                    application.ApproveOrReject(_currentUserContext);
                }
                //οπεκά, πρώτος έλεγχος εγγράφων
                else if (application.State == AppState.PendingDocumentsApproval && application.IsAdminUser) //IsOpekaUser
                {
                    application.RemoteHostIP = req.RemoteHostIP;  // pass remote host ID for the IBAN Validation WS
                    await CreateValidationRemarksForApplicationAsync(application);
                    application.ApproveOrReject(_currentUserContext);
                }
                //κκ, δεύτερος έλεγχος εγγράφων
                else if (application.State == AppState.RejectedDocuments && application.IsAdminUser)
                {
                    application.RemoteHostIP = req.RemoteHostIP;  // pass remote host ID for the IBAN Validation WS
                    await CreateValidationRemarksForApplicationAsync(application);
                    application.ApproveOrReject(_currentUserContext);
                }

                #endregion

                await repository.Save(application, req.IsModelStateValid);
                application = await repository.Load(application.Id, application.Revision);
                return FinalSubmitApplicationResponse.Saved(application);
            }
            catch (Exception ex)
            {
                resp.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return resp;
        }

    }

    public class FinalSubmitApplicationRequest
    {
        public string Id { get; set; }
        public int Revision { get; set; }
        /// <summary>
        /// get remote host ip to pass to the Validate IBAN WS
        /// </summary>
        public string RemoteHostIP { get; set; }
        public bool FEKDocumentRequired { get; set; }
        public bool MaritalStatusDocumentRequired { get; set; }
        public bool PensionAlbaniaDocumentRequired { get; set; }
        public bool SpouseAlbaniaDocumentRequired { get; set; }
        public bool IsModelStateValid { get; set; }
    }

    public class FinalSubmitApplicationResponse : NEEServiceResponseBase
    {
        public FinalSubmitApplicationResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public Application Application { get; set; }

        public static FinalSubmitApplicationResponse NotFound(string id)
        {
            var res = new FinalSubmitApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Δε βρέθηκε αίτηση με κωδικό {id}");
            return res;
        }

        public static FinalSubmitApplicationResponse NotAllowed()
        {
            var res = new FinalSubmitApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Δεν είναι δυνατή η τροποποίηση της αίτησης");
            return res;
        }

        public static FinalSubmitApplicationResponse CantBeSubmitted()
        {
            var res = new FinalSubmitApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Η αίτηση δεν μπορεί να οριστικοποιηθεί - ανατρέξτε στις 'Επισημάνσεις Αίτησης' για περισσότερες πληροφορίες.");
            return res;
        }

        public static FinalSubmitApplicationResponse NotEditable(string id)
        {
            var res = new FinalSubmitApplicationResponse { _IsSuccessful = false };
            res.AddError(ErrorCategory.UIDisplayed, $"Δεν έχετε δικαίωμα επεξεργασίας στην αίτηση με κωδικό: {id}");
            return res;
        }
        public static FinalSubmitApplicationResponse DocumentsMissing(string docName)
        {
            var res = new FinalSubmitApplicationResponse();
            res.AddError(ErrorCategory.UIDisplayed, $"Το έγγραφο {docName} είναι υποχρεωτικό.");
            return res;
        }

        public static FinalSubmitApplicationResponse ApprovalMissing(string docName)
        {
            var res = new FinalSubmitApplicationResponse();
            res.AddError(ErrorCategory.UIDisplayed, $"Πρέπει να πατήσετε αποδοχή/απόρριψη για το έγγραφο {docName}.");
            return res;
        }

        public static FinalSubmitApplicationResponse Saved(Application application)
        {
            var res = new FinalSubmitApplicationResponse
            {
                _IsSuccessful = true
            };

            res.Application = application;

            return res;
        }
    }
}
