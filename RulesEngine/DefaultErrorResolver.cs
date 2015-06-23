using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine
{
    public class DefaultErrorResolver : IErrorResolver
    {
        private readonly List<MessageEntry> _messageEntries = new List<MessageEntry>();

        private EquatableExpression GetSelf(Type type)
        {
            var p = Expression.Parameter(type, "p");
            var lambda = Expression.Lambda(p, p);
            return new EquatableExpression(lambda);
        }

        public void AddEntry(MessageEntry entry)
        {
            if (entry == null) throw new System.ArgumentNullException("entry");

            //If the expression returns self, ignore the expression.
            var expression = entry.Expression == GetSelf(entry.Expression.ParameterType) ? null : entry.Expression;

            //It is possible that AddEntry is called multiple times with the same MessageEntry. It is has no other consequence other than bloating _messageEntries.
            //Still, prevent that from happening.
            if (_messageEntries.Any(m => m.Type == entry.Type && m.Expression == expression && m.Rule == entry.Rule && m.Message == entry.Message)) return;
            _messageEntries.Add(new MessageEntry(entry.Type, expression, entry.Rule, entry.Message));
        }


        public string GetErrorMessage(Type type, EquatableExpression expression, IRule rule, object[] arguments)
        {
            var entry = GetEntry(type, rule, expression);
            if (entry == null)
            {
                foreach (Type baseType in GetTypeHierarchy(type))
                {
                    entry = GetEntry(baseType, rule, expression);
                    if (entry != null) break;
                }
            }
            if (entry == null) return null;
            return FormatMessage(entry.Message(), arguments);
        }

        protected virtual string FormatMessage(string format, object[] arguments)
        {
            var paddedArgs = new object[15];
            arguments.CopyTo(paddedArgs, 0);
            return string.Format(format, paddedArgs);
        }

        protected virtual MessageEntry GetEntry(Type type, IRule rule, EquatableExpression expression)
        {
            var entries = _messageEntries.ToArray();
            //entries = entries.Where(e => e.Type.IsAssignableFrom(validationError.Value.GetType())).ToArray();
            entries = entries.Where(e => e.Type == type).ToArray();
            if (entries.Length == 0) return null;

            var entriesByExpression = entries.Where(e => expression == null || expression.AppliesTo(e.Expression)).ToArray();
            if (entriesByExpression.Length == 0)
            {
                entriesByExpression = entries.Where(e => e.Expression == null).ToArray();
                if (entriesByExpression.Length == 0) return null;
            }

            var entriesByRule = entriesByExpression.Where(e => object.ReferenceEquals(e.Rule, rule)).ToArray();
            if (entriesByRule.Length == 0)
            {
                entriesByRule = entriesByExpression.Where(e => object.ReferenceEquals(e.Rule, null)).ToArray();
                if (entriesByRule.Length == 0) return null;
            }
            return entriesByRule.Last();
        }

        private Type[] GetTypeHierarchy(Type type)
        {
            List<Type> result = new List<Type>();
            var t = type.BaseType;
            while (t != typeof(object) && t != null)
            {
                result.Add(t);
                t = t.BaseType;
            }
            result.AddRange(GetInterfaces(type));
            return result.ToArray();
        }

        private Type[] GetInterfaces(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                return type.GetInterfaces().Except(GetInterfaces(type.BaseType)).ToArray();
            }
            return type.GetInterfaces();
        }

        public DefaultErrorResolver Clone()
        {
            var result = new DefaultErrorResolver();
            result._messageEntries.AddRange(_messageEntries);
            return result;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
