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
    public class ApplicationValidationIbanNotFound : ApplicationValidationRule
    {
        public ApplicationValidationIbanNotFound(Application application)
            : base(application, IbanNotFound)
        {
            this.RelatedRemark = new Remark(RemarkType.IbanNotFound,
                Name,
                Name,
                NEERemarkSeverity.Medium,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = (string.IsNullOrEmpty(Application.IBAN));
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}