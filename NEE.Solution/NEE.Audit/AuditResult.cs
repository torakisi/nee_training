using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Audit
{
    public class AuditResult : ICloneable
    {
        public bool IsValid { get; private set; }
        public string ApplicationId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public DateTime AuditDate { get; private set; }
        public DateTime AuditedAt { get; private set; }
        public bool HasErrors => Remarks.Any(r => r.IsError);

        public IEnumerable<AuditRemark> Remarks { get; private set; }

        private AuditResult() { }
        private AuditResult(bool isValid, DateTime auditedAt, IEnumerable<AuditRemark> remarks)
        {
            IsValid = isValid;
            AuditedAt = auditedAt;
            Remarks = remarks.ToArray();
        }
        private AuditResult(DateTime auditedAt, Exception error)
        {
            IsValid = false;
            AuditedAt = auditedAt;
            Remarks = new AuditRemark[0];
        }
        public object Clone()
        {
            var ret = new AuditResult();

            ret.ApplicationId = this.ApplicationId;
            ret.IsValid = this.IsValid;
            ret.AuditedAt = this.AuditedAt;

            ret.Remarks = (this.Remarks == null) ? null : this.Remarks.ToArray();

            return ret;
        }

        public AuditResult WithAuditInfo(string applicationId, DateTime? approvedAt, DateTime auditDate)
        {
            AuditResult ret = (AuditResult)this.Clone();
            ret.ApplicationId = applicationId;
            ret.ApprovedAt = approvedAt;
            ret.AuditDate = auditDate;
            return ret;
        }

        public AuditResult Merge(AuditResult other)
        {
            if (this.IsValid && other.IsValid)
            {
                return other.AuditedAt > AuditedAt
                    ? other
                    : this;
            }
            else if (!this.IsValid && !other.IsValid)
            {
                var latest = (other.AuditedAt > this.AuditedAt) ? other : this;
                return Invalid(latest.AuditedAt, this.Remarks.Concat(other.Remarks))
                    .WithAuditInfo(latest.ApplicationId, latest.ApprovedAt, latest.AuditDate);
            }
            else
                return this.IsValid
                    ? other
                    : this;
        }

        public static AuditResult Valid(DateTime auditedAt)
        {
            return new AuditResult(true, auditedAt, Enumerable.Empty<AuditRemark>());
        }
        public static AuditResult Invalid(DateTime auditedAt, IEnumerable<AuditRemark> remarks)
        {
            if (!remarks.Any())
                throw new InvalidOperationException("Cannot create invalid audit result without any remarks");

            return new AuditResult(false, auditedAt, remarks);
        }

        public static AuditResult Identity => Valid(DateTime.MinValue);

        public static AuditResult Merge(IEnumerable<AuditResult> results) =>
            results.Aggregate(Identity, (x, y) => x.Merge(y));
    }
}