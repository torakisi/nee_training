using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Rules;

namespace NEE.Service.RuleProviders.Rules
{
    public class ApplicationValidationAADEVehicleExceeded : ApplicationValidationRule
    {
        public ApplicationValidationAADEVehicleExceeded(Application application)
            : base(application, AADEVehicleExceeded)
        {
            this.RelatedRemark = new Remark(RemarkType.AADEVehicleExceeded,
                Name,
                Name,
                NEERemarkSeverity.High,
                Application.Applicant.AMKA,
                Application.Applicant.AFM);
        }

        public override bool? CheckHasFailed()
        {
            HasFailed = Application.Applicant.VehiclesValue > 6000;
            return HasFailed;
        }

        public override Remark GetRelatedRemark()
        {
            return this.RelatedRemark;
        }
    }
}
