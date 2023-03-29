using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationHousingAssistanceBenefitExceeded : ApplicationValidationRule
    {
        public ApplicationValidationHousingAssistanceBenefitExceeded(Application application)
            : base(application, HousingAssistanceBenefitExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.HousingAssistanceBenefitExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.HousingAssistanceBenefit > 387.9m;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
