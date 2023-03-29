using System.Collections.Generic;
using System.Data;

namespace NEE.Core.Rules
{
    public class RuleProvider : IRuleProvider
    {
        List<Rule> Rules { get; set; } = new List<Rule>();
        public void AddRule(Rule rule)
        {
            Rules.Add(rule);
        }

        public List<Rule> GetRules()
        {
            return Rules;
        }

        private bool isValid = false;

        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }

        public void Validate()
        {
            bool oneRuleFailed = false;

            foreach (Rule rule in Rules)
            {
                var rulefailed = rule.CheckHasFailed();
                if (rulefailed.HasValue && rulefailed.Value)
                {
                    oneRuleFailed = true;
                }
            }

            isValid = !oneRuleFailed;
        }

    }
}
