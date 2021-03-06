﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Ploch.Common.ConsoleApplication.Core;
using Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes;
using Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes.VerbApps;
using Ploch.TestingSupport.FluentAssertions;
using Ploch.TestingSupport.Xunit.AutoFixture;
using Xunit;

namespace Ploch.Common.ConsoleApplication.Runner.Tests
{
    

    public class AppBootstrapperTests
    {
        [Theory]
        [AutoDataMoq]
        public void ExecuteApp_should_resolve_AppEvents_and_execute_OnStartup(IAppEvents appEventsMock)
        {
            var bootstrapper = new AppBootstrapper();
            var commandLine = "--Prop1Str str1";

            bootstrapper.ExecuteApp<ImmutableArgsApp1, ImmutableArgs1>(commandLine.Split(" "));

            ImmutableArgsApp1.Args.Should().NotBeNull();
            ImmutableArgsApp1.ExecuteCallCount.Should().Be(1);
        }

        [Fact]
        public void ExecuteApp_should_be_able_to_parse_args_with_immutable_properties()
        {
            var bootstrapper = new AppBootstrapper();
            var commandLine = "--Prop1Str str1";

            bootstrapper.ExecuteApp<ImmutableArgsApp1, ImmutableArgs1>(commandLine.Split(" "));

            ImmutableArgsApp1.Args.Should().NotBeNull();
            ImmutableArgsApp1.ExecuteCallCount.Should().Be(1);
        }

        [Fact]
        public void ExecuteApp_should_report_meaningfull_error_if_custom_service_provider_is_null()
        {
            var bootstrapper = new AppBootstrapper(services => null, new DefaultServices());

            var commandLine = "--Prop1Str str1";


            bootstrapper.Invoking(b => b.ExecuteApp<ImmutableArgsApp1, ImmutableArgs1>(commandLine.Split(" ")))
                        .Should()
                        .Throw<InvalidOperationException>()
                        .Which.Message.Should().ContainAllEquivalentOf("service", "provider");
        }

        [Fact(DisplayName = "ExecuteApp should resolve argument types, choose correct app based on a verb and parse correct arguments")]
        public void ExecuteApps_resolves_argument_classes_and_executes_app_selected_by_verb_with_parsed_options()
        {
            var bootstrapper = new AppBootstrapper();
            var apps = new[] {typeof(App1SimpleArgs), typeof(App2SimpleArgs)};
            var commandLine = "app2 -a val1 -b -s val2";

            bootstrapper.ExecuteApp(commandLine.Split(" "), apps);

            App1SimpleArgs.ExecuteCallCount.Should().Be(0);
            App1SimpleArgs.Args.Should().BeNull();

            App2SimpleArgs.ExecuteCallCount.Should().Be(1);
            var args = App2SimpleArgs.Args;
            args.AnotherStringProperty.Should().Be("val1");
            args.BoolProperty1.Should().BeTrue();
            args.StringProperty1.Should().Be("val2");
        }

        
    }
}