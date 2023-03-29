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
    public class ApplicationValidationIbanUnsupported : ApplicationValidationRule
    {
        public ApplicationValidationIbanUnsupported(Application application)
            : base(application, UnsupportedBank)
        {
            this.RelatedRemark = new Remark(RemarkType.UnsupportedBank,
                Name,
                Name,
                NEERemarkSeverity.Medium,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = (Application.IbanValidationResult.HasValue
                && Application.IbanValidationResult.Value == IbanValidationServiceResult.UnsupportedBank);

            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
