using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XServices.Iban;
using NEE.Core.BO;

namespace NEE.Service
{
    partial class AppService
    {
        public async Task<ValidateIBANResponse> ValidateIBAN(ValidateIBANRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.ValidateIBAN,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            ValidateIBANResponse response = new ValidateIBANResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                GetIbanValidateRequest ibanReq = new GetIbanValidateRequest
                {
                    IBAN = req.IBAN,
                    AFM = req.AFM
                    //---audit   
                    ,
                    auditProtocol = req.auditProtocol,
                    auditUnit = req.auditUnit,
                    auditUserId = req.auditUserId,
                    auditUserIp = req.auditUserIp
                };

                var application = req.Application;

                response.Result = IbanValidationServiceResult.Valid;
                application.IbanValidationResult = IbanValidationServiceResult.Valid;

                var ibanRes = await _xsIbanService.ValidateIbanAsync(ibanReq);   //<<    WS call!!!

                if (!ibanRes._IsSuccessful)
                {
                    response.Result = IbanValidationServiceResult.ServiceFailed;
                    application.IbanValidationResult = IbanValidationServiceResult.ServiceFailed;
                }

                if ((ibanRes.ErrorCode != null && ibanRes.ErrorCode.ErrorCode == 5))
                {
                    application.IbanValidationResult = IbanValidationServiceResult.UnsupportedBank;
                    response.Result = IbanValidationServiceResult.UnsupportedBank;
                }

                if ((ibanRes.CorrectCombination.HasValue && !ibanRes.CorrectCombination.Value)
                    || (ibanRes.UsageAllowed.HasValue && !ibanRes.UsageAllowed.Value))
                {
                    response.Result = IbanValidationServiceResult.IncorrectCombination;
                    application.IbanValidationResult = IbanValidationServiceResult.IncorrectCombination;
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }
    }

    public class ValidateIBANRequest
    {
        public Application Application { get; set; }
        public string AFM { get; set; }
        public string IBAN { get; set; }


        // ----   Those fields added for the WS GSISKED   ------------
        public string RequestId { get; set; }
        public string auditTransactionId { get; set; }
        public System.DateTime auditTransactionDate { get; set; }
        public string auditUnit { get; set; }
        public string auditProtocol { get; set; }
        public string auditUserId { get; set; }
        public string auditUserIp { get; set; }
    }

    public class ValidateIBANResponse : NEEServiceResponseBase
    {
        public ValidateIBANResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public static ValidateIBANResponse NotMatching(string iban, string afm)
        {
            var res = new ValidateIBANResponse();
            res.AddError(ErrorCategory.UIDisplayed, $"To ΙΒΑΝ: {iban} δεν αντιστοιχεί στο ΑΦΜ: {afm}");

            return res;
        }

        public static ValidateIBANResponse NotSupportedBank(string iban)
        {
            var res = new ValidateIBANResponse();
            res.AddError(ErrorCategory.UIDisplayed, $"To ΙΒΑΝ: {iban} δεν αντιστοιχεί σε υποστηριζόμενη τράπεζα");
            // res.SupportedBank = false;

            return res;
        }

        public IbanValidationServiceResult Result { get; set; }
    }

}
