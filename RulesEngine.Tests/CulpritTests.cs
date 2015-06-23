using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;
using RulesEngine.Rules;

namespace RulesEngine.Tests
{
    [TestClass]
    public class CulpritTests
    {
        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Address HomeAddress { get; set; }
        }
        private class Address
        {
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string PostCode { get; set; }
            public string Suburb { get; set; }
        }

        [TestMethod]
        public void ShouldBlameExplicitlySelectedCulprit_SameCulpritObject()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<Person>()
                    .Setup(p => p.FirstName)
                        .Blame(p => p.HomeAddress.Line2)
                        .MustBeOneOf("John", "Paul", "James");

            var engine = builder.Build();
            var report = new TestingValidationReport(engine);
            var address = MakeAddress("18 Perkins St", null, "1234", "Brisbane");
            var person = MakePerson("Jane", "Holland", address);
            report.Validate(person);
            report.AssertError(person, m => m.HomeAddress.Line2, RuleKinds.OneOfRule, null);
        }


        [TestMethod]
        public void ShouldBlameExplicitlySelectedCulprit_DifferentCulpritObject()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<Person>()
                    .Setup(p => p.FirstName)
                        .Blame(p => p.HomeAddress, a => a.Line1)
                        .MustBeOneOf("John", "Paul", "James");

            var engine = builder.Build();
            var report = new TestingValidationReport(engine);
            var address = MakeAddress("18 Perkins St", null, "1234", "Brisbane");
            report.Validate(MakePerson("Jane", "Holland", address));
            report.AssertError(address, m => m.Line1, RuleKinds.OneOfRule, null);
        }

        [TestMethod]
        public void ShouldBlameExplicitlySelectedCulprit_BlameIsAfterTheRule()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<Person>()
                    .Setup(p => p.FirstName)
                        .MustBeOneOf("John", "Paul", "James")
                        .Blame(p => p.HomeAddress, a => a.Line1);

            var engine = builder.Build();
            var report = new TestingValidationReport(engine);
            var address = MakeAddress("18 Perkins St", null, "1234", "Brisbane");
            report.Validate(MakePerson("Jane", "Holland", address));
            report.AssertError(address, m => m.Line1, RuleKinds.OneOfRule, null);
        }

        [TestMethod]
        public void ShouldBlameExplicitlySelectedCulprit_DifferentCulpritForDifferentRules()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<Person>()
                    .Setup(p => p.FirstName)
                        .MustNotEqual("Paul")
                        .Blame(p => p.HomeAddress, a => a.Line1)
                        .MustNotEqual("Peter")
                        .Blame(p => p.HomeAddress, a => a.PostCode);

            var engine = builder.Build();
            var report = new TestingValidationReport(engine);
            var address = MakeAddress("18 Perkins St", null, "1234", "Brisbane");
            report.Validate(MakePerson("Paul", "Holland", address));
            report.AssertError(address, m => m.Line1, RuleKinds.NotEqualRule, null);

            report.Clear();
            report.Validate(MakePerson("Peter", "Holland", address));
            report.AssertError(address, m => m.PostCode, RuleKinds.NotEqualRule, null);

        }

        //TODO: Blame Culprit when using composition!
        //[TestMethod]
        public void ShouldBlameExplicitlySelectedCulprit_Composition()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<Person>()
                    .Setup(p => p.HomeAddress)
                        .CallValidate()
                        .Blame(p => p.FirstName);

            builder.For<Address>()
                    .Setup(a => a.Line1)
                        .MustEqual("Line1");
                    


            var engine = builder.Build();
            var report = new TestingValidationReport(engine);
            var address = MakeAddress("Not Line 1", null, "1234", "Brisbane");
            var person = MakePerson("Blinky", "Bill", address);
            report.Validate(person);
            report.AssertError(person, m => m.FirstName, RuleKinds.EqualRule, "Line1");
        }



        private Person MakePerson(string firstName, string lastName, Address address)
        {
            return new Person { FirstName = firstName, LastName = lastName, HomeAddress = address };
        }

        private Address MakeAddress(string line1, string line2, string postCode, string suburb)
        {
            return new Address { Line1 = line1, Line2 = line2, PostCode = postCode, Suburb = suburb };
        }


    }
}
