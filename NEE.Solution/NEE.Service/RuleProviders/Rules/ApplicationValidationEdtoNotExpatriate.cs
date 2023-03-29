using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationEdtoNotExpatriate : ApplicationValidationRule
    {
        public ApplicationValidationEdtoNotExpatriate(Application application)
            : base(application, EdtoNotExpatriate)
        {
            this.RelatedRemark = new Remark(RemarkType.EdtoNotExpatriate,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = (Application.Applicant.PermitNumber == null && Application.ProvidedFEKDocument == false) || Application.Applicant.AdministrationDate >= new System.DateTime(2023,01,01);            
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
