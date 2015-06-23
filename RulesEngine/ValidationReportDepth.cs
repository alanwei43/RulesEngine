using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public enum ValidationReportDepth
    {
        /// <summary>
        /// Only report the first encountered error for an Expression.
        /// </summary>
        FieldShortCircuit,

        
        /// <summary>
        /// Stops reporting at the first error encountered.
        /// </summary>
        ShortCircuit,

        /// <summary>
        /// Test all Validation Rules.
        /// </summary>
        All
    }
}
