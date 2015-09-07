using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public interface IRule
    {
        string RuleKind { get; }
    }

    public interface IRule<R> : IRule
    {
        ValidationResult Validate(R value);
    }
}
