using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;
using RulesEngine.Fluent;

namespace RulesEngine
{
    public interface IInvokerRegistry : ICloneable
    {
        void RegisterInvoker(IRuleInvoker ruleInvoker);
        IRuleInvoker[] GetInvokers(Type type);
        IRuleInvoker[] GetInvokers();
    }

    public class InvokerRegistry : IInvokerRegistry
    {
        private readonly object _synclock = new object();

        //Use a list rather than a Dictionary here because the order in which the invokers are added is relevant.
        private readonly List<IRuleInvoker> _invokers = new List<IRuleInvoker>();

        //Store all invokers relevant for a type. E.g. For a 'Person' Type, you will get invokers that apply to parent class/interfaces of Person.
        private readonly Dictionary<Type, IRuleInvoker[]> _normalizedInvokers = new Dictionary<Type, IRuleInvoker[]>();

        public IRuleInvoker[] GetInvokers()
        {
            return _invokers.ToArray();
        }

        public IRuleInvoker[] GetInvokers(Type type)
        {
            IRuleInvoker[] result;
            if (_normalizedInvokers.TryGetValue(type, out result)) return result;

            lock (_synclock)
            {
                if (_normalizedInvokers.TryGetValue(type, out result)) return result;
                result = _invokers.Where(i => IsTypeCompatible(type, i.ParameterType)).ToArray();
                _normalizedInvokers[type] = result;
            }

            return result;

        }

        public void RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            //Re-Calculate normalized invokers every time a new invoker is added.
            lock (_synclock)
            {
                _normalizedInvokers.Clear();
                _invokers.Add(ruleInvoker);
            }
        }

        private bool IsTypeCompatible(Type value, Type invokerType)
        {
            return invokerType.IsAssignableFrom(value);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public InvokerRegistry Clone()
        {
            var result = new InvokerRegistry();
            result._invokers.AddRange(_invokers);
            return result;
        }
    }
}
