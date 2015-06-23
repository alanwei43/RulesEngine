using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine
{
    public class CulpritResult
    {
        private readonly object _value;
        private readonly EquatableExpression _expression;

        public object Value
        {
            get
            {
                return _value;
            }
        }
        public EquatableExpression Expression
        {
            get
            {
                return _expression;
            }
        }

        public CulpritResult(object value, EquatableExpression expression)
        {
            _value = value;
            _expression = expression;
        }
    }

    public interface ICulpritResolverFactory
    {
        ICulpritResolver Create(LambdaExpression culpritObjectExpression, LambdaExpression culpritExpression);
    }

    public interface ICulpritResolver
    {
        IEnumerable<CulpritResult> Resolve(object value);
    }

    public class DefaultCulpritResolverFactory : ICulpritResolverFactory
    {
        public ICulpritResolver Create(LambdaExpression culpritObjectExpression, LambdaExpression culpritExpression)
        {
            return new DefaultCulpritResolver(culpritExpression);
        }
    }


    public class DefaultCulpritResolver : ICulpritResolver
    {
        LambdaExpression _expression;
        Delegate _culpritValueDelegate;
        EquatableExpression _culpritExpression;

        public DefaultCulpritResolver(LambdaExpression expression)
        {
            Initialize(expression);
        }

        public IEnumerable<CulpritResult> Resolve(object value)
        {
            CulpritResult result;
            if (_culpritValueDelegate == null)
            {
                result = new CulpritResult(value, _culpritExpression); 
            }
            else
            {
                var valueToBlame = _culpritValueDelegate.DynamicInvoke(value);
                var expressionToBlame = _culpritExpression;
                result = new CulpritResult(valueToBlame, expressionToBlame);
            }

            return new[] { result };
        }

        private void Initialize(LambdaExpression expression)
        {
            _expression = expression;

            LambdaExpression culpritValue;
            LambdaExpression culpritExpression;

            if (Utilities.TryGetCulprit(expression, out culpritValue, out culpritExpression))
            {
                _culpritValueDelegate = culpritValue.Compile();
                _culpritExpression = culpritExpression;
            }
            else
            {
                _culpritValueDelegate = null;
                _culpritExpression = expression;
            }
        }

    }





    public interface IValueResolverFactory
    {
        IValueResolver CreateResolver(LambdaExpression expression);
    }
    public class DefaultValueResolverFactory : IValueResolverFactory
    {
        public IValueResolver CreateResolver(LambdaExpression expression)
        {
            var result = (IValueResolver)Utilities.CreateType(
                                            typeof(DefaultValueResolver<,>)
                                            , expression.Parameters[0].Type
                                            , expression.ReturnType)
                                        .CreateInstance(expression);
            return result;
        }
    }


    public interface IValueResolver
    {
        bool TryGetValue(object source, out object result);
    }
    public interface IValueResolver<T, R> : IValueResolver
    {
        bool TryGetValue(T source, out R result);
    }

    public class DefaultValueResolver<T, R> : IValueResolver<T, R>
    {
        Func<T, R> _compiledExpression;

        public DefaultValueResolver(Expression<Func<T, R>> expression)
        {
            if (expression == null) throw new System.ArgumentNullException("expression");
            _compiledExpression = expression.Compile();
        }

        public bool TryGetValue(T source, out R result)
        {
            try
            {
                result = _compiledExpression(source);
                return true;
            }
            catch (NullReferenceException) { }
            catch (IndexOutOfRangeException) { }
            catch (KeyNotFoundException) { }
            result = default(R);
            return false;
        }

        bool IValueResolver.TryGetValue(object source, out object result)
        {
            R tmpResult;
            var success = TryGetValue((T)source, out tmpResult);
            result = tmpResult;
            return success;
        }
    }


    public class RuleInvoker<T, R> : IRuleInvoker
    {
        IRule<R> _rule;
        EquatableExpression _originatingExpression;
        private IValueResolver<T, R> _valueToValidate;
        private ICulpritResolver _culpritResolver;

        public RuleInvoker(IRule<R> rule, IValueResolver<T, R> valueToValidate, EquatableExpression originatingExpression, ICulpritResolver culpritResolver)
        {
            _rule = rule;
            _originatingExpression = originatingExpression;
            _valueToValidate = valueToValidate;
            _culpritResolver = culpritResolver;
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            //If validating an Expression that has already failed a rule, then skip.
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(value, _originatingExpression))
            {
                return;
            }

            var originatingValue = (T)value;
            R valueToValidate;
            if (!_valueToValidate.TryGetValue(originatingValue, out valueToValidate)) return;
            var result = _rule.Validate(valueToValidate);
            if (!result.IsValid)
            {
                var culpritResults = _culpritResolver.Resolve(originatingValue);
                foreach (var culpritResult in culpritResults)
                {
                    report.AddError(new ValidationError(_rule, culpritResult.Expression, result.Arguments, culpritResult.Value, originatingValue, _originatingExpression));
                }
            }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }

    }
}
