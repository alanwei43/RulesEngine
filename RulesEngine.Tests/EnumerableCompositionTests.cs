using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine;
using RulesEngine.Fluent;
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
        private IEngine _re;

        [TestInitialize]
        public void TestInitialize()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<Foo>()
                .Setup(f => f.Prop1)
                    .MustBeLessThan(10);
            builder.For<Container>()
                    .Setup(c => c.Foos)
                        .MustNotBeNull()
                        .CallValidateForEachElement();

            _re = builder.Build();
            
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
            var builder = new Fluent.FluentBuilder();
            builder.For<Foo>()
                        .Setup(f => f.Prop1)
                            .MustEqual(8);
            var fooEngine = builder.Build();

            var builder2 = new Fluent.FluentBuilder();
            builder2.For<Container>()
                    .Setup(c => c.Foos)
                        .CallValidateForEachElement(fooEngine);

            var containerEngine = builder2.Build();

            Assert.IsTrue(fooEngine.Validate(new Foo(8)));
            Assert.IsFalse(fooEngine.Validate(new Foo(7)));

            Assert.IsTrue(containerEngine.Validate(new Container(new Foo(8), new Foo(8), new Foo(8))));
            //Should not be valid if any of the Foo's is invalid in the collection.
            Assert.IsFalse(containerEngine.Validate(new Container(new Foo(7), new Foo(8), new Foo(8))));
            Assert.IsFalse(containerEngine.Validate(new Container(new Foo(8), new Foo(7), new Foo(8))));
            Assert.IsFalse(containerEngine.Validate(new Container(new Foo(8), new Foo(8), new Foo(7))));

        }
    }
}
