using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RulesEngine.Rules
{
    public class RegexRule : IRule<string>
    {
        private Regex _regex;

        public RegexRule(Regex regex)
        {
            if (regex == null) throw new System.ArgumentNullException("regex");
            _regex = regex;
        }

        public ValidationResult Validate(string value)
        {
            //NOTE: Yes, null string will pass RegexRules. Use the NotNullRule in combination to invalidate nulls.
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                if (_regex.IsMatch(value))
                    return ValidationResult.Success;
                else
                    return ValidationResult.Fail(_regex);
            }
        }

        public string RuleKind
        {
            get { return RuleKinds.RegexRule; }
        }
    }
}
