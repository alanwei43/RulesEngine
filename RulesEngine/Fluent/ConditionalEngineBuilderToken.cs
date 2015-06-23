using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulesEngine.Fluent
{
    internal class ConditionalEngineBuilderToken
    {
        private readonly IEngine _ifTrueEngine;
        private readonly IEngine _ifFalseEngine;
        private IEngine _currentEngine;

        /// <summary>
        /// Gets TrueEngine
        /// </summary>
        public IEngine IfTrueEngine
        {
            get { return _ifTrueEngine; }
        }

        /// <summary>
        /// Gets FalseEngine
        /// </summary>
        public IEngine IfFalseEngine
        {
            get { return _ifFalseEngine; }
        }

        /// <summary>
        /// Gets or Sets CurrentEngine
        /// </summary>
        public IEngine CurrentEngine
        {
            get { return _currentEngine; }
            set { _currentEngine = value; }
        }

        public ConditionalEngineBuilderToken(IEngine ifTrueEngine, IEngine ifFalseEngine, IEngine currentEngine)
        {
            _ifTrueEngine = ifTrueEngine;
            _ifFalseEngine = ifFalseEngine;
            _currentEngine = currentEngine;
        }

    }
}
