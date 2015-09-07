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
    public class InheritanceTests
    {
        private class ClassA
        {
            public int ParamA { get; set; }
        }
        private class ClassB : ClassA
        {
            public int ParamB { get; set; }
        }
        private class ClassC : ClassB
        {
            public int ParamC { get; set; }
        }

        Engine _re = new Engine();

        [TestInitialize]
        public void TestInitialize()
        {
            _re.For<ClassA>()
                .Setup(a => a.ParamA)
                    .MustBeLessThan(1000);
            _re.For<ClassB>()
                .Setup(b => b.ParamB)
                    .MustBeLessThan(1000);
            _re.For<ClassC>()
                    .Setup(c => c.ParamC)
                        .MustBeLessThan(1000)
                    .Setup(c => c.ParamA)
                        .MustBeLessThan(250);           
        }

        [TestMethod]
        public void ShouldInheritRules()
        {
            ClassB b = new ClassB() { ParamA = 2000, ParamB = 999};
            Assert.IsFalse(_re.Validate(b), "Expected 'b' to be invalid because inherited ParamA is not valid.");
        }

        [TestMethod]
        public void ShouldInheritRules_2()
        {
            ClassC c = new ClassC() { ParamA = 999, ParamB = 999, ParamC=999 };
            Assert.IsFalse(_re.Validate(c), "Expected 'c' to be invalid because inherited ParamA is not valid.");
        }
    }
}
