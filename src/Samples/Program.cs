// MIT License
// 
// Copyright (c) 2020 Alexey Politov
// https://github.com/EmptyBucket/Modules
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules;

var serviceCollection = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .Build();
// this will register the modules in topological order, i.e.: FirstDomainModule, FirstAppModule, SecondDomainModule, SecondAppModule, HostModule
serviceCollection.AddRootModule<HostModule>(configuration);

public class FirstDomainModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // configure services
    }
}

public class SecondDomainModule : ModuleBase
{
}

public class FirstAppModule : ModuleBase
{
    public override void ConfigureDependencies(IModuleDependencyBuilder moduleDependencyBuilder)
    {
        moduleDependencyBuilder.DependOn<FirstDomainModule>();
    }
}

public class SecondAppModule : ModuleBase
{
    public override void ConfigureDependencies(IModuleDependencyBuilder moduleDependencyBuilder)
    {
        moduleDependencyBuilder
            .DependOn<FirstDomainModule>()
            .DependOn<SecondDomainModule>();
    }
}

public class HostModule : ModuleBase
{
    public override void ConfigureDependencies(IModuleDependencyBuilder moduleDependencyBuilder)
    {
        moduleDependencyBuilder
            .DependOn<FirstAppModule>()
            .DependOn<SecondAppModule>();
    }
}