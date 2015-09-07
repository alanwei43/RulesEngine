using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine;
using System.Collections.ObjectModel;

namespace RulesEngine.Tests
{
    [TestClass]
    public class EnumerableCompositionTests
    {
        private class Foo
        {
            public int Prop1 { get; set; }
            public Foo(int prop1)
            {
                this.Prop1 = prop1;
            }
        }
        private class Container
        {
            List<Foo> _foos = new List<Foo>();

            /// <summary>
            /// Gets Foos
            /// </summary>
            public List<Foo> Foos
            {
                get { return _foos; }
            }

            public Container(params Foo[] foos)
            {
                _foos.AddRange(foos);
            }
        }
        private Engine _re;

        [TestInitialize]
        public void TestInitialize()
        {
            _re = new Engine();
            _re.For<Foo>()
                .Setup(f => f.Prop1)
                    .MustBeLessThan(10);
            _re.For<Container>()
                    .Setup(c => c.Foos)
                        .MustNotBeNull()
                        .CallValidateForEachElement();
            
        }

        [TestMethod]
        public void EnumerableTest1()
        {
            Container container = new Container(new Foo(1), new Foo(2), new Foo(3));
            Assert.IsTrue(_re.Validate(container));
            container.Foos.Add(new Foo(11));
            Assert.IsFalse(_re.Validate(new Foo(11)), "Expected Foo to be invalid.");
            Assert.IsFalse(_re.Validate(container), "Expected Container to be invalid because one Foo is invalid.");
        }

        [TestMethod]
        public void ShouldValidateUsingOtherRulesEngine()
        {
            Engine newEngine = new Engine();
            newEngine.For<Foo>()
                        .Setup(f => f.Prop1)
                            .MustEqual(8);
            _re.For<Container>()
                    .Setup(c => c.Foos)
                        .CallValidateForEachElement(newEngine);
                    

            Container container = new Container(new Foo(8), new Foo(8), new Foo(8));
            Assert.IsTrue(_re.Validate(container));

            Assert.IsFalse(_re.Validate(new Foo(11)), "Expected Foo to be invalid.");
            //_re has Foo rules (less than 10). MustEqual 8 only applies to the Foo'S collection (because of newEngine).
            Assert.IsTrue(_re.Validate(new Foo(7)), "Expected Foo to be valid.");

            container.Foos.Add(new Foo(9));
            Assert.IsFalse(_re.Validate(container), "Expected Container to be invalid because one Foo is invalid.");
        }
    }
}
