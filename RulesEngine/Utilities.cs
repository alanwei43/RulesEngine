using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    internal static class Utilities
    {
        public static Expression<Func<T, T>> ReturnSelf<T>()
        {
            var t = Expression.Parameter(typeof(T), "t");
            return Expression.Lambda<Func<T, T>>(t, t);
        }
        
        public static LambdaExpression ReturnSelf(Type source)
        {
            var t = Expression.Parameter(source, "t");
            return Expression.Lambda(t, t);
        }

        public static int CombineHash(int hashcode, params int[] otherHashes)
        {
            for (int i = 0; i < otherHashes.Length; i++)
            {
                hashcode = (hashcode << 5) - hashcode ^ otherHashes[i];
            }
            return hashcode;
        }

        public static Type CreateType(Type type, Type genericArgument)
        {
            if (genericArgument == null) throw new System.ArgumentNullException("genericArgument");
            return CreateType(type, new[] { genericArgument });
        }
        public static Type CreateType(Type type, params Type[] genericArguments)
        {
            if (genericArguments == null) throw new System.ArgumentNullException("genericArguments");
            if (type == null) throw new System.ArgumentNullException("type");
            var result = type.MakeGenericType(genericArguments);
            return result;
        }

        public static object CreateInstance(this Type type)
        {
            return CreateInstance(type, new object[0]);
        }

        public static object CreateInstance(this Type type, object constructorArg)
        {
            if (constructorArg == null) throw new System.ArgumentNullException("constructorArg");
            return CreateInstance(type, new object[] { constructorArg });
        }

        public static object CreateInstance(this Type type, params object[] constructorArgs)
        {
            if (constructorArgs == null) throw new System.ArgumentNullException("constructorArgs");
            if (type == null) throw new System.ArgumentNullException("type");
            return Activator.CreateInstance(type, constructorArgs);
        }

        public static bool TryGetCulprit(LambdaExpression expression, out LambdaExpression culpritValue, out LambdaExpression culpritExpression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                culpritValue = null;
                culpritExpression = null;
                return false;
            }
            var genericArgs = expression.Type.GetGenericArguments().ToArray();
            genericArgs[genericArgs.Length - 1] = memberExpression.Expression.Type;
            var lambdaType = Utilities.CreateType(typeof(Func<,>), genericArgs);

            culpritValue = Expression.Lambda(lambdaType, memberExpression.Expression, expression.Parameters);

            var parameter = Expression.Parameter(culpritValue.ReturnType, "p0");
            var member = Expression.MakeMemberAccess(parameter, memberExpression.Member);
            culpritExpression = Expression.Lambda(member, parameter);
            return true;
        }

    }
}
