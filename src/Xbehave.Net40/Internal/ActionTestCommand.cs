﻿// <copyright file="ActionTestCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit.Sdk;

    internal class ActionTestCommand : TestCommand
    {
        private readonly Action test;

        public ActionTestCommand(IMethodInfo method, string name, int timeout, Action test)
            : base(method, name, timeout)
        {
            this.test = test;
        }

        public override MethodResult Execute(object testClass)
        {
            this.test();
            return new PassedResult(this.testMethod, DisplayName);
        }
    }
}