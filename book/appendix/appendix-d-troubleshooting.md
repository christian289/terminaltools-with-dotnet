# Appendix D: 트러블슈팅 가이드

## 일반적인 문제와 해결 방법

### 1. 한글/유니코드 깨짐 현상

**증상:**
```
출력: ?????? ??? ??
```

**원인:** 잘못된 인코딩 설정

**해결:**
```csharp
// Program.cs 시작 부분에 추가
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;
```

**Windows 콘솔:**
```bash
# 코드 페이지를 UTF-8로 변경
chcp 65001
```

### 2. 파이프라인에서 출력이 안 나옴

**증상:**
```bash
echo "test" | dotnet run
# 아무 출력 없음
```

**원인:** 버퍼링 문제

**해결:**
```csharp
// 즉시 플러시
Console.Out.AutoFlush = true;

// 또는 명시적 플러시
Console.WriteLine("output");
Console.Out.Flush();
```

### 3. 색상이 표시되지 않음

**증상:** ANSI 이스케이프 코드가 그대로 출력됨

**원인:** 터미널이 ANSI를 지원하지 않음

**해결:**
```csharp
// Windows 10 이상에서 ANSI 활성화
if (OperatingSystem.IsWindows())
{
    var handle = GetStdHandle(STD_OUTPUT_HANDLE);
    GetConsoleMode(handle, out uint mode);
    mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
    SetConsoleMode(handle, mode);
}

// 또는 리다이렉션 감지
if (Console.IsOutputRedirected)
{
    // 색상 사용 안 함
}
else
{
    // 색상 사용
    Console.ForegroundColor = ConsoleColor.Green;
}
```

### 4. Ctrl+C가 작동하지 않음

**증상:** Ctrl+C를 눌러도 프로그램이 종료되지 않음

**원인:** CancelKeyPress 이벤트 핸들러 필요

**해결:**
```csharp
Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("\n프로그램을 종료합니다...");
    // 정리 작업
    CleanupResources();

    e.Cancel = true;  // 기본 동작 취소
    Environment.Exit(0);
};
```

### 5. 대용량 파일 처리 시 OutOfMemoryException

**증상:**
```
System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown.
```

**원인:** File.ReadAllText 등으로 전체 파일을 메모리에 로드

**해결:**
```csharp
// ❌ 나쁜 예
var allText = File.ReadAllText("large-file.txt");

// ✅ 좋은 예
await foreach (var line in File.ReadLinesAsync("large-file.txt"))
{
    ProcessLine(line);
}

// 또는 스트림 사용
using var reader = new StreamReader("large-file.txt");
string? line;
while ((line = await reader.ReadLineAsync()) != null)
{
    ProcessLine(line);
}
```

### 6. System.CommandLine 파싱 오류

**증상:**
```
Unrecognized command or argument '--input'
```

**원인:** 옵션 이름 불일치

**해결:**
```csharp
// 옵션 정의 확인
var inputOption = new Option<string>(
    aliases: new[] { "-i", "--input" },  // 둘 다 지원
    description: "입력 파일"
);

// 사용 시
// dotnet run -- -i file.txt  (OK)
// dotnet run -- --input file.txt  (OK)
```

### 7. Generic Host 앱이 즉시 종료됨

**증상:** 프로그램이 시작하자마자 종료

**원인:** BackgroundService가 없거나 즉시 완료됨

**해결:**
```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    // ❌ 나쁜 예 - 즉시 완료
    Console.WriteLine("Started");

    // ✅ 좋은 예 - 계속 실행
    while (!stoppingToken.IsCancellationRequested)
    {
        await DoWork();
        await Task.Delay(1000, stoppingToken);
    }
}
```

### 8. 권한 거부 오류 (Permission Denied)

**증상:**
```
System.UnauthorizedAccessException: Access to the path '/usr/bin/mytool' is denied.
```

**원인:** 파일이나 디렉토리 접근 권한 없음

**해결:**
```bash
# Linux/macOS: 실행 권한 부여
chmod +x mytool

# 관리자 권한 필요 시
sudo ./mytool

# 또는 코드에서 처리
try
{
    File.WriteAllText(path, content);
}
catch (UnauthorizedAccessException)
{
    Console.Error.WriteLine("권한이 없습니다. sudo로 실행해주세요.");
    Environment.Exit(1);
}
```

### 9. Native AOT 컴파일 오류

**증상:**
```
Trimming assemblies failed. See output for details.
```

**원인:** 리플렉션 사용 코드가 트리밍됨

**해결:**
```csharp
// DynamicallyAccessedMembers 특성 사용
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
public class MyClass { }

// 또는 트리밍 제외
<ItemGroup>
  <TrimmerRootAssembly Include="MyAssembly" />
</ItemGroup>

// JSON 직렬화는 Source Generator 사용
[JsonSerializable(typeof(MyData))]
internal partial class MyJsonContext : JsonSerializerContext { }
```

### 10. 크로스 플랫폼 경로 문제

**증상:** Windows에서는 작동하지만 Linux/macOS에서 실패

**원인:** 하드코딩된 경로 구분자

**해결:**
```csharp
// ❌ 나쁜 예
var path = "data\\files\\log.txt";  // Windows만

// ✅ 좋은 예
var path = Path.Combine("data", "files", "log.txt");

// 또는
var path = Path.Join("data", "files", "log.txt");
```

## 디버깅 팁

### Console.WriteLine 디버깅

```csharp
#if DEBUG
    Console.WriteLine($"[DEBUG] Variable value: {variable}");
#endif
```

### 조건부 컴파일

```csharp
[Conditional("DEBUG")]
void DebugLog(string message)
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
}
```

### Trace 리스너

```csharp
Trace.Listeners.Add(new TextWriterTraceListener("debug.log"));
Trace.WriteLine("Debug message");
```

### 예외 상세 정보

```csharp
try
{
    // 코드
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Console.Error.WriteLine($"Stack trace:\n{ex.StackTrace}");

    #if DEBUG
    Console.Error.WriteLine($"Full exception:\n{ex}");
    #endif
}
```

## 성능 문제 해결

### 1. 느린 시작 시간

**진단:**
```bash
dotnet-trace collect --process-id <PID>
```

**해결:**
- Generic Host 대신 간단한 Main 사용
- 불필요한 의존성 제거
- Native AOT 컴파일 고려

### 2. 높은 메모리 사용

**진단:**
```bash
dotnet-counters monitor --process-id <PID>
dotnet-dump collect --process-id <PID>
```

**해결:**
- 스트리밍 사용 (ReadLines vs ReadAllLines)
- Span<T> 활용
- ArrayPool 사용
```csharp
var pool = ArrayPool<byte>.Shared;
var buffer = pool.Rent(1024);
try
{
    // 사용
}
finally
{
    pool.Return(buffer);
}
```

### 3. CPU 100% 사용

**원인:** 무한 루프 또는 폴링

**해결:**
```csharp
// ❌ 나쁜 예
while (true)
{
    if (CheckCondition())
        break;
}

// ✅ 좋은 예
while (true)
{
    if (CheckCondition())
        break;
    await Task.Delay(100);  // CPU 양보
}
```

## 로그 분석

### 구조화된 로깅

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

Log.Information("Processing {Count} files from {Directory}", files.Length, directory);
```

### 로그 레벨

```csharp
// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "MyApp": "Debug"
    }
  }
}
```

## 자주 묻는 질문 (FAQ)

**Q: dotnet tool로 설치한 도구를 찾을 수 없습니다.**

A: PATH에 도구 경로 추가:
```bash
# Linux/macOS
export PATH="$PATH:$HOME/.dotnet/tools"

# Windows
setx PATH "%PATH%;%USERPROFILE%\.dotnet\tools"
```

**Q: JSON 직렬화 시 순환 참조 오류**

A:
```csharp
var options = new JsonSerializerOptions
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles
};

JsonSerializer.Serialize(data, options);
```

**Q: 비동기 Main에서 예외 처리는?**

A:
```csharp
static async Task<int> Main(string[] args)
{
    try
    {
        await RunAsync(args);
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Fatal error: {ex.Message}");
        return 1;
    }
}
```

## 도움 받기

- **GitHub Issues**: 프로젝트 저장소의 Issues 섹션
- **.NET Discord**: https://aka.ms/dotnet-discord
- **Stack Overflow**: `[c#]` `[.net]` `[console-application]` 태그
- **.NET 문서**: https://docs.microsoft.com/dotnet

## 핵심 체크리스트

디버깅 전 확인 사항:
- [ ] UTF-8 인코딩 설정
- [ ] 리다이렉션 감지 및 처리
- [ ] 예외 처리 및 로깅
- [ ] 크로스 플랫폼 경로 사용
- [ ] 메모리 효율적 I/O
- [ ] 적절한 버퍼링 및 플러싱
- [ ] ANSI 지원 확인 (색상 사용 시)
- [ ] 권한 및 접근 제어
