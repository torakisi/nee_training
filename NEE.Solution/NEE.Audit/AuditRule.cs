using NEE.Core.BO;
using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Audit
{
    public abstract class AsyncAuditRule : AuditRule
    {
        protected abstract Task<AuditResult> CheckCoreAsync(Application application, AuditContext context);
        protected override AuditResult CheckCore(Application application, AuditContext context)
        {
            return Core.Helpers.AsyncHelper.RunSync(() => CheckCoreAsync(application, context));
        }
    }
    public abstract class AuditRule
    {
        public string Id => GetType().Name;

        public AuditResult Check(Application application, AuditContext context)
        {
            try
            {
                return CheckCore(application, context)
                            .WithAuditInfo(application.Id, application.ApprovedAt, context.AuditDate);
            }
            catch (Exception ex)
            {
                return AuditResult.Invalid(DateTime.Now, new[] { AuditRemark.Error(this, ex) });
            }
        }
        protected abstract AuditResult CheckCore(Application application, AuditContext context);

        protected AuditRemark Remark(NEERemarkSeverity severity, string description)
        {
            return AuditRemark.Create(this, severity, description);
        }
        protected AuditRemark RemarkHigh(string description) => Remark(NEERemarkSeverity.High, description);
        protected AuditRemark RemarkMedium(string description) => Remark(NEERemarkSeverity.Medium, description);
        protected AuditRemark RemarkLow(string description) => Remark(NEERemarkSeverity.Low, description);

        protected AuditResult Valid()
        {
            return AuditResult.Valid(GetAuditedAt());
        }
        protected AuditResult Invalid(AuditRemark remark, params AuditRemark[] otherRemarks)
        {
            var remarks = new List<AuditRemark>();
            remarks.Add(remark);
            remarks.AddRange(otherRemarks);
            return Invalid(remarks);
        }
        protected AuditResult Result(IEnumerable<AuditRemark> remarks) =>
            remarks.Any()
                ? Invalid(remarks)
                : Valid();

        protected AuditResult Invalid(IEnumerable<AuditRemark> remarks)
        {
            return AuditResult.Invalid(GetAuditedAt(), remarks);
        }
        protected AuditResult Invalid(string remark, NEERemarkSeverity severity)
        {
            return Invalid(Remark(severity, remark));
        }
        protected AuditResult InvalidHigh(string remark) => Invalid(remark, NEERemarkSeverity.High);
        protected AuditResult InvalidMedium(string remark) => Invalid(remark, NEERemarkSeverity.Medium);
        protected AuditResult InvalidLow(string remark) => Invalid(remark, NEERemarkSeverity.Low);

        protected AuditResult Error(Exception ex) => Invalid(AuditRemark.Error(this, ex));
        protected AuditResult Error(string message) => Invalid(AuditRemark.Error(this, message));

        private DateTime GetAuditedAt() => DateTime.Now;
    }
}
