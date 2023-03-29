using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationPensionExceeded : ApplicationValidationRule
    {
        public ApplicationValidationPensionExceeded(Application application)
            : base(application, PensionExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.PensionExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.PensionAmount > 387.9m;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
