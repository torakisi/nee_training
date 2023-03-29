using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationAADERealEstateExceeded : ApplicationValidationRule
    {
        public ApplicationValidationAADERealEstateExceeded(Application application)
            : base(application, AADERealEstateExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.AADERealEstateExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.AssetsValue > 90000;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
