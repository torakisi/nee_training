using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationFEKUploaded : ApplicationValidationRule
    {
        public ApplicationValidationFEKUploaded(Application application)
            : base(application, FEKUploaded)
        {
            this.RelatedRemark = new Remark(RemarkType.FEKUploaded,
                Name,
                Name,
                NEERemarkSeverity.Low,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = false;
            if (Application.HasFEKDocument && !Application.HasFEKDocumentDecision)
            {
                HasFailed = true;
                RelatedRemark.Severity = NEERemarkSeverity.Low;
            }
            else if (!Application.HasFEKDocument2 && Application.HasFEKDocumentDecision)
            {
                HasFailed = (bool)!Application.IsFEKDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.LowMedium;
                RelatedRemark.Message = Application.FEKDocumentRejectionReason;
            }
            else if (Application.HasFEKDocument2 && Application.HasFEKDocumentDecision)
            {
                HasFailed = (bool)!Application.IsFEKDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.MediumHigh;
                RelatedRemark.Message = Application.FEKDocumentRejectionReason2;
            }
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
