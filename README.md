# Modules
Library for working with modules. Define services and module dependencies in modules, then register them in DI, this will register them in the correct topological order and avoid double registration
#### Nuget:
* https://www.nuget.org/packages/ap.Modules/
## Usage
```cssharp
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
```
