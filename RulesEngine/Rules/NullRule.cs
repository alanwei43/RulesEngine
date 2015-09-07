using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulesEngine.Rules
{
    public class NullRule<R> : IRule<R>
        where R : class
    {
        public ValidationResult Validate(R value)
        {
            if (value == null)
                return ValidationResult.Success;
            else
                return ValidationResult.Fail();
        }

        public string RuleKind
        {
            get { return RuleKinds.NullRule; }
        }
    }
}
