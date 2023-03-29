using System.Collections.Generic;
using System.Data;

namespace NEE.Core.Rules
{
    public interface IRuleProvider
    {
        void AddRule(Rule rule);
        List<Rule> GetRules();
        void Validate();
    }
}
