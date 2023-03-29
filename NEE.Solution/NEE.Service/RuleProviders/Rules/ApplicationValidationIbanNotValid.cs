using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationIbanNotValid : ApplicationValidationRule
    {
        public ApplicationValidationIbanNotValid(Application application)
            : base(application, IbanNotValid)
        {
            this.RelatedRemark = new Remark(RemarkType.IbanNotValid,
                Name,
                Name,
                NEERemarkSeverity.Medium,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = (!string.IsNullOrEmpty(Application.IBAN) && !Iban.IsValid(Application.IBAN));
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
