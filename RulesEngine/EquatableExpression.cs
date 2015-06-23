using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public class EquatableExpression : IEquatable<EquatableExpression>
    {
        private class _DefaultComparer : IEqualityComparer<EquatableExpression>
        {
            public bool Equals(EquatableExpression x, EquatableExpression y)
            {
                if (object.ReferenceEquals(x, y)) return true;
                if (object.ReferenceEquals(null, x) || object.ReferenceEquals(null, y)) return false;

                return x.ParameterType == y.ParameterType
                        && x._expressionAsString == y._expressionAsString;
            }

            public int GetHashCode(EquatableExpression obj)
            {
                if (object.ReferenceEquals(null, obj)) return 0;
                return Utilities.CombineHash(obj.ParameterType.GetHashCode(), obj._expressionAsString.GetHashCode());
            }
        }
        private class _AppliesToComparer : IEqualityComparer<EquatableExpression>
        {
            public bool Equals(EquatableExpression x, EquatableExpression y)
            {
                if (object.ReferenceEquals(x, y)) return true;
                if (object.ReferenceEquals(null, x) || object.ReferenceEquals(null, y)) return false;

                return x._expressionAsString == y._expressionAsString
                                && (x.ParameterType.IsAssignableFrom(y.ParameterType)
                                || y.ParameterType.IsAssignableFrom(x.ParameterType));
            }

            public int GetHashCode(EquatableExpression obj)
            {
                if (object.ReferenceEquals(null, obj)) return 0;
                return obj._expressionAsString.GetHashCode();
            }
        }
        private class ParameterReplacerVisitor : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMap = new Dictionary<ParameterExpression, ParameterExpression>();

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _parameterMap[node];
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var orderedMapped = new List<ParameterExpression>();
                for (int i = 0; i < node.Parameters.Count; i++)
                {
                    var p = node.Parameters[i];
                    var mapped = System.Linq.Expressions.Expression.Parameter(p.Type, "p" + i);
                    _parameterMap.Add(p, mapped);
                    orderedMapped.Add(mapped);
                }
                return System.Linq.Expressions.Expression.Lambda<T>(this.Visit(node.Body), node.Name, node.TailCall, orderedMapped);
            }
        }

        private static readonly IEqualityComparer<EquatableExpression> _defaultComparer = new _DefaultComparer();
        private static readonly IEqualityComparer<EquatableExpression> _appliesToComparer = new _AppliesToComparer();

        LambdaExpression _expression;
        string _expressionAsString;
        Type _parameterType;

        /// <summary>
        /// Gets Expression
        /// </summary>
        public LambdaExpression Expression
        {
            get { return _expression; }
        }

        /// <summary>
        /// Gets ParameterType
        /// </summary>
        public Type ParameterType
        {
            get { return _parameterType; }
        }

        /// <summary>
        /// Gets DefaultComparer
        /// </summary>
        public static IEqualityComparer<EquatableExpression> DefaultComparer
        {
            get { return _defaultComparer; }
        }

        /// <summary>
        /// Gets AppliesToComparer
        /// </summary>
        public static IEqualityComparer<EquatableExpression> AppliesToComparer
        {
            get { return _appliesToComparer; }
        }

        public EquatableExpression(LambdaExpression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (expression.Parameters.Count != 1) throw new ArgumentException("Invalid LambdaExpression. Must only have one parameter.");
            _expression = expression;
            _parameterType = _expression.Parameters[0].Type;
            var visitor = new ParameterReplacerVisitor();
            _expressionAsString = visitor.Visit(expression).ToString();
        }

        public bool AppliesTo(EquatableExpression expression)
        {
            return AppliesToComparer.Equals(this, expression);
        }

        public override string ToString()
        {
            return _expressionAsString;
        }

        public bool Equals(EquatableExpression other)
        {
            return DefaultComparer.Equals(this, other);
        }
        public override bool Equals(object obj)
        {
            return DefaultComparer.Equals(this, obj as EquatableExpression);
        }
        public override int GetHashCode()
        {
            return DefaultComparer.GetHashCode(this);
        }
        public static bool operator ==(EquatableExpression a, EquatableExpression b)
        {
            return DefaultComparer.Equals(a, b);
        }
        public static bool operator !=(EquatableExpression a, EquatableExpression b)
        {
            return !(a == b);
        }
        public static implicit operator LambdaExpression(EquatableExpression expression)
        {
            if (expression == null) return null;
            return expression._expression;
        }
        public static implicit operator EquatableExpression(LambdaExpression expression)
        {
            if (expression == null) return null;
            return new EquatableExpression(expression);
        }
        public static EquatableExpression Create<T1, T>(Expression<Func<T1, T>> expression)
        {
            return new EquatableExpression(expression);
        }
    }
}
