using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;
using RulesEngine.Fluent;

namespace RulesEngine
{
    public interface IEngineBuilder
    {
        IEngine Build();
    }
}
