using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationAADEFamilyIncomeExceeded : ApplicationValidationRule
    {
        public ApplicationValidationAADEFamilyIncomeExceeded(Application application)
            : base(application, AADEFamilyIncomeExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.AADEFamilyIncomeExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            var familyIncome = Application.Applicant.FamilyIncome;
            if (familyIncome == 0)
            {
                familyIncome = Application.Applicant.Income + Application.Spouse?.Income;
            }
            HasFailed = familyIncome > 8640;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
