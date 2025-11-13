# Chapter 6: ConsoleAppFramework와 구조적 설계

## 6.1 ConsoleAppFramework 핵심 개념

### ConsoleAppFramework란?

[ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework)는 Cysharp가 개발한 경량 CLI 프레임워크로, 간결한 API와 높은 성능을 제공합니다.

**주요 특징:**
- 최소한의 보일러플레이트 코드
- 자동 커맨드 라인 파싱
- [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host) 통합
- 필터 파이프라인
- 배치 처리 지원

**공식 문서 및 소스 코드:**
- [GitHub 저장소](https://github.com/Cysharp/ConsoleAppFramework)
- [NuGet 패키지](https://www.nuget.org/packages/ConsoleAppFramework)

### 설치

```bash
dotnet new console -n MyCliTool
cd MyCliTool
dotnet add package ConsoleAppFramework
```

### 기본 사용법

```csharp
using ConsoleAppFramework;

// 최소 코드로 CLI 작성
var app = ConsoleApp.Create();
app.AddRootCommand((string name, int age) =>
{
    Console.WriteLine($"Hello {name}, you are {age} years old!");
});
app.Run(args);
```

**실행:**
```bash
dotnet run -- --name John --age 30
# 출력: Hello John, you are 30 years old!
```

### 메서드 기반 커맨드

```csharp
using ConsoleAppFramework;

var app = ConsoleApp.Create();
app.AddCommands<Commands>();
app.Run(args);

public class Commands
{
    /// <summary>
    /// 파일을 복사합니다
    /// </summary>
    /// <param name="source">-s, 원본 파일</param>
    /// <param name="destination">-d, 대상 파일</param>
    /// <param name="force">-f, 강제 덮어쓰기</param>
    [Command("copy")]
    public void CopyFile(
        [Argument] string source,
        [Argument] string destination,
        bool force = false)
    {
        Console.WriteLine($"Copying {source} to {destination} (force: {force})");

        if (File.Exists(destination) && !force)
        {
            throw new Exception("Destination file already exists. Use --force to overwrite.");
        }

        File.Copy(source, destination, force);
        Console.WriteLine("Copy completed!");
    }

    /// <summary>
    /// 파일을 삭제합니다
    /// </summary>
    [Command("delete")]
    public void DeleteFile([Argument] string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        File.Delete(path);
        Console.WriteLine($"Deleted: {path}");
    }

    /// <summary>
    /// 파일 목록을 표시합니다
    /// </summary>
    [Command("list")]
    public void ListFiles(
        string path = ".",
        bool recursive = false)
    {
        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = Directory.GetFiles(path, "*", option);

        foreach (var file in files)
        {
            Console.WriteLine(file);
        }

        Console.WriteLine($"\nTotal: {files.Length} files");
    }
}
```

**실행:**
```bash
dotnet run -- copy -s file1.txt -d file2.txt
dotnet run -- copy -s file1.txt -d file2.txt --force
dotnet run -- delete file2.txt
dotnet run -- list --path . --recursive
```

## 6.2 커맨드와 서브커맨드 구조

### Git 스타일 서브커맨드

```csharp
using ConsoleAppFramework;

var app = ConsoleApp.Create();
app.AddCommands<GitCommands>();
app.Run(args);

public class GitCommands
{
    [Command("clone")]
    public void Clone([Argument] string repository, string directory = "")
    {
        Console.WriteLine($"Cloning {repository} into {directory}");
    }

    [Command("add")]
    public void Add([Argument] params string[] files)
    {
        Console.WriteLine($"Adding {files.Length} files to staging area");
        foreach (var file in files)
        {
            Console.WriteLine($"  + {file}");
        }
    }

    [Command("commit")]
    public void Commit(string message, bool amend = false)
    {
        Console.WriteLine($"Creating commit: {message}");
        if (amend)
        {
            Console.WriteLine("Amending previous commit");
        }
    }

    [Command("push")]
    public void Push(string remote = "origin", string branch = "main")
    {
        Console.WriteLine($"Pushing to {remote}/{branch}");
    }

    [Command("status")]
    public void Status()
    {
        Console.WriteLine("On branch main");
        Console.WriteLine("nothing to commit, working tree clean");
    }
}
```

### 중첩된 커맨드 그룹

```csharp
using ConsoleAppFramework;

var app = ConsoleApp.Create();
app.AddCommands<DockerCommands>();
app.AddCommands<DockerContainerCommands>();
app.AddCommands<DockerImageCommands>();
app.Run(args);

public class DockerCommands
{
    [Command("version")]
    public void Version()
    {
        Console.WriteLine("Docker version 24.0.0");
    }
}

public class DockerContainerCommands
{
    [Command("container ls")]
    public void ListContainers(bool all = false)
    {
        Console.WriteLine($"Listing containers (all: {all})");
    }

    [Command("container start")]
    public void StartContainer([Argument] string container)
    {
        Console.WriteLine($"Starting container: {container}");
    }

    [Command("container stop")]
    public void StopContainer([Argument] string container)
    {
        Console.WriteLine($"Stopping container: {container}");
    }
}

public class DockerImageCommands
{
    [Command("image ls")]
    public void ListImages()
    {
        Console.WriteLine("Listing images");
    }

    [Command("image pull")]
    public void PullImage([Argument] string image)
    {
        Console.WriteLine($"Pulling image: {image}");
    }
}
```

## 6.3 의존성 주입과 설정 관리

### DI 통합

```csharp
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;

var app = ConsoleApp.Create();

app.ConfigureServices((context, services) =>
{
    // 서비스 등록
    services.AddSingleton<IFileService, FileService>();
    services.AddSingleton<ILogger, ConsoleLogger>();
});

app.AddCommands<FileCommands>();
app.Run(args);

// 서비스 인터페이스
public interface IFileService
{
    void ProcessFile(string path);
}

public interface ILogger
{
    void Log(string message);
}

// 구현
public class FileService : IFileService
{
    private readonly ILogger logger;

    public FileService(ILogger logger)
    {
        this.logger = logger;
    }

    public void ProcessFile(string path)
    {
        logger.Log($"Processing file: {path}");
        // 실제 처리 로직
        Thread.Sleep(1000);
        logger.Log("Processing completed");
    }
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}

// DI를 사용하는 커맨드
public class FileCommands
{
    private readonly IFileService fileService;
    private readonly ILogger logger;

    // 생성자 주입
    public FileCommands(IFileService fileService, ILogger logger)
    {
        this.fileService = fileService;
        this.logger = logger;
    }

    [Command("process")]
    public void ProcessFile([Argument] string path)
    {
        logger.Log($"Command started");
        fileService.ProcessFile(path);
        logger.Log($"Command completed");
    }
}
```

### 구성 관리

```csharp
using ConsoleAppFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var app = ConsoleApp.Create();

app.ConfigureServices((context, services) =>
{
    // Configuration 설정
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    // Options 패턴
    services.Configure<AppSettings>(configuration.GetSection("App"));
    services.AddSingleton<IDataService, DataService>();
});

app.AddCommands<DataCommands>();
app.Run(args);

// 설정 클래스
public class AppSettings
{
    public string ConnectionString { get; set; } = "";
    public int MaxRetries { get; set; } = 3;
    public int Timeout { get; set; } = 30;
}

// 서비스
public interface IDataService
{
    Task ProcessDataAsync();
}

public class DataService : IDataService
{
    private readonly IOptions<AppSettings> settings;
    private readonly ILogger logger;

    public DataService(IOptions<AppSettings> settings, ILogger logger)
    {
        this.settings = settings;
        this.logger = logger;
    }

    public async Task ProcessDataAsync()
    {
        var config = settings.Value;
        logger.Log($"Connecting to: {config.ConnectionString}");
        logger.Log($"Max retries: {config.MaxRetries}");
        logger.Log($"Timeout: {config.Timeout}s");

        await Task.Delay(100);
    }
}

public class DataCommands
{
    private readonly IDataService dataService;

    public DataCommands(IDataService dataService)
    {
        this.dataService = dataService;
    }

    [Command("process")]
    public async Task ProcessAsync()
    {
        await dataService.ProcessDataAsync();
    }
}
```

## 6.4 필터와 미들웨어 패턴

### 글로벌 필터

```csharp
using ConsoleAppFramework;
using ConsoleAppFramework.Filters;

var app = ConsoleApp.Create();

// 글로벌 필터 추가
app.UseFilter<TimingFilter>();
app.UseFilter<ErrorHandlingFilter>();

app.AddCommands<MyCommands>();
app.Run(args);

// 타이밍 필터
public class TimingFilter : ConsoleAppFilter
{
    public async override ValueTask InvokeAsync(ConsoleAppContext context, Func<ConsoleAppContext, ValueTask> next)
    {
        var sw = Stopwatch.StartNew();

        await next(context);

        sw.Stop();
        Console.WriteLine($"\n[Timing] Execution time: {sw.ElapsedMilliseconds}ms");
    }
}

// 에러 처리 필터
public class ErrorHandlingFilter : ConsoleAppFilter
{
    public async override ValueTask InvokeAsync(ConsoleAppContext context, Func<ConsoleAppContext, ValueTask> next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"\n[ERROR] {ex.Message}");
            Console.ResetColor();

            Environment.ExitCode = 1;
        }
    }
}
```

## 6.5 배치 모드와 대화형 모드

### 배치 모드

```csharp
using ConsoleAppFramework;

var app = ConsoleApp.Create();
app.AddCommands<BatchCommands>();
app.Run(args);

public class BatchCommands
{
    [Command("batch")]
    public async Task ProcessBatch([Argument] string inputFile)
    {
        if (!File.Exists(inputFile))
        {
            throw new FileNotFoundException($"Input file not found: {inputFile}");
        }

        var lines = await File.ReadAllLinesAsync(inputFile);
        var total = lines.Length;
        var processed = 0;
        var failed = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                await ProcessLineAsync(line);
                processed++;

                // 진행률 표시
                var percent = (processed + failed) * 100 / total;
                Console.Write($"\rProgress: {percent}% ({processed}/{total})");
            }
            catch (Exception ex)
            {
                failed++;
                Console.Error.WriteLine($"\nFailed to process: {line}");
                Console.Error.WriteLine($"  Error: {ex.Message}");
            }
        }

        Console.WriteLine($"\n\nBatch processing completed!");
        Console.WriteLine($"  Processed: {processed}");
        Console.WriteLine($"  Failed: {failed}");
        Console.WriteLine($"  Total: {total}");
    }

    private async Task ProcessLineAsync(string line)
    {
        // 실제 처리 로직
        await Task.Delay(100);
    }
}
```

### 핵심 요약

1. **ConsoleAppFramework**: 경량, 고성능 CLI 프레임워크
2. **커맨드 모델**: 메서드 기반, 자동 파싱
3. **DI 통합**: Generic Host와 완벽한 통합
4. **필터**: 미들웨어 패턴으로 횡단 관심사 처리
5. **배치 모드**: 대량 작업 처리에 최적화

다음 챕터에서는 System.CommandLine을 다루겠습니다.
