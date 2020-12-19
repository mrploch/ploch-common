using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Runner.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Ploch.Common.ConsoleApplication.Runner.Tests
{
    public class ConsoleOutputTests
    {
        [Theory]
        [AutoData]
        public void WriteErrorLine_should_output_string_message_correctly(string msg1, string msg2, Guid guid1, DateTime dt1)
        {
            var collectedActions = new List<TextWriterEventArgs>();
            var exceptions = new List<Exception>();
            var eventfulWriter = new EventfulTextWriter();
            var writeEvents = Observable.FromEventPattern<EventHandler<TextWriterEventArgs>, TextWriterEventArgs>(h => eventfulWriter.WriteExecuted += h,
                h => { eventfulWriter.WriteExecuted -= h; }).Select(ev => ev.EventArgs);
            using var subscription = writeEvents.Subscribe(nextArgs => collectedActions.Add(nextArgs), exception => exceptions.Add(exception));

            var collectedErrorActions = new List<TextWriterEventArgs>();
            var exceptionsOnErrorWrite = new List<Exception>();
            var eventfulErrorWriter = new EventfulTextWriter();
            var writeErrorEvents = Observable.FromEventPattern<EventHandler<TextWriterEventArgs>, TextWriterEventArgs>(h => eventfulErrorWriter.WriteExecuted += h,
                h => { eventfulErrorWriter.WriteExecuted -= h; }).Select(ev => ev.EventArgs);
            using var subscriptionErrorWrite = writeErrorEvents.Subscribe(nextArgs => collectedErrorActions.Add(nextArgs), exception => exceptionsOnErrorWrite.Add(exception));

            //Console.SetOut(eventfulWriter);

            var console = new ConsoleOutput(eventfulWriter, eventfulErrorWriter);
            console.Write(msg1)
                   .WriteLine(msg2)
                   .Write("Format {0} {1}", guid1, dt1)
                   .WriteLine("Format Line {0} {1}", guid1, dt1)
                   .Write(guid1)
                   .WriteLine(dt1)
                   .Write(guid1, dt1);
            var collectedArray = collectedActions.ToArray();

            ValidateEntry(collectedArray, 0, WriteOperationType.Write, msg1);
            ValidateEntry(collectedArray, 1, WriteOperationType.WriteLine, msg2);
        }

        private void ValidateEntry(TextWriterEventArgs[] collectedArgs, int index, WriteOperationType operation, object expectedValue, params object[] args)
        {
            collectedArgs[index].Should().NotBeNull();
            collectedArgs[index].Value.Should().Should().NotBeNull();
            collectedArgs[index].OperationType.Should().Be(operation);
            collectedArgs[index].Value.Should().BeEquivalentTo(expectedValue);

        }

    }
}