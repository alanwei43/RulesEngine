using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine
{
    public interface IErrorResolver
    {
        void SetErrorMessageFor(Type type, EquatableExpression expression, IRule rule, string message);
        void SetErrorMessageFor(Type type, EquatableExpression expression, IRule rule, Func<string> message);
        string GetErrorMessage(Type type, EquatableExpression expression, IRule rule, object[] arguments);
        IErrorResolver Clone();
    }
}
