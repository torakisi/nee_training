using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationBenefitForOmogeneisExceeded : ApplicationValidationRule
    {
        public ApplicationValidationBenefitForOmogeneisExceeded(Application application)
            : base(application, BenefitForOmogeneisExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.BenefitForOmogeneisExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.BenefitForOmogeneis > 387.9m;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
