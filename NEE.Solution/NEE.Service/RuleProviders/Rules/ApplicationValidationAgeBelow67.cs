using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationAgeBelow67 : ApplicationValidationRule
    {
        public ApplicationValidationAgeBelow67(Application application)
            : base(application, AgeBelow67)
        {
            this.RelatedRemark = new Remark(RemarkType.AgeBelow67,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.Age < 67;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
