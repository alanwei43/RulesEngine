using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Rules
{
    public class NoLeadingWhitespaceRule : IRule<string>
    {
        public ValidationResult Validate(string value)
        {
            if (value == null || value.Length == 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                if (!char.IsWhiteSpace(value[0]))
                    return ValidationResult.Success;
                else
                    return ValidationResult.Fail();
            }
        }

        public string RuleKind
        {
            get { return RuleKinds.NoLeadingWhitespaceRule; }
        }
    }
}
