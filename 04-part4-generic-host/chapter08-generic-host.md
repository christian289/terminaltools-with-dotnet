# Chapter 8: Generic Host 기반 콘솔 애플리케이션

## 8.1 IHost와 IHostBuilder 이해

### Generic Host란?

[Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)는 .NET Core 2.1에서 도입된 애플리케이션 호스팅 모델입니다. 원래 [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)를 위해 만들어진 개념이 콘솔 애플리케이션에도 확장되었습니다.

**주요 이점:**
- [의존성 주입](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection) (DI)
- 구성 관리 ([Configuration](https://learn.microsoft.com/dotnet/core/extensions/configuration))
- [로깅](https://learn.microsoft.com/dotnet/core/extensions/logging)
- 백그라운드 서비스
- 생명주기 관리
- 우아한 종료

### 기본 Generic Host 애플리케이션

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// .NET 6+ Top-Level Statements
var builder = Host.CreateApplicationBuilder(args);

// 서비스 등록
builder.Services.AddHostedService<Worker>();

// 로깅 설정
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var host = builder.Build();

await host.RunAsync();

// Worker 서비스
class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;

    public Worker(ILogger<Worker> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
```

### 전통적인 Main 메서드 스타일

```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GenericHostExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IDataProcessor, DataProcessor>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
    }
}
```

## 8.2 의존성 주입 컨테이너 활용

### 서비스 등록 패턴

```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Singleton: 앱 생명주기 동안 하나의 인스턴스
builder.Services.AddSingleton<IConfiguration, Configuration>();

// Scoped: 요청/작업당 하나의 인스턴스
builder.Services.AddScoped<IRepository, Repository>();

// Transient: 요청시마다 새 인스턴스
builder.Services.AddTransient<IService, Service>();

// 구현체로 직접 등록
builder.Services.AddSingleton<DatabaseConnection>();

// 팩토리 패턴
builder.Services.AddSingleton<ICache>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new RedisCache(config.ConnectionString);
});

// 옵션 패턴
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

var host = builder.Build();
await host.RunAsync();

// 인터페이스와 구현
interface IDataProcessor
{
    Task ProcessAsync(string data);
}

class DataProcessor : IDataProcessor
{
    private readonly ILogger<DataProcessor> logger;
    private readonly IConfiguration config;

    public DataProcessor(ILogger<DataProcessor> logger, IConfiguration config)
    {
        this.logger = logger;
        this.config = config;
    }

    public async Task ProcessAsync(string data)
    {
        logger.LogInformation("Processing: {data}", data);
        await Task.Delay(100);
    }
}
```

### 실전 예제: CLI 도구

```csharp
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

var rootCommand = new RootCommand("Generic Host 기반 CLI");

var inputOption = new Option<string>(
    "--input",
    "입력 파일"
) { IsRequired = true };

var outputOption = new Option<string>(
    "--output",
    "출력 파일"
);

rootCommand.AddOption(inputOption);
rootCommand.AddOption(outputOption);

rootCommand.SetHandler(async (input, output) =>
{
    // Generic Host 구성
    var builder = Host.CreateApplicationBuilder();

    // CLI 옵션을 서비스로 등록
    builder.Services.AddSingleton(new CliOptions
    {
        InputFile = input,
        OutputFile = output
    });

    // 서비스 등록
    builder.Services.AddSingleton<IFileReader, FileReader>();
    builder.Services.AddSingleton<IDataProcessor, DataProcessor>();
    builder.Services.AddSingleton<IFileWriter, FileWriter>();
    builder.Services.AddHostedService<ProcessorService>();

    // 로깅
    builder.Logging.SetMinimumLevel(LogLevel.Information);

    var host = builder.Build();
    await host.RunAsync();

}, inputOption, outputOption);

return await rootCommand.InvokeAsync(args);

// 옵션 클래스
class CliOptions
{
    public string InputFile { get; set; } = "";
    public string? OutputFile { get; set; }
}

// 서비스 구현
class ProcessorService : BackgroundService
{
    private readonly IFileReader reader;
    private readonly IDataProcessor processor;
    private readonly IFileWriter writer;
    private readonly CliOptions options;
    private readonly IHostApplicationLifetime lifetime;
    private readonly ILogger<ProcessorService> logger;

    public ProcessorService(
        IFileReader reader,
        IDataProcessor processor,
        IFileWriter writer,
        CliOptions options,
        IHostApplicationLifetime lifetime,
        ILogger<ProcessorService> logger)
    {
        this.reader = reader;
        this.processor = processor;
        this.writer = writer;
        this.options = options;
        this.lifetime = lifetime;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("처리 시작: {input}", options.InputFile);

            var data = await reader.ReadAsync(options.InputFile);
            var processed = await processor.ProcessAsync(data);

            if (options.OutputFile != null)
            {
                await writer.WriteAsync(options.OutputFile, processed);
                logger.LogInformation("결과 저장: {output}", options.OutputFile);
            }
            else
            {
                Console.WriteLine(processed);
            }

            logger.LogInformation("처리 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "처리 중 에러 발생");
            Environment.ExitCode = 1;
        }
        finally
        {
            // 애플리케이션 종료
            lifetime.StopApplication();
        }
    }
}

interface IFileReader
{
    Task<string> ReadAsync(string path);
}

class FileReader : IFileReader
{
    public async Task<string> ReadAsync(string path)
    {
        return await File.ReadAllTextAsync(path);
    }
}

interface IDataProcessor
{
    Task<string> ProcessAsync(string data);
}

class DataProcessor : IDataProcessor
{
    public async Task<string> ProcessAsync(string data)
    {
        await Task.Delay(100); // 시뮬레이션
        return data.ToUpper();
    }
}

interface IFileWriter
{
    Task WriteAsync(string path, string data);
}

class FileWriter : IFileWriter
{
    public async Task WriteAsync(string path, string data)
    {
        await File.WriteAllTextAsync(path, data);
    }
}
```

## 8.3 구성(Configuration) 관리

### appsettings.json 사용

```json
// appsettings.json
{
  "App": {
    "Name": "DataProcessor",
    "Version": "1.0.0",
    "MaxRetries": 3,
    "Timeout": 30
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=mydb",
    "MaxPoolSize": 100
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

```csharp
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

// 구성 소스 추가
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// 옵션 패턴으로 바인딩
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("App"));

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

builder.Services.AddHostedService<ConfiguredWorker>();

var host = builder.Build();
await host.RunAsync();

// 설정 클래스들
class AppSettings
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public int MaxRetries { get; set; }
    public int Timeout { get; set; }
}

class DatabaseSettings
{
    public string ConnectionString { get; set; } = "";
    public int MaxPoolSize { get; set; }
}

// 설정 사용
class ConfiguredWorker : BackgroundService
{
    private readonly IOptions<AppSettings> appSettings;
    private readonly IOptions<DatabaseSettings> dbSettings;
    private readonly ILogger<ConfiguredWorker> logger;

    public ConfiguredWorker(
        IOptions<AppSettings> appSettings,
        IOptions<DatabaseSettings> dbSettings,
        ILogger<ConfiguredWorker> logger)
    {
        this.appSettings = appSettings;
        this.dbSettings = dbSettings;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var app = appSettings.Value;
        var db = dbSettings.Value;

        logger.LogInformation("앱: {name} v{version}", app.Name, app.Version);
        logger.LogInformation("DB: {connection}", db.ConnectionString);
        logger.LogInformation("최대 재시도: {retries}", app.MaxRetries);

        await Task.CompletedTask;
    }
}
```

### 환경 변수와 비밀 정보

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// 개발 환경에서 User Secrets 사용
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// 환경 변수 (우선순위 높음)
builder.Configuration.AddEnvironmentVariables(prefix: "MYAPP_");

// 구성 값 읽기
var apiKey = builder.Configuration["ApiKey"];
var dbPassword = builder.Configuration["Database:Password"];

// 환경별 동작
if (builder.Environment.IsProduction())
{
    // 프로덕션 설정
    builder.Services.AddSingleton<ICache, RedisCache>();
}
else
{
    // 개발 설정
    builder.Services.AddSingleton<ICache, MemoryCache>();
}
```

## 8.4 로깅과 모니터링

### 로깅 설정

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// 로깅 설정
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = "simple";
});

// 로그 레벨 설정
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);

// Serilog 사용 예시
// builder.Logging.AddSerilog();

builder.Services.AddHostedService<LoggingWorker>();

var host = builder.Build();
await host.RunAsync();

class LoggingWorker : BackgroundService
{
    private readonly ILogger<LoggingWorker> logger;

    public LoggingWorker(ILogger<LoggingWorker> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 다양한 로그 레벨
        logger.LogTrace("Trace 메시지");
        logger.LogDebug("Debug 메시지");
        logger.LogInformation("Information 메시지");
        logger.LogWarning("Warning 메시지");
        logger.LogError("Error 메시지");
        logger.LogCritical("Critical 메시지");

        // 구조화된 로깅
        var userId = 12345;
        var action = "Login";
        logger.LogInformation("사용자 {userId}가 {action} 했습니다", userId, action);

        // 예외 로깅
        try
        {
            throw new Exception("테스트 예외");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "작업 실패: {message}", ex.Message);
        }

        // 로그 스코프
        using (logger.BeginScope("요청 ID: {requestId}", Guid.NewGuid()))
        {
            logger.LogInformation("스코프 내 로그 1");
            logger.LogInformation("스코프 내 로그 2");
        }

        await Task.CompletedTask;
    }
}
```

## 8.5 백그라운드 서비스와 생명주기

### 백그라운드 서비스 패턴

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// 1. 단순 반복 작업
class PeriodicWorker : BackgroundService
{
    private readonly ILogger<PeriodicWorker> logger;
    private readonly TimeSpan interval = TimeSpan.FromSeconds(5);

    public PeriodicWorker(ILogger<PeriodicWorker> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("주기적 작업 시작");

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("작업 실행: {time}", DateTime.Now);
            await DoWork();
            await Task.Delay(interval, stoppingToken);
        }

        logger.LogInformation("주기적 작업 종료");
    }

    private async Task DoWork()
    {
        // 실제 작업
        await Task.Delay(100);
    }
}

// 2. 큐 기반 작업자
class QueuedWorker : BackgroundService
{
    private readonly IBackgroundTaskQueue taskQueue;
    private readonly ILogger<QueuedWorker> logger;

    public QueuedWorker(
        IBackgroundTaskQueue taskQueue,
        ILogger<QueuedWorker> logger)
    {
        this.taskQueue = taskQueue;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("큐 작업자 시작");

        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "작업 실행 중 에러");
            }
        }

        logger.LogInformation("큐 작업자 종료");
    }
}

// 백그라운드 작업 큐 인터페이스
interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

// 3. 생명주기 관리
class ManagedService : IHostedService
{
    private readonly ILogger<ManagedService> logger;

    public ManagedService(ILogger<ManagedService> logger)
    {
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("서비스 시작");

        // 초기화 작업
        InitializeResources();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("서비스 종료");

        // 정리 작업
        CleanupResources();

        return Task.CompletedTask;
    }

    private void InitializeResources()
    {
        // 데이터베이스 연결, 캐시 초기화 등
    }

    private void CleanupResources()
    {
        // 연결 종료, 리소스 해제 등
    }
}
```

### 우아한 종료

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// 종료 타임아웃 설정
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHostedService<GracefulWorker>();

var host = builder.Build();

// 생명주기 이벤트 구독
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("애플리케이션 시작됨");
});

lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("애플리케이션 종료 중...");
});

lifetime.ApplicationStopped.Register(() =>
{
    Console.WriteLine("애플리케이션 종료됨");
});

await host.RunAsync();

class GracefulWorker : BackgroundService
{
    private readonly ILogger<GracefulWorker> logger;
    private readonly IHostApplicationLifetime lifetime;

    public GracefulWorker(
        ILogger<GracefulWorker> logger,
        IHostApplicationLifetime lifetime)
    {
        this.logger = logger;
        this.lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await DoWork(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("작업이 취소되었습니다");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "치명적 에러 발생");
            Environment.ExitCode = 1;
        }
        finally
        {
            // 애플리케이션 종료
            lifetime.StopApplication();
        }
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        for (int i = 0; i < 100; i++)
        {
            stoppingToken.ThrowIfCancellationRequested();

            logger.LogInformation("진행: {i}/100", i + 1);
            await Task.Delay(100, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("정리 작업 시작");

        // 진행 중인 작업 완료 대기
        await Task.Delay(1000, cancellationToken);

        logger.LogInformation("정리 작업 완료");

        await base.StopAsync(cancellationToken);
    }
}
```

### 핵심 요약

1. **Generic Host**: 엔터프라이즈급 콘솔 애플리케이션을 위한 프레임워크
2. **의존성 주입**: 느슨한 결합과 테스트 가능한 코드
3. **구성 관리**: appsettings.json, 환경 변수, User Secrets
4. **로깅**: 구조화된 로깅과 다양한 로그 레벨
5. **백그라운드 서비스**: 장기 실행 작업과 우아한 종료

다음 챕터에서는 Top-Level Programs와 전통적 구조의 비교를 다루겠습니다.
