using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationDisabilityBenefitsExceeded : ApplicationValidationRule
    {
        public ApplicationValidationDisabilityBenefitsExceeded(Application application)
            : base(application, DisabilityBenefitsExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.DisabilityBenefitsExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.DisabilityBenefits > 387.9m;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
