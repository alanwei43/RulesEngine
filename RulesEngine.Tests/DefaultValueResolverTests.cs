using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RulesEngine.Tests
{
    [TestClass]
    public class DefaultValueResolverTests
    {
        private class Foo
        {
            public Dictionary<string, object> Dictionary = new Dictionary<string, object>();
            public object[] Array;
            public Foo Composition;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldNotAcceptNullArgument()
        {
            new DefaultValueResolver<int, int>(null);
        }

        [TestMethod]
        public void ShouldResolveValueFromDictionary()
        {
            var foo = new Foo();
            var obj = new object();
            foo.Dictionary["test1"] = obj;

            var resolver = new DefaultValueResolver<Foo, object>(f => f.Dictionary["test1"]);
            object result;
            Assert.IsTrue(resolver.TryGetValue(foo, out result));
            Assert.AreSame(obj, result);
        }

        [TestMethod]
        public void ShouldIgnoreKeyNotFoundExceptions()
        {
            var foo = new Foo();
            var obj = new object();
            foo.Dictionary["test1"] = obj;

            var resolver = new DefaultValueResolver<Foo, object>(f => f.Dictionary["test2"]);
            object result;
            Assert.IsFalse(resolver.TryGetValue(foo, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldResolveValueFromArray()
        {
            var foo = new Foo();
            var obj = new object();
            foo.Array = new[] { obj };

            var resolver = new DefaultValueResolver<Foo, object>(f => f.Array[0]);
            object result;
            Assert.IsTrue(resolver.TryGetValue(foo, out result));
            Assert.AreSame(obj, result);
        }

        [TestMethod]
        public void ShouldIgnoreIndexOutOfRange()
        {
            var foo = new Foo();
            var obj = new object();
            foo.Array = new[] { obj };

            var resolver = new DefaultValueResolver<Foo, object>(f => f.Array[1]);

            object result;
            Assert.IsFalse(resolver.TryGetValue(foo, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldResolveValueFromComposedExpression()
        {
            var foo = new Foo();
            var obj = new Foo();
            foo.Composition = obj;

            var resolver = new DefaultValueResolver<Foo, object>(f => f.Composition.Dictionary);
            object result;
            Assert.IsTrue(resolver.TryGetValue(foo, out result));
            Assert.AreSame(obj.Dictionary, result);
        }

        [TestMethod]
        public void ShouldIgnoreNullReferenceExceptions()
        {
            var foo = new Foo();
            foo.Composition = null;

            var resolver = new DefaultValueResolver<Foo, object>(f => f.Composition.Dictionary);

            object result;
            Assert.IsFalse(resolver.TryGetValue(foo, out result));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldImplementIValueResolver()
        {
            var foo = new Foo();
            var obj = new Foo();
            foo.Composition = obj;

            var resolver = new DefaultValueResolver<Foo, object>(f => f.Composition.Dictionary);
            object result;
            Assert.IsTrue(((IValueResolver)resolver).TryGetValue(foo, out result));
            Assert.AreSame(obj.Dictionary, result);
        }

    }
}
