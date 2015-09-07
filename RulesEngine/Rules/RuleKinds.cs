using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulesEngine.Rules
{
    public static class RuleKinds
    {
        public const string BetweenRule = "BetweenRule";
        public const string EqualRule = "EqualRule";
        public const string GenericRule = "GenericRule";
        public const string GreaterThanRule = "GreaterThanRule";
        public const string GreaterThanOrEqualToRule = "GreaterThanOrEqualToRule";
        public const string LessThanRule = "LessThanRule";
        public const string LessThanOrEqualToRule = "LessThanOrEqualToRule";
        public const string NoLeadingWhitespaceRule = "NoLeadingWhitespaceRule";
        public const string NotEqualRule = "NotEqualRule";
        public const string NotNullOrEmpty = "NotNullOrEmpty";
        public const string NotNullRule = "NotNullRule";
        public const string NotOneOfRule = "NotOneOfRule";
        public const string NullRule = "NullRule";
        public const string OfTypeRule = "OfTypeRule";
        public const string OneOfRule = "OneOfRule";
        public const string RegexRule = "RegexRule";
    }
}
