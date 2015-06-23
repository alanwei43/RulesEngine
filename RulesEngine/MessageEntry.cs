using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine
{
    public class MessageEntry
    {
        Type _type;
        EquatableExpression _expression;
        IRule _rule;
        Func<string> _message;

        /// <summary>
        /// Gets Type
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets Expression
        /// </summary>
        public EquatableExpression Expression
        {
            get { return _expression; }
        }

        /// <summary>
        /// Gets Rule
        /// </summary>
        public IRule Rule
        {
            get { return _rule; }
        }

        /// <summary>
        /// Gets Message
        /// </summary>
        public Func<string> Message
        {
            get { return _message; }
        }

        public MessageEntry(Type type, EquatableExpression expression, IRule rule, Func<string> message)
        {
            //Type cannot be null, others can however.
            if (type == null) throw new System.ArgumentNullException("type");

            _type = type;
            _expression = expression;
            _rule = rule;
            _message = message;
        }

        public override string ToString()
        {
            return string.Format("'{0}', {1}, {2}, {3}", _message(), _expression, _type, _rule);
        }
    }
}
