using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.WebApi.CrudController;

namespace Ploch.Common.Data.CommandQuery.Tests;

public class CommandInterfaceHierarchyTests
{
    [Theory]
    [AutoMockData]
    public void ICommand_Execute_should_be_called(bool expectedResult)
    {
        var command = new TestCommandNoOutput(expectedResult);
        var (success, data, errors) = command.Execute("test");

        success.Should().Be(expectedResult);

        data.Should().BeEmpty();

        ICommand commandInterface = command;

        command.Execute("test").Should().BeEquivalentTo(commandInterface.Execute("test"));

        var commandWithOutput = new TestCommandWithOutput();
        var (additionalData, success2) = commandWithOutput.Execute("test");

        additionalData.Should().Be("test");
        success2.Should().BeFalse();

        ICommand commandWithOutputInterface = commandWithOutput;
        commandWithOutput.Execute("test").Should().BeEquivalentTo(commandWithOutputInterface.Execute("test"));
    }

    public record TestCommandResult : CommandResult
    {
        public TestCommandResult(string SomeAdditionalData, bool success) : base(success)
        {
            this.SomeAdditionalData = SomeAdditionalData;
            this.success = success;
            Data = new Dictionary<string, object> { { "SomeAdditionalData", SomeAdditionalData } };
        }

        public string SomeAdditionalData { get; init; }

        public bool success { get; init; }

        public void Deconstruct(out string SomeAdditionalData, out bool success)
        {
            SomeAdditionalData = this.SomeAdditionalData;
            success = this.success;
        }
    }

    public class TestCommandNoOutput : Command<string>
    {
        private readonly bool _expectedResult;

        public TestCommandNoOutput(bool expectedResult)
        {
            _expectedResult = expectedResult;
        }

        public override CommandResult Execute(string input)
        {
            return new CommandResult(_expectedResult);
        }
    }

    public class TestCommandWithOutput : Command<string, TestCommandResult>
    {
        public override TestCommandResult Execute(string input)
        {
            return new TestCommandResult("test", false);
        }
    }
}