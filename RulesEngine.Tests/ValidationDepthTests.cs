using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine;
using System.Collections.ObjectModel;
using RulesEngine.Rules;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class ValidationDepthTests
    {
        IEngine _en1;
        Fluent.FluentBuilder _builder1;
        private class Club
        {
            public Member President { get; set; }
            public IEnumerable<Member> Members { get; set; }
            public Club(Member president, params Member[] members)
            {
                this.President = president;
                this.Members = members;
            }
        }
        private class Member
        {
            public string Name { get; set; }
            public Member(string name)
            {
                this.Name = name;
            }
        }

        public ValidationDepthTests()
        {
            _builder1 = new Fluent.FluentBuilder();
            _builder1.For<Member>()
                .Setup(m => m.Name)
                    .MustNotBeNull()
                .EndSetup();
            _builder1.For<Club>()
                .Setup(c => c.President)
                    .MustNotBeNull()
                    .CallValidate()
                .Setup(c => c.Members)
                    .MustNotBeNull()
                    .CallValidateForEachElement()
            ;

            _en1 = _builder1.Build();

        }

        [TestMethod]
        public void ShouldValidateAllItemsInEnumeration()
        {
            var member1 = new Member(null);
            var member2 = new Member(null);
            var club = new Club(new Member("president"), member1, member2);
            var report = new ValidationReport();
            _en1.Validate(club, report, ValidationReportDepth.FieldShortCircuit);

            var memberNameExp = new EquatableExpression(ExpressionHelper.New<Member, string>(mm => mm.Name));
            Assert.IsTrue(report.HasError(member1, memberNameExp), "Expected validation failure for member1. Name was null...");
            Assert.IsTrue(report.HasError(member2, memberNameExp), "Expected validation failure for member2. Name was null...");
        }

        [TestMethod]
        public void ShouldShortCircuit()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<Member>()
                .MustPassGenericRule(m => { throw new AssertFailedException("Should have short-circuited."); });
            var en2 = builder.Build(_builder1);

            var member1 = new Member(null);
            var memberNameExp = ExpressionHelper.New<Member, string>(mm => mm.Name);
            var report = new ValidationReport();

            //Only short-circuit here would work...Others would throw exception...
            _en1.Validate(member1, report, ValidationReportDepth.ShortCircuit);
        }

        [TestMethod]
        public void ShouldSkipValidationWhenShortCircuiting()
        {
            var president = new Member(null);
            var club = new Club(president);
            var report = new ValidationReport();
            var memberNameExp = new EquatableExpression(ExpressionHelper.New<Member, string>(mm => mm.Name));
            var presidentExp = new EquatableExpression(ExpressionHelper.New<Club, Member>(cc => cc.President));

            //Report an error on the President expression...
            report.AddError(new ValidationError(new EqualRule<int>(2), presidentExp, new object[0], club, club, presidentExp));
            _en1.Validate(club, report, ValidationReportDepth.FieldShortCircuit);
            //because an error has already been reported on the club's president, the member's name rule would not have been invoked
            Assert.IsFalse(report.HasError(president, memberNameExp));

            //Now try this again without short-circuiting
            _en1.Validate(club, report, ValidationReportDepth.All);
            Assert.IsTrue(report.HasError(president, memberNameExp));
        }


    }
}
