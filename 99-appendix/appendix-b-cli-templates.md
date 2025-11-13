# Appendix B: .NET CLI 도구 템플릿

## 기본 콘솔 템플릿

### 1. 최소 콘솔 애플리케이션

```bash
dotnet new console -n MinimalCli
```

**Program.cs:**
```csharp
Console.WriteLine("Hello, World!");
```

### 2. 전통적 콘솔 애플리케이션

```bash
dotnet new console -n TraditionalCli --use-program-main
```

**Program.cs:**
```csharp
namespace TraditionalCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
```

## CLI 도구 템플릿

### 3. System.CommandLine 템플릿

**MyTool.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.CommandLine" Version="2.0.0-beta4.*" />
  </ItemGroup>
</Project>
```

**Program.cs:**
```csharp
using System.CommandLine;

var rootCommand = new RootCommand("My CLI tool");

var nameOption = new Option<string>(
    "--name",
    "Your name"
);

rootCommand.Options.Add(nameOption);

rootCommand.SetAction((name) =>
{
    Console.WriteLine($"Hello, {name}!");
}, nameOption);

return await rootCommand.Parse(args).InvokeAsync();
```

### 4. Generic Host 기반 워커 서비스

```bash
dotnet new worker -n MyWorker
```

**Program.cs:**
```csharp
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
```

**Worker.cs:**
```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
```

### 5. dotnet tool 템플릿

**MyGlobalTool.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>

    <!-- Tool 설정 -->
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>mytool</ToolCommandName>
    <PackageId>MyGlobalTool</PackageId>
    <Version>1.0.0</Version>
    <Authors>YourName</Authors>
    <Description>My awesome CLI tool</Description>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.CommandLine" Version="2.0.0-beta4.*" />
  </ItemGroup>
</Project>
```

**설치 및 사용:**
```bash
# 패킹
dotnet pack -c Release

# 로컬 설치
dotnet tool install -g --add-source ./bin/Release MyGlobalTool

# 사용
mytool --help
```

## 프로젝트 구조 템플릿

### 6. 구조화된 CLI 프로젝트

```
MyCli/
├── MyCli.csproj
├── Program.cs
├── Commands/
│   ├── BaseCommand.cs
│   ├── ProcessCommand.cs
│   └── AnalyzeCommand.cs
├── Services/
│   ├── IDataService.cs
│   └── DataService.cs
├── Models/
│   └── DataModel.cs
└── appsettings.json
```

**Program.cs:**
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCli.Commands;
using MyCli.Services;
using System.CommandLine;

var rootCommand = new RootCommand("MyCli");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IDataService, DataService>();

var processCommand = new ProcessCommand(builder.Services.BuildServiceProvider());
var analyzeCommand = new AnalyzeCommand(builder.Services.BuildServiceProvider());

rootCommand.AddCommand(processCommand);
rootCommand.AddCommand(analyzeCommand);

return await rootCommand.InvokeAsync(args);
```

## NuGet 패키지 배포 템플릿

### 7. NuGet 패키지로 배포

**nuget.config:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

**배포 스크립트:**
```bash
#!/bin/bash
# publish.sh

VERSION="1.0.0"
API_KEY="your-api-key"

dotnet pack -c Release /p:Version=$VERSION
dotnet nuget push bin/Release/MyTool.$VERSION.nupkg --api-key $API_KEY --source https://api.nuget.org/v3/index.json
```

## GitHub Actions CI/CD 템플릿

### 8. GitHub Actions 워크플로우

**.github/workflows/build.yml:**
```yaml
name: Build and Publish

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'

    - name: Restore
      run: dotnet restore

    - name: Build
      run: dotnet build -c Release --no-restore

    - name: Test
      run: dotnet test -c Release --no-build

    - name: Pack
      run: dotnet pack -c Release --no-build -o ./artifacts

    - name: Publish to NuGet
      run: dotnet nuget push ./artifacts/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

## Docker 배포 템플릿

### 9. Dockerfile

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "MyCli.dll"]
```

**사용:**
```bash
# 빌드
docker build -t mycli:1.0 .

# 실행
docker run --rm mycli:1.0 --help
docker run --rm -v $(pwd)/data:/data mycli:1.0 process --input /data/file.txt
```

## 빠른 시작 스크립트

### 10. init.sh - 프로젝트 초기화

```bash
#!/bin/bash
# init.sh - CLI 프로젝트 빠른 시작

PROJECT_NAME=$1

if [ -z "$PROJECT_NAME" ]; then
    echo "Usage: ./init.sh <project-name>"
    exit 1
fi

echo "Creating $PROJECT_NAME CLI project..."

# 프로젝트 생성
dotnet new console -n $PROJECT_NAME
cd $PROJECT_NAME

# 패키지 추가
dotnet add package System.CommandLine --prerelease
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Serilog.Extensions.Hosting
dotnet add package Serilog.Sinks.Console

# 디렉토리 구조 생성
mkdir -p Commands Services Models

# README 생성
cat > README.md << EOF
# $PROJECT_NAME

CLI tool built with .NET

## Install

\`\`\`bash
dotnet tool install -g $PROJECT_NAME
\`\`\`

## Usage

\`\`\`bash
$PROJECT_NAME --help
\`\`\`
EOF

echo "✓ Project created: $PROJECT_NAME"
echo "  cd $PROJECT_NAME && dotnet run"
```

**사용:**
```bash
chmod +x init.sh
./init.sh MyCoolTool
```
