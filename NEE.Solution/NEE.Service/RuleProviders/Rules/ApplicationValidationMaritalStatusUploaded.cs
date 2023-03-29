using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationMaritalStatusUploaded : ApplicationValidationRule
    {
        public ApplicationValidationMaritalStatusUploaded(Application application)
            : base(application, MaritalStatusUploaded)
        {
            this.RelatedRemark = new Remark(RemarkType.MaritalStatusUploaded,
                Name,
                Name,
                NEERemarkSeverity.Low,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = false;
            if (Application.HasMaritalStatusDocument && !Application.HasMaritalStatusDocumentDecision)
            {
                HasFailed = true;
            }
            else if (!Application.HasMaritalStatusDocument2 && Application.HasMaritalStatusDocumentDecision)
            {
                HasFailed = (bool)!Application.IsMaritalStatusDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.LowMedium;
                RelatedRemark.Message = Application.MaritalStatusDocumentRejectionReason;
            }
            else if (Application.HasMaritalStatusDocument2 && Application.HasMaritalStatusDocumentDecision)
            {
                HasFailed = (bool)!Application.IsMaritalStatusDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.MediumHigh;
                RelatedRemark.Message = Application.MaritalStatusDocumentRejectionReason2;
            }
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
