using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulesEngine.Fluent
{
    internal class FluentScopeAction : IFluentScopeValue
    {
        Action<FluentScope, FluentBuilderToken> _action;

        public void Execute(FluentScope scope, FluentBuilderToken token)
        {
            _action(scope, token);
        }

        public FluentScopeAction(Action<FluentScope, FluentBuilderToken> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _action = action;
        }

        public object Copy()
        {
            //NOTE: FluentScopeAction are NEVER copied to the child scopes!
            return null;
        }
    }
}
