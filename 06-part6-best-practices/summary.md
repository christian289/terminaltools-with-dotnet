# Part VI: 실전 패턴과 베스트 프랙티스 - 요약

## Chapter 13: 실전 디자인 패턴

### 13.1 Command 패턴

```csharp
using System.IO;
using System.Threading.Tasks;

interface ICommand
{
    Task ExecuteAsync();
}

class CopyFileCommand : ICommand
{
    private readonly string source, dest;

    public CopyFileCommand(string source, string dest)
    {
        this.source = source;
        this.dest = dest;
    }

    public async Task ExecuteAsync()
    {
        await using var sourceStream = File.OpenRead(source);
        await using var destStream = File.Create(dest);
        await sourceStream.CopyToAsync(destStream);
    }
}

// 실행
ICommand command = new CopyFileCommand("a.txt", "b.txt");
await command.ExecuteAsync();
```

### 13.2 Chain of Responsibility (파이프라인)

```csharp
using System.Threading.Tasks;

interface IPipelineStep
{
    Task<string> ProcessAsync(string input);
}

class UpperCaseStep : IPipelineStep
{
    public Task<string> ProcessAsync(string input) =>
        Task.FromResult(input.ToUpper());
}

class TrimStep : IPipelineStep
{
    public Task<string> ProcessAsync(string input) =>
        Task.FromResult(input.Trim());
}

// 파이프라인 실행
var pipeline = new[] {
    new TrimStep(),
    new UpperCaseStep()
};

var result = await pipeline.AggregateAsync(input, async (current, step) =>
    await step.ProcessAsync(current));
```

### 13.3 Strategy 패턴 (출력 포맷터)

```csharp
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

interface IOutputFormatter
{
    string Format(object data);
}

class JsonFormatter : IOutputFormatter
{
    public string Format(object data) =>
        JsonSerializer.Serialize(data);
}

class XmlFormatter : IOutputFormatter
{
    public string Format(object data)
    {
        var serializer = new XmlSerializer(data.GetType());
        using var writer = new StringWriter();
        serializer.Serialize(writer, data);
        return writer.ToString();
    }
}

// 사용
IOutputFormatter formatter = format switch
{
    "json" => new JsonFormatter(),
    "xml" => new XmlFormatter(),
    _ => throw new ArgumentException()
};

Console.WriteLine(formatter.Format(data));
```

## Chapter 14: 도구 간 연동과 자동화

### 14.1 JSON, XML, YAML 출력

```csharp
using System;
using System.Text.Json;

class MultiFormatOutput
{
    public static void Output(object data, string format)
    {
        var result = format.ToLower() switch
        {
            "json" => JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }),
            "xml" => SerializeToXml(data),
            "yaml" => SerializeToYaml(data),
            _ => data.ToString()
        };

        Console.WriteLine(result);
    }
}
```

### 14.2 구조화된 로깅

```csharp
// Serilog 구조화 로깅
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

Log.Information("Processing {Count} files from {Directory}",
    files.Length, directory);

// 출력 (JSON):
// {"@t":"2025-11-12T...","Count":5,"Directory":"/data"}
```

### 14.4 CI/CD 파이프라인 통합

```yaml
# GitHub Actions 예시
name: Build and Test

on: [push]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '9.0.x'
      - name: Build
        run: dotnet build
      - name: Test
        run: dotnet test
      - name: Run CLI Tool
        run: dotnet run --project MyTool -- --input data.txt --output result.json
```

## Chapter 15: 보안과 권한 관리

### 15.1 민감한 정보 처리

```csharp
using System;

// User Secrets (개발 환경)
// dotnet user-secrets set "ApiKey" "secret-key-123"

var apiKey = configuration["ApiKey"];

// 환경 변수 (프로덕션)
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

// 절대 하지 말 것:
// const string ApiKey = "secret-key-123";  // ❌ 하드코딩
```

### 15.2 안전한 패스워드 입력

```csharp
using System;
using System.Text;

static string ReadPassword()
{
    var password = new StringBuilder();

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
            break;

        if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password.Remove(password.Length - 1, 1);
            Console.Write("\b \b");
        }
        else if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
            Console.Write("*");
        }
    }

    Console.WriteLine();
    return password.ToString();
}
```

### 15.4 환경 변수와 시크릿 관리

```csharp
using System;
using Microsoft.Extensions.Configuration;

// 환경별 설정
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Azure Key Vault, AWS Secrets Manager 등 사용
if (environment == "Production")
{
    // configuration.AddAzureKeyVault(...);
}
```

## 핵심 요약

- **디자인 패턴**: Command, Chain of Responsibility, Strategy
- **출력 포맷**: JSON, XML, YAML 지원으로 도구 간 연동
- **CI/CD**: 자동화 파이프라인에서 실행 가능하도록 설계
- **보안**: 민감 정보 분리, 안전한 입력 처리
- **환경 관리**: User Secrets, 환경 변수, Key Vault
