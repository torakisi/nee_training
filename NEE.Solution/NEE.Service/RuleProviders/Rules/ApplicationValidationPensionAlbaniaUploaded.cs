using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationPensionAlbaniaUploaded : ApplicationValidationRule
    {
        public ApplicationValidationPensionAlbaniaUploaded(Application application)
            : base(application, PensionAlbaniaUploaded)
        {
            this.RelatedRemark = new Remark(RemarkType.PensionAlbaniaUploaded,
                Name,
                Name,
                NEERemarkSeverity.Low,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = false;
            if (Application.HasPensionAlbaniaDocument && !Application.HasPensionAlbaniaDocumentDecision)
            {
                HasFailed = true;
            }
            else if (!Application.HasPensionAlbaniaDocument2 && Application.HasPensionAlbaniaDocumentDecision)
            {
                HasFailed = (bool)!Application.IsPensionAlbaniaDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.LowMedium;
                RelatedRemark.Message = Application.PensionAlbaniaDocumentRejectionReason;
            }
            else if (Application.HasPensionAlbaniaDocument2 && Application.HasPensionAlbaniaDocumentDecision)
            {
                HasFailed = (bool)!Application.IsPensionAlbaniaDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.MediumHigh;
                RelatedRemark.Message = Application.PensionAlbaniaDocumentRejectionReason2;
            }
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
