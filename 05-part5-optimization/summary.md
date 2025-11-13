# Part V: 고급 기능과 최적화 - 요약

이 Part는 콘솔 애플리케이션의 성능을 최적화하고 크로스 플랫폼 호환성을 확보하는 방법을 다룹니다.

## Chapter 10: 비동기 프로그래밍과 성능

### 10.1 콘솔 애플리케이션에서의 async/await

[비동기 프로그래밍](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/)은 [async/await](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/async-scenarios) 키워드를 사용하여 I/O 작업이나 긴 작업을 효율적으로 처리합니다.

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;

// Main을 async로
static async Task<int> Main(string[] args)
{
    await ProcessFilesAsync(args);
    return 0;
}

// 병렬 처리
var tasks = files.Select(f => ProcessFileAsync(f));
await Task.WhenAll(tasks);
```

### 10.2 취소 토큰과 타임아웃

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    await LongRunningOperationAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation timed out");
}
```

### 10.3 병렬 처리와 동시성

```csharp
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

// Parallel.ForEachAsync (.NET 6+)
await Parallel.ForEachAsync(items, async (item, ct) =>
{
    await ProcessItemAsync(item, ct);
});

// Channel을 사용한 생산자-소비자 패턴
var channel = Channel.CreateUnbounded<string>();

// 생산자
_ = Task.Run(async () =>
{
    await foreach (var item in ReadItemsAsync())
    {
        await channel.Writer.WriteAsync(item);
    }
    channel.Writer.Complete();
});

// 소비자
await foreach (var item in channel.Reader.ReadAllAsync())
{
    await ProcessItemAsync(item);
}
```

### 10.4 메모리 최적화 (Span<T>, Memory<T>)

```csharp
using System;
using System.Threading.Tasks;

// Span<T>로 메모리 할당 최소화
ReadOnlySpan<char> ProcessLine(ReadOnlySpan<char> line)
{
    int index = line.IndexOf(':');
    if (index >= 0)
    {
        return line.Slice(index + 1).Trim();
    }
    return line;
}

// Memory<T>로 비동기 작업
async Task ProcessAsync(Memory<byte> buffer)
{
    int bytesRead = await stream.ReadAsync(buffer);
    // buffer.Span을 사용한 처리
}
```

### 10.5 Native AOT와 트리밍

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <PublishAot>true</PublishAot>
    <PublishTrimmed>true</PublishTrimmed>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>
</Project>
```

```bash
dotnet publish -c Release
# 실행 파일 크기: ~3MB (vs ~60MB)
# 시작 시간: ~10ms (vs ~200ms)
```

## Chapter 11: 크로스 플랫폼 고려사항

### 11.1 Windows, Linux, macOS 차이점

```csharp
using System;
using System.IO;

var separator = Path.DirectorySeparatorChar;  // Windows: \, Unix: /
var newLine = Environment.NewLine;  // Windows: \r\n, Unix: \n

// 플랫폼 감지
if (OperatingSystem.IsWindows())
{
    // Windows 전용 코드
}
else if (OperatingSystem.IsLinux())
{
    // Linux 전용 코드
}
else if (OperatingSystem.IsMacOS())
{
    // macOS 전용 코드
}
```

### 11.3 ANSI 이스케이프 시퀀스

```csharp
// Windows 10+에서 ANSI 활성화
bool EnableAnsiSupport()
{
    if (!OperatingSystem.IsWindows()) return true;

    var handle = GetStdHandle(STD_OUTPUT_HANDLE);
    GetConsoleMode(handle, out uint mode);
    mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
    return SetConsoleMode(handle, mode);
}
```

### 11.4 파일 시스템과 경로 처리

```csharp
using System.IO;

// 크로스 플랫폼 경로 처리
var path = Path.Combine("data", "files", "log.txt");

// 올바름: Path.Combine 사용
var file = Path.Combine(baseDir, "subfolder", "file.txt");

// 잘못됨: 하드코딩
var wrongFile = $"{baseDir}\\subfolder\\file.txt";  // Windows만 작동
```

## Chapter 12: 테스팅과 디버깅

### 12.1 콘솔 출력 테스팅

```csharp
using System;
using System.IO;
using Xunit;

public class ConsoleAppTests
{
    [Fact]
    public void TestConsoleOutput()
    {
        // Console.Out을 캡처
        var output = new StringWriter();
        Console.SetOut(output);

        // 테스트할 코드 실행
        MyApp.PrintGreeting("World");

        // 검증
        Assert.Equal("Hello, World!\n", output.ToString());
    }
}
```

### 12.2 입력 시뮬레이션

```csharp
using System;
using System.IO;
using Xunit;

[Fact]
public void TestConsoleInput()
{
    // 입력 시뮬레이션
    var input = new StringReader("John\n30\n");
    Console.SetIn(input);

    var output = new StringWriter();
    Console.SetOut(output);

    // 실행
    MyApp.CollectUserInfo();

    // 검증
    var result = output.ToString();
    Assert.Contains("John", result);
    Assert.Contains("30", result);
}
```

### 12.3 통합 테스트

```csharp
using System.IO;
using System.Threading.Tasks;
using Xunit;

[Fact]
public async Task IntegrationTest()
{
    // System.CommandLine 통합 테스트
    var rootCommand = CreateRootCommand();

    var result = await rootCommand.InvokeAsync("--input test.txt --output result.txt");

    Assert.Equal(0, result);
    Assert.True(File.Exists("result.txt"));
}
```

### 12.5 성능 프로파일링

```csharp
using System;
using System.Diagnostics;

var sw = Stopwatch.StartNew();

// 측정할 코드
ProcessLargeFile("data.txt");

sw.Stop();
Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms");

// BenchmarkDotNet 사용
[MemoryDiagnoser]
public class Benchmarks
{
    [Benchmark]
    public void ProcessWithLinq() { /* ... */ }

    [Benchmark]
    public void ProcessWithFor() { /* ... */ }
}
```

## 핵심 요약

- **비동기**: async/await, Task.WhenAll, Parallel.ForEachAsync
- **메모리**: Span<T>, Memory<T>로 할당 최소화
- **Native AOT**: 빠른 시작, 작은 크기
- **크로스 플랫폼**: Path, Environment, OperatingSystem
- **테스팅**: Console 리다이렉션, 통합 테스트
- **프로파일링**: Stopwatch, BenchmarkDotNet
