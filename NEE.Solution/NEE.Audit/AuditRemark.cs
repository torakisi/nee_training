using NEE.Core;
using NEE.Core.BO;
using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Audit
{
    public class AuditRemark : ICloneable
    {
        public string Rule { get; private set; }
        public string Description { get; private set; }
        public NEERemarkSeverity Severity { get; private set; }
        public bool IsError { get; private set; }

        public Dictionary<string, object> Related { get; set; } = new Dictionary<string, object>();
        public bool ShouldSerializeRelated() => (this.Related != null) && this.Related.Any();

        private AuditRemark() { }
        private AuditRemark(string rule, NEERemarkSeverity severity, string description, bool isError)
        {
            Rule = rule;
            Severity = severity;
            Description = description;
            IsError = isError;
        }

        public object Clone()
        {
            var ret = new AuditRemark();

            ret.Rule = this.Rule;
            ret.Severity = this.Severity;
            ret.Description = this.Description;
            ret.IsError = this.IsError;

            ret.Related = new Dictionary<string, object>(this.Related);

            return ret;

        }

        public static AuditRemark Create(AuditRule rule, NEERemarkSeverity severity, string description)
        {
            return new AuditRemark(rule.GetType().FullName, severity, description, isError: false);
        }
        public static AuditRemark Error(AuditRule rule, Exception ex)
        {
            return new AuditRemark(rule.GetType().FullName, NEERemarkSeverity.High, ex.Message, isError: true);
        }
        public static AuditRemark Error(AuditRule rule, string message)
        {
            return new AuditRemark(rule.GetType().FullName, NEERemarkSeverity.High, message, isError: true);
        }


        public AuditRemark WithRelated(IDictionary<string, object> items)
        {
            AuditRemark ret = (AuditRemark)this.Clone();
            ret.Related.AddRange(items);
            return ret;
        }

        public AuditRemark WithRelated(string key, object value)
        {
            AuditRemark ret = (AuditRemark)this.Clone();
            ret.Related.Add(key, value);
            return ret;
        }

        public AuditRemark WithRelated(object anonymousType) => this.WithRelated(anonymousType.ToDictionary());

        public AuditRemark WithRelated(Person person) => this.WithRelated(new { person.AMKA, person.AFM });


    }
}
