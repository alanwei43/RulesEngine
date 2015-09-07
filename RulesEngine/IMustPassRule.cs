using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public interface IMustPassRule<M, T, R>
    {
        M MustPassRule(IRule<R> rule);
        M MustPassRule(IRule<T> rule);
        Expression<Func<T, R>> Expression { get; }
        M GetSelf();
        Engine RulesEngine { get; }
        IRule LastRule { get; }
    }
}
