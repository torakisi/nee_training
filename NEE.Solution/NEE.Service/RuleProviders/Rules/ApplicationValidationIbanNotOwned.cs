using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationIbanNotOwned : ApplicationValidationRule
    {
        public ApplicationValidationIbanNotOwned(Application application)
            : base(application, IbanNotOwned)
        {
            this.RelatedRemark = new Remark(RemarkType.IbanNotOwned,
                Name,
                Name,
                NEERemarkSeverity.Medium,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = (Application.IbanValidationResult.HasValue
                && Application.IbanValidationResult.Value == IbanValidationServiceResult.IncorrectCombination);

            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
