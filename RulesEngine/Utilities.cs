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
        public static int CombineHash(int hashcode, params int[] otherHashes)
        {
            for (int i = 0; i < otherHashes.Length; i++)
            {
                hashcode = (hashcode << 5) - hashcode ^ otherHashes[i];
            }
            return hashcode;
        }
    }
}
