﻿// <copyright file="StepTimeoutFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if !V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
#if NET40 || NET45
    using System.Threading;
#endif
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    // In order to prevent very long running tests <-- improve!
    // As a developer
    // I want a feature to fail if a given step takes to long to run
    public class StepTimeoutFeature : Feature
    {
#if NET40 || NET45
        private static readonly ManualResetEventSlim @event = new ManualResetEventSlim();
#endif

        [Scenario]
        public void StepExecutesFastEnough()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a feature with a scenario with a single step which does not exceed it's timeout"
                .Given(() => feature = typeof(StepFastEnough));

            "When I run the scenarios"
                .When(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a pass"
                .And(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

#if NET40 || NET45
        [Scenario]
        public void StepExecutesTooSlowly()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a feature with a scenario with a single step which exceeds it's 1ms timeout"
                .Given(() => feature = typeof(StepTooSlow));

            "When I run the scenarios"
                .When(() =>
                {
                    @event.Reset();
                    results = this.Run<ITestResultMessage>(feature);
                })
                .Teardown(() => @event.Set());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<ITestFailed>());

            "And the result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<ITestFailed>().Should().OnlyContain(result =>
                    result.Messages.Single() == "Test execution time exceeded: 1ms"));
        }
#endif

        private static class StepFastEnough
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { })
                    .WithTimeout(int.MaxValue);
            }
        }

#if NET40 || NET45
        private static class StepTooSlow
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => @event.Wait())
                    .WithTimeout(1);
            }
        }
#endif
    }
}
#endif
