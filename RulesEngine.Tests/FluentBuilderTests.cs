using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class FluentBuilderTests
    {
        [TestMethod]
        public void ShouldCreateEngine()
        {
            var builder = new FluentBuilder();
            builder.For<Foo>()
                    .Setup(m => m.Value)
                        .MustBeGreaterThan(10);

            var engine = builder.Build();
            Assert.IsTrue(engine.Validate(new Foo(11)));
            Assert.IsFalse(engine.Validate(new Foo(9)));

        }

        [TestMethod]
        public void NodeTests()
        {
            var builder = new FluentBuilder();
            builder.For<Foo>()
                .Setup(m => m.Value)
                .Setup(m => m.Value)
                .EndSetup();
        }
    }
}
