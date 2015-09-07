using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Tests
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, T1>> New<T, T1>(Expression<Func<T, T1>> expression)
        {
            return expression;
        }
        public static Expression<Func<T, T1, T2>> New<T, T1, T2>(Expression<Func<T, T1, T2>> expression)
        {
            return expression;
        }
    }
}
