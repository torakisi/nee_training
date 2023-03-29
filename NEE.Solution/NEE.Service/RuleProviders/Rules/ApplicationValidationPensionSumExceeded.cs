using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationPensionSumExceeded : ApplicationValidationRule
    {
        public ApplicationValidationPensionSumExceeded(Application application)
            : base(application, PensionSumExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.PensionSumExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            decimal? normalizedAmount = Application.Applicant.PensionAmountAlbania;
            if (Application.Applicant.Currency == "LEK")
            {
                normalizedAmount = Application.Applicant.PensionAmountAlbania / 114.2275m;
            }
            HasFailed = Application.Applicant.PensionAmount + normalizedAmount > 387.9m;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
