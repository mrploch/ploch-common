using Xunit;
using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Ardalis.GuardClauses.Tests
{
    
    public class CustomConditionGuardTests
    {
        [Fact(DisplayName = "ArgCondition should throw ArgumentException when condition is met and print message")]
        public void ArgCondition_should_throw_ArgumentException_when_condition_is_met_and_print_message()
        {
            int arg1 = 5;
            Guard.Against.Invoking(y => y.ArgCondition(arg1 != 4, nameof(arg1), "Expected 4"))
                 .Should()
                 .Throw<ArgumentException>()
                 .Where(ex => ex.Message.Contains(nameof(arg1)) && ex.Message.Contains("Expected 4"));

        }

        [Fact()]
        public void ArgCondition_should__throw_ArgumentException_when_expression_condition_is_met_and_print_default_message()
        {
            int arg1 = 5;
            Guard.Against.Invoking(y => y.ArgCondition(() => arg1 != 4, nameof(arg1)))
                 .Should()
                 .Throw<ArgumentException>()
                 .Where(ex => ex.Message.Contains(nameof(arg1)) && ex.Message.Contains("!=") && ex.Message.Contains("4"));
        }

        [Fact()]
        public void OperationCondition_should_not_throw_when_condition_is_not_met()
        {
            int arg1 = 4;
            Guard.Against.Invoking(y => y.ArgCondition(arg1 != 4, nameof(arg1), "Expected 4"))
                 .Should()
                 .NotThrow();
        }

        [Fact()]
        public void OperationCondition_should_not_throw_when_expression_condition_is_not_met()
        {
            int arg1 = 4;
            Guard.Against.Invoking(y => y.ArgCondition(() => arg1 != 4, nameof(arg1)))
                 .Should()
                 .NotThrow();
        }
    }
}