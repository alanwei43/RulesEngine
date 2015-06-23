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
    public sealed class Getter<T, R> : IGetter<T>,  IWrapper<T>
    {
        IWrapper<T> _wrapper;
        Expression<Func<T, R>> _expression;
        Func<T, R> _compiledExpression;

        internal Getter(IWrapper<T> wrapper, Expression<Func<T, R>> expression)
        {
            _wrapper = wrapper;
            //TODO: Tell the wrapper if there are dependant getters what other expressions are affected?
            _expression = expression;
            _compiledExpression = expression.Compile();
        }

        public Getter<T, R> Affects<R1>(Expression<Func<T, R1>> expression) 
        {
            return this;
        }
        public Getter<T, R> IsAffectedBy<R1>(Expression<Func<T, R1>> expression) 
        {
            return this;
        }

        public Getter<T, R1> CreateGetter<R1>(string name, Expression<Func<T, R1>> formula)
        {
            return _wrapper.CreateGetter<R1>(name, formula);
        }

        public IWrapper<T> CreateSetter<R>(string name, Expression<Action<T, R>> formula)
        {
            throw new NotImplementedException();
        }

        public static implicit operator R(Getter<T, R> formula)
        {
            return default(R);
        }

        public object Invoke(T value)
        {
            return _compiledExpression.Invoke(value);
        }

        public LambdaExpression Expression
        {
            get { return _expression; }
        }
    }
}
