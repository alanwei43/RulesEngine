using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Rules
{
    public class NullableNullRule<R> : IRule<Nullable<R>>
        where R: struct
    {
        public ValidationResult Validate(R? value)
        {
            if (!value.HasValue)
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
