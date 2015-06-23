using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Tests
{
    public class Foo
    {
        private int _value;

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Foo(int value)
        {
            _value = value;
        }
    }
}
