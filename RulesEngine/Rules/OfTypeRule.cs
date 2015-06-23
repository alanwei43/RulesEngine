using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulesEngine.Rules
{
    public class OfTypeRule<R>
        : IRule<R>
    {
        Type _type;

        public OfTypeRule(Type type)
        {
            _type = type;
        }

        public ValidationResult Validate(R value)
        {
            if (value != null && _type.IsAssignableFrom(value.GetType()))
                return ValidationResult.Success;
            else
                return ValidationResult.Fail(_type);
        }

        public string RuleKind
        {
            get { return RuleKinds.OfTypeRule; }
        }
    }
}
