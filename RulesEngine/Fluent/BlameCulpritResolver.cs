using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    internal class BlameCulpritResolver : ICulpritResolver
    {
        private Delegate _culpritObjectExpressionDelegate;
        private LambdaExpression _culpritExpression;

        public BlameCulpritResolver(LambdaExpression culpritObjectExpression, LambdaExpression culpritExpression)
        {
            //NOTE: You can have a null culpritObjectExpression
            if (culpritExpression == null) throw new ArgumentNullException("culpritExpression");
            if (culpritObjectExpression != null)
            {
                _culpritObjectExpressionDelegate = culpritObjectExpression.Compile();
            }
            _culpritExpression = culpritExpression;
        }

        public IEnumerable<CulpritResult> Resolve(object value)
        {
            var culprit = _culpritObjectExpressionDelegate == null ? value : _culpritObjectExpressionDelegate.DynamicInvoke(value);
            return new[] { new CulpritResult(culprit, _culpritExpression) };
        }
    }
}
