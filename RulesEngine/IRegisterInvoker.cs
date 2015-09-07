using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace RulesEngine
{
    public interface IRegisterInvoker
    {
        void RegisterInvoker(IRuleInvoker ruleInvoker);
        Engine RulesEngine { get; }
    }
}
