using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class IFluentNodeConformityTests
    {
        [TestMethod]
        public void AllFluentNodesShouldDeclareAInternalConstructor()
        {
            //BuilderHelper.CreateFluentNode assumes all implementors of IFluentNode have an internal constructor with a single IFluentScope parameter.
            //Hence this unit-test.
            var fluentNodeTypes = typeof(Engine).Assembly.GetTypes().Where(t => t.IsClass && typeof(IFluentNode).IsAssignableFrom(t));
            const BindingFlags bindingFlag = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


            foreach (var fluentNodeType in fluentNodeTypes)
            {
                var constructor = fluentNodeType.GetConstructor(bindingFlag, null, new[] { typeof(FluentScope) }, null);
                Assert.IsNotNull(constructor, "Cannot find appropriate constructor for type {0}", fluentNodeType);
            }
        }
    }
}
