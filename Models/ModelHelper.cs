using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace RulesEngine.Model
{
    public static class ModelHelper
    {
        internal static IGetter<T> CreateGetter<T>(IWrapper<T> wrapper, PropertyInfo property)
        {
            ParameterExpression paramExp = Expression.Parameter(property.ReflectedType);
            Expression propExp = Expression.Property(paramExp, property);
            LambdaExpression action = Expression.Lambda(propExp, paramExp);

            Type getterType = typeof(Getter<,>).MakeGenericType(typeof(T), action.ReturnType);
            return (IGetter<T>)getterType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(new object[] { wrapper, action });
        }

        internal static Delegate CreateSetter(PropertyInfo property)
        {
            ParameterExpression param1Exp = Expression.Parameter(property.ReflectedType);
            ParameterExpression param2Exp = Expression.Parameter(property.PropertyType);

            Expression propExp = Expression.Property(param1Exp, property);
            Expression assignmentExp = Expression.Assign(propExp, param2Exp);
            LambdaExpression action = Expression.Lambda(assignmentExp, param1Exp, param2Exp);
            return action.Compile();
        }

        public static IWrapper<T> CreateModel<T>(T value)
        {
            if (value == null) throw new System.ArgumentNullException("value");
            return new MyDynamic<T>(value);
        }
        public static IWrapper<T> CreateModel<T>(ICollection<T> value)
        {
            if (value == null) throw new System.ArgumentNullException("value");
            return new MyDynamicCollection<T>(value);
        }
    }
}
