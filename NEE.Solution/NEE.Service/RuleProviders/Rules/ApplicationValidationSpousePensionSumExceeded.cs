using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationSpousePensionSumExceeded : ApplicationValidationRule
    {
        public ApplicationValidationSpousePensionSumExceeded(Application application)
            : base(application, SpousePensionSumExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.SpousePensionSumExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            decimal? normalizedAmount = Application.Spouse.PensionAmountAlbania;
            if (Application.Spouse.Currency == "LEK")
            {
                normalizedAmount = Application.Spouse.PensionAmountAlbania / 114.2275m;
            }
            HasFailed = Application.Spouse.PensionAmount + normalizedAmount > 387.9m;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
