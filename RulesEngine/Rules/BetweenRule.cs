using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Rules
{
    public enum BetweenRuleBoundsOption
    {
        BothInclusive,
        LowerInclusiveUpperExclusive,
        LowerExclusiveUpperInclusive,
        BothExclusive
    }

    public class BetweenRule<T, R> : IRule<T>
        where R : IComparable<R>
    {
        private Func<T, R> _greaterThan;
        private Func<T, R> _lessThan;
        private Func<T, R> _value;
        private int _compareToResultLower;
        private int _compareToResultUpper;
        private BetweenRuleBoundsOption _options;

        public BetweenRule(Expression<Func<T, R>> value, Expression<Func<T, R>> greaterThan, Expression<Func<T, R>> lessThan, BetweenRuleBoundsOption options)
        {
            if (lessThan == null) throw new ArgumentNullException("lessThan");
            if (greaterThan == null) throw new ArgumentNullException("greaterThan");
            if (value == null) throw new ArgumentNullException("value");
            _value = value.Compile();
            _greaterThan = greaterThan.Compile();
            _lessThan = lessThan.Compile();
            _options = options;
            Initialize();
        }

        private void Initialize()
        {
            if (_options == BetweenRuleBoundsOption.BothInclusive)
            {
                _compareToResultLower = 1;
                _compareToResultUpper = -1;
            }
            else if (_options == BetweenRuleBoundsOption.LowerInclusiveUpperExclusive)
            {
                _compareToResultLower = 1;
                _compareToResultUpper = 0;
            }
            else if (_options == BetweenRuleBoundsOption.LowerExclusiveUpperInclusive)
            {
                _compareToResultLower = 0;
                _compareToResultUpper = -1;
            }
            else
            {
                _compareToResultLower = 0;
                _compareToResultUpper = 0;
            }
        }

        public ValidationResult Validate(T value)
        {
            R v = _value(value);
            IComparable<R> lowerBound = _greaterThan(value);
            IComparable<R> upperBound = _lessThan(value);

            if (lowerBound.CompareTo(v) < _compareToResultLower && upperBound.CompareTo(v) > _compareToResultUpper)
                return ValidationResult.Success;
            else
                return ValidationResult.Fail(lowerBound, upperBound, v, _options);
        }

        public string RuleKind
        {
            get
            {
                return RuleKinds.BetweenRule;
            }
        }

    }
}
