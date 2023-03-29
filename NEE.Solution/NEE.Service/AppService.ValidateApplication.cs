using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEE.Core.Rules;
using NEE.Service.RuleProviders;

namespace NEE.Service
{
    partial class AppService
    {
        public ValidateApplicationResponse ValidateApplication(ValidateApplicationRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.ValidateApplication,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            ValidateApplicationResponse response = new ValidateApplicationResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                var application = req.Application;

                RuleProvider ruleProvider = new ApplicationApprovalRuleProvider(application, this);
                                
                ruleProvider.Validate();

                if (!ruleProvider.IsValid)
                {
                    foreach (ApplicationValidationRule rule in ruleProvider.GetRules())
                    {
                        if (rule.HasFailed.HasValue && rule.HasFailed.Value)
                        {
                            response.NewRemarks.Add(rule.GetRelatedRemark());
                        }
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

    public class ValidateApplicationRequest : NEEServiceRequestBase
    {
        public Application Application { get; set; }

        public override List<string> IsValid()
        {
            return null;
        }
    }
    public class ValidateApplicationResponse : NEEServiceResponseBase
    {
        public ValidateApplicationResponse(IErrorLogger errorLogger = null, string userName = null)
            : base(errorLogger, userName)
        {
        }
        public List<Remark> NewRemarks = new List<Remark>();
    }
}
