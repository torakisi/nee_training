using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationSpousePensionAlbaniaUploaded : ApplicationValidationRule
    {
        public ApplicationValidationSpousePensionAlbaniaUploaded(Application application)
            : base(application, SpousePensionAlbaniaUploaded)
        {
            this.RelatedRemark = new Remark(RemarkType.SpousePensionAlbaniaUploaded,
                Name,
                Name,
                NEERemarkSeverity.Low,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = false;
            if (Application.HasSpousePensionDocument && !Application.HasSpousePensionDocumentDecision)
            {
                HasFailed = true;
            }
            else if (!Application.HasSpousePensionDocument2 && Application.HasSpousePensionDocumentDecision)
            {
                HasFailed = (bool)!Application.IsSpousePensionDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.LowMedium;
                RelatedRemark.Message = Application.SpousePensionDocumentRejectionReason;
            }
            else if (Application.HasSpousePensionDocument2 && Application.HasSpousePensionDocumentDecision)
            {
                HasFailed = (bool)!Application.IsSpousePensionDocumentApproved;
                RelatedRemark.Severity = NEERemarkSeverity.MediumHigh;
                RelatedRemark.Message = Application.SpousePensionDocumentRejectionReason2;
            }
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
