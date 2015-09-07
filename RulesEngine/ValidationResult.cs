using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public struct ValidationResult
    {
        private bool _isValid;
        private object[] _arguments;

        /// <summary>
        /// Gets IsValid
        /// </summary>
        public bool IsValid
        {
            get { return _isValid; }
        }

        /// <summary>
        /// Gets Arguments
        /// </summary>
        public object[] Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Successfull result for a validation.
        /// </summary>
        public static ValidationResult Success
        {
            get { return _valid; }
        }

        public ValidationResult(bool isValid, object[] arguments)
        {
            _isValid = isValid;
            _arguments = arguments;
        }

        private static readonly ValidationResult _valid = new ValidationResult(true, null);

        public static ValidationResult Fail(params object[] arguments)
        {
            return new ValidationResult(false, arguments);
        }
        public static ValidationResult Fail()
        {
            return new ValidationResult(false, new object[0]);
        }
    }
}
