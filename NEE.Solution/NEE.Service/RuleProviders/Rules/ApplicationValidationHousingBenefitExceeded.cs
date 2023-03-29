using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationHousingBenefitExceeded : ApplicationValidationRule
    {
        public ApplicationValidationHousingBenefitExceeded(Application application)
            : base(application, HousingBenefitExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.HousingBenefitExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.HousingBenefit > 387.9m;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
