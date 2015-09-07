using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Rules
{
    public class NotNullRule<R> : IRule<R>
        where R : class
    {
        public ValidationResult Validate(R value)
        {
            if (value != null)
                return ValidationResult.Success;
            else
                return ValidationResult.Fail();
        }

        public string RuleKind
        {
            get { return RuleKinds.NotNullRule; }
        }
    }
}
