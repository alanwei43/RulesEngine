using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class FluentScopeTests
    {
        [TestMethod]
        public void ShouldReturnPrimitive()
        {
            var bag = CreateScope();
            bag.Set(CreateKey<int>("lala"), 123);
            Assert.AreEqual(123, bag.Get(CreateKey<int>("lala")));
            Assert.AreEqual(123, bag.Get(CreateKey<Int32>("lala")));
            Assert.AreEqual(123, bag.Get(CreateKey<object>("lala")));
        }

        [TestMethod]
        public void ShouldReturnReference()
        {
            var bag = CreateScope();
            var value = new object();
            bag.Set(CreateKey<object>("lala"), value);
            Assert.AreSame(value, bag.Get(CreateKey<object>("lala")));
            Assert.AreSame(value, bag.Get(CreateKey<object>("lala")));
        }

        private FluentScope CreateScope()
        {
            return new FluentScope();
        }

        private IFluentScopeKey<T> CreateKey<T>(string key)
        {
            var result = new Key<T>(key);
            return result;
        }

        private class Key<T> : IFluentScopeKey<T>
        {
            string _key;
            string IFluentScopeKey<T>.Key
            {
                get { return _key; }
            }

            public Key(string key)
            {
                _key = key;
            }

        }



    }
}
