# DBTools

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://gitlab.aiursoft.cn/aiursoft/DbTools/-/blob/master/LICENSE)
[![Pipeline stat](https://gitlab.aiursoft.cn/aiursoft/DbTools/badges/master/pipeline.svg)](https://gitlab.aiursoft.cn/aiursoft/DbTools/-/pipelines)
[![Test Coverage](https://gitlab.aiursoft.cn/aiursoft/DbTools/badges/master/coverage.svg)](https://gitlab.aiursoft.cn/aiursoft/DbTools/-/pipelines)
[![NuGet version (Aiursoft.CSTools)](https://img.shields.io/nuget/v/Aiursoft.DbTools.svg)](https://www.nuget.org/packages/Aiursoft.DbTools/)
[![ManHours](https://manhours.aiursoft.cn/r/gitlab.aiursoft.cn/aiursoft/dbtools.svg)](https://gitlab.aiursoft.cn/aiursoft/dbtools/-/commits/master?ref_type=heads)

DbTools are Aiursoft's common database tools. It contains a lot of useful database tools for developers.

## Installation

To install `Aiursoft.DbTools` to your project from [nuget.org](https://www.nuget.org/packages/Aiursoft.DbTools/):

```bash
dotnet add package Aiursoft.DbTools
```

## Usage

Easier to register DbContext:

SQLite

```csharp
var services = new ServiceCollection();
services.AddAiurSqliteWithCache<MyDbContext>("Data Source=app.db");

var built = services.BuildServiceProvider();
var context = built.GetRequiredService<MyDbContext>();
```

SQL Server

```csharp
var services = new ServiceCollection();
services.AddAiurSqlServerWithCache<MyDbContext>("Server=(localdb)\\mssqllocaldb;Database=DebugTrusted_Connection=True;MultipleActiveResultSets=true");

var built = services.BuildServiceProvider();
var context = built.GetRequiredService<MyDbContext>();
```

Easier to update database:

```csharp
var hostBuilder = Host.CreateDefaultBuilder();
hostBuilder.ConfigureServices(services => 
    services.AddAiurSqliteWithCache<MyDbContext>(@"DataSource=app.db;Cache=Shared")
);
var host = hostBuilder.Build();

// Now update:
await host.UpdateDbAsync<MyDbContext>(UpdateMode.CreateThenUse);
```

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you with push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
