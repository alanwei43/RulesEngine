using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine
{
    public static class MessageHelper
    {
        public static M WithMessage<M, T, R>(this IMustPassRule<M, T, R> mpr, string message)
        {
            if (message == null) throw new System.ArgumentNullException("message");
            return WithMessage(mpr, () => message);
        }
        public static M WithMessage<M, T, R>(this IMustPassRule<M, T, R> mpr, Func<string> message)
        {
            if (message == null) throw new System.ArgumentNullException("message");
            //NOTE: Use the rulesEngine must be the original - i.e. Setting Messages on nested If's RulesEngine.
            var rulesEngine = mpr.RulesEngine.Original;
            rulesEngine.DefaultErrorResolver.SetErrorMessageFor(typeof(T), new EquatableExpression(mpr.Expression), mpr.LastRule, message);
            return mpr.GetSelf();
        }
    }
}
