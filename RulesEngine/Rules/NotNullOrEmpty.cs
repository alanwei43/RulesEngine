using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RulesEngine.Rules
{
    public class NotNullOrEmpty : IRule<string>
    {
        public NotNullOrEmpty()
        {
        }

        public ValidationResult Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ValidationResult.Fail();
            else
                return ValidationResult.Success;
        }

        public string RuleKind
        {
            get { return RuleKinds.NotNullOrEmpty; }
        }
    }
}
