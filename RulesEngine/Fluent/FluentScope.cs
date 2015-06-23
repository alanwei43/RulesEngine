using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulesEngine.Fluent
{
    public class FluentScope
    {
        private readonly Dictionary<string, object> _values;
        private readonly FluentScope _parent;
        private FluentScope _child;

        public FluentScope()
            : this(null, new Dictionary<string, object>())
        {
        }

        private FluentScope(FluentScope parent, Dictionary<string, object> values)
        {
            _parent = parent;
            _values = values;
        }

        /// <summary>
        /// Gets Parent
        /// </summary>
        public FluentScope Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets Child
        /// </summary>
        public FluentScope Child
        {
            get { return _child; }
        }

        public T Get<T>(IFluentScopeKey<T> key, T valueIfNotfound)
        {
            return Get(key.Key, valueIfNotfound, false);
        }
        public T Get<T>(IFluentScopeKey<T> key)
        {
            return Get(key.Key, default(T), true);
        }
        private T Get<T>(string key, T valueIfNotfound, bool throwIfNotFound)
        {
            if (key == null) throw new System.ArgumentNullException("key");
            object result;
            if (_values.TryGetValue(key, out result))
            {
                return (T)result;
            }

            if (throwIfNotFound) throw new KeyNotFoundException(key);
            return valueIfNotfound;
        }

        public void Set<T>(IFluentScopeKey<T> key, T value)
        {
            if (key == null) throw new System.ArgumentNullException("key");
            _values[key.Key] = value;
        }

        public void Delete<T>(IFluentScopeKey<T> key)
        {
            if (key == null) throw new System.ArgumentNullException("key");
            _values.Remove(key.Key);
        }

        public T Copy<T>(IFluentScopeKey<T> keyFrom, IFluentScopeKey<T> keyTo)
        {
            T valueFrom = this.Get(keyFrom.Key, default(T), true);
            Set(keyTo, valueFrom);
            return valueFrom;
        }

        /// <summary>
        /// Creates a child node
        /// </summary>
        public FluentScope CreateChild()
        {
            return CreateChild(this);
        }

        /// <summary>
        /// Creates a child node
        /// </summary>
        /// <param name="getInheritedValuesFrom">Node to get inherited values from</param>
        /// <returns></returns>
        public FluentScope CreateChild(FluentScope getInheritedValuesFrom)
        {
            if (getInheritedValuesFrom == null) throw new ArgumentNullException("getValuesFrom");
            if (_child != null) throw new InvalidOperationException("Cannot call CreateChild when Child already exists.");

            var values = getInheritedValuesFrom._values.ToDictionary(kp => kp.Key, kp => kp.Value is IFluentScopeValue ? ((IFluentScopeValue)kp.Value).Copy() : kp.Value);
            var result = new FluentScope(this, values);
            _child = result;
            return result;
        }
    }
}
