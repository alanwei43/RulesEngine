using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine
{
    public interface IErrorResolver : ICloneable
    {
        void AddEntry(MessageEntry entry);
        string GetErrorMessage(Type type, EquatableExpression expression, IRule rule, object[] arguments);
    }
}
