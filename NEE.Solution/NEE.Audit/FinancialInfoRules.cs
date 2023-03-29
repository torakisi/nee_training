using NEE.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Audit
{
    public class FinancialInfoRules: AsyncAuditRule
    {
        protected override async Task<AuditResult> CheckCoreAsync(Application application, AuditContext context)
        {
            var applicant = application.Applicant;

            //var ageAtApproval = application.Applicant.AgeAt(application.ApprovedAt.Value);
            //if (ageAtApproval < 25)
            //{
            //    var xsRes = await this.GetAcademicStatus(applicant);
            //    if (xsRes.IsAcademic)
            //    {
            //        var remark = RemarkHigh("Ο αιτών είναι φοιτητής με ηλικία μικρότερη από 25 κατά την έγκριση")
            //                    .WithRelated(application.Applicant)
            //                    .WithRelated(new
            //                    {
            //                        DOB = applicant.DOB,
            //                        AgeAtApproval = ageAtApproval,
            //                        AcademicCardId = xsRes.Submissions.FirstOrDefault().Code,
            //                        AcademicDescription = xsRes.Submissions.FirstOrDefault().Name
            //                    });
            //        return Invalid(remark);
            //    }
            //    return Valid();
            //}
            return Valid();
        }
    }
}
