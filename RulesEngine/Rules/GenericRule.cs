using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Rules
{
    public class GenericRule<R> : IRule<R>
    {
        Func<R, bool> _rule;

        public ValidationResult Validate(R value)
        {
            if (_rule(value))
                return ValidationResult.Success;
            else
                return ValidationResult.Fail();
        }

        public GenericRule(Func<R, bool> rule)
        {
            if (rule == null) throw new ArgumentNullException("rule");
            _rule = rule;
        }

        public string RuleKind
        {
            get { return RuleKinds.GenericRule; }
        }
    }
}
