# Chapter 7: System.CommandLine으로 만드는 현대적 CLI

## 7.1 System.CommandLine 아키텍처

### System.CommandLine이란?

System.CommandLine은 Microsoft가 공식적으로 개발하는 CLI 프레임워크로, .NET CLI, dotnet tool 등에서 사용됩니다.

**주요 특징:**
- 강력한 타입 시스템
- 자동 도움말 생성
- 탭 완성 지원
- POSIX 규약 준수
- 검증 및 제안 기능

### 설치

```bash
dotnet add package System.CommandLine
```

### 핵심 구성 요소

```csharp
using System.CommandLine;

// 1. Command: 실행 가능한 명령
var rootCommand = new RootCommand("파일 관리 도구");

// 2. Option: 플래그와 값 옵션
var verboseOption = new Option<bool>(
    aliases: new[] { "-v", "--verbose" },
    description: "상세 출력 활성화"
);

// 3. Argument: 위치 인자
var fileArgument = new Argument<FileInfo>(
    name: "file",
    description: "처리할 파일"
);

// 4. Handler: 실행 로직
rootCommand.SetHandler((file, verbose) =>
{
    Console.WriteLine($"Processing: {file.FullName}");
    if (verbose)
    {
        Console.WriteLine("Verbose mode enabled");
    }
}, fileArgument, verboseOption);

// 실행
return await rootCommand.InvokeAsync(args);
```

## 7.2 커맨드, 옵션, 인자 모델링

### 옵션 타입

```csharp
using System.CommandLine;

var rootCommand = new RootCommand("옵션 데모");

// Boolean 플래그
var forceOption = new Option<bool>(
    aliases: new[] { "-f", "--force" },
    description: "강제 실행"
);

// 문자열 옵션
var outputOption = new Option<string>(
    aliases: new[] { "-o", "--output" },
    description: "출력 파일"
);

// 정수 옵션 (기본값 포함)
var retriesOption = new Option<int>(
    aliases: new[] { "-r", "--retries" },
    getDefaultValue: () => 3,
    description: "재시도 횟수"
);

// 열거형 옵션
var formatOption = new Option<OutputFormat>(
    aliases: new[] { "--format" },
    getDefaultValue: () => OutputFormat.Json,
    description: "출력 형식"
);

// 배열 옵션
var tagsOption = new Option<string[]>(
    aliases: new[] { "--tags" },
    description: "태그 목록"
) { AllowMultipleArgumentsPerToken = true };

rootCommand.AddOption(forceOption);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(retriesOption);
rootCommand.AddOption(formatOption);
rootCommand.AddOption(tagsOption);

rootCommand.SetHandler((force, output, retries, format, tags) =>
{
    Console.WriteLine($"Force: {force}");
    Console.WriteLine($"Output: {output ?? "(none)"}");
    Console.WriteLine($"Retries: {retries}");
    Console.WriteLine($"Format: {format}");
    Console.WriteLine($"Tags: {string.Join(", ", tags ?? Array.Empty<string>())}");
}, forceOption, outputOption, retriesOption, formatOption, tagsOption);

await rootCommand.InvokeAsync(args);

enum OutputFormat
{
    Json,
    Xml,
    Yaml,
    Csv
}
```

**사용 예:**
```bash
dotnet run -- --force --output result.txt --retries 5 --format Yaml --tags tag1 --tags tag2 --tags tag3
```

### 인자 모델링

```csharp
using System.CommandLine;

var rootCommand = new RootCommand("인자 데모");

// 필수 인자
var sourceArgument = new Argument<FileInfo>(
    name: "source",
    description: "원본 파일"
);

// 선택적 인자
var destinationArgument = new Argument<FileInfo?>(
    name: "destination",
    description: "대상 파일 (선택)"
) { Arity = ArgumentArity.ZeroOrOne };

// 다중 인자
var filesArgument = new Argument<FileInfo[]>(
    name: "files",
    description: "처리할 파일들"
) { Arity = ArgumentArity.OneOrMore };

var command = new Command("copy", "파일 복사");
command.AddArgument(sourceArgument);
command.AddArgument(destinationArgument);

command.SetHandler((source, destination) =>
{
    Console.WriteLine($"Source: {source.FullName}");
    Console.WriteLine($"Destination: {destination?.FullName ?? "(stdout)"}");

    if (destination != null)
    {
        source.CopyTo(destination.FullName);
    }
    else
    {
        Console.WriteLine(File.ReadAllText(source.FullName));
    }
}, sourceArgument, destinationArgument);

rootCommand.AddCommand(command);
await rootCommand.InvokeAsync(args);
```

### 서브커맨드 구조

```csharp
using System.CommandLine;

var rootCommand = new RootCommand("Git 스타일 CLI");

// git add 커맨드
var addCommand = new Command("add", "스테이징에 파일 추가");
var addFilesArg = new Argument<string[]>("files") { Arity = ArgumentArity.OneOrMore };
var addAllOption = new Option<bool>(new[] { "-A", "--all" }, "모든 변경사항 추가");
addCommand.AddArgument(addFilesArg);
addCommand.AddOption(addAllOption);
addCommand.SetHandler((files, all) =>
{
    if (all)
    {
        Console.WriteLine("Adding all changes");
    }
    else
    {
        foreach (var file in files)
        {
            Console.WriteLine($"Adding: {file}");
        }
    }
}, addFilesArg, addAllOption);

// git commit 커맨드
var commitCommand = new Command("commit", "변경사항 커밋");
var messageOption = new Option<string>(new[] { "-m", "--message" }, "커밋 메시지") { IsRequired = true };
var amendOption = new Option<bool>("--amend", "이전 커밋 수정");
commitCommand.AddOption(messageOption);
commitCommand.AddOption(amendOption);
commitCommand.SetHandler((message, amend) =>
{
    Console.WriteLine($"Committing: {message}");
    if (amend)
    {
        Console.WriteLine("Amending previous commit");
    }
}, messageOption, amendOption);

// git log 커맨드
var logCommand = new Command("log", "커밋 히스토리 표시");
var onelineOption = new Option<bool>("--oneline", "한 줄로 표시");
var maxCountOption = new Option<int?>(new[] { "-n", "--max-count" }, "최대 표시 개수");
logCommand.AddOption(onelineOption);
logCommand.AddOption(maxCountOption);
logCommand.SetHandler((oneline, maxCount) =>
{
    Console.WriteLine($"Showing log (oneline: {oneline}, max: {maxCount?.ToString() ?? "all"})");
}, onelineOption, maxCountOption);

rootCommand.AddCommand(addCommand);
rootCommand.AddCommand(commitCommand);
rootCommand.AddCommand(logCommand);

await rootCommand.InvokeAsync(args);
```

## 7.3 파싱과 바인딩 메커니즘

### 커스텀 바인딩

```csharp
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;

var rootCommand = new RootCommand();

// 복잡한 객체를 바인딩하는 Binder
class ConfigBinder : BinderBase<Config>
{
    private readonly Option<string> hostOption;
    private readonly Option<int> portOption;
    private readonly Option<bool> sslOption;

    public ConfigBinder(Option<string> hostOption, Option<int> portOption, Option<bool> sslOption)
    {
        this.hostOption = hostOption;
        this.portOption = portOption;
        this.sslOption = sslOption;
    }

    protected override Config GetBoundValue(BindingContext bindingContext)
    {
        return new Config
        {
            Host = bindingContext.ParseResult.GetValueForOption(hostOption)!,
            Port = bindingContext.ParseResult.GetValueForOption(portOption),
            UseSsl = bindingContext.ParseResult.GetValueForOption(sslOption)
        };
    }
}

var hostOption = new Option<string>("--host", () => "localhost");
var portOption = new Option<int>("--port", () => 8080);
var sslOption = new Option<bool>("--ssl");

rootCommand.AddOption(hostOption);
rootCommand.AddOption(portOption);
rootCommand.AddOption(sslOption);

rootCommand.SetHandler((config) =>
{
    Console.WriteLine($"Connecting to {(config.UseSsl ? "https" : "http")}://{config.Host}:{config.Port}");
}, new ConfigBinder(hostOption, portOption, sslOption));

await rootCommand.InvokeAsync(args);

class Config
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public bool UseSsl { get; set; }
}
```

## 7.4 자동 완성과 도움말 생성

### 탭 완성 활성화

```csharp
using System.CommandLine;

var rootCommand = new RootCommand("완성 기능 데모");

// 완성 제안 추가
var environmentOption = new Option<string>("--env")
{
    Description = "환경"
};

// 미리 정의된 값 제안
environmentOption.AddCompletions("dev", "staging", "production");

// 동적 완성
var fileOption = new Option<string>("--file")
{
    Description = "파일"
};

fileOption.AddCompletions(ctx =>
{
    // 현재 디렉토리의 파일 제안
    var pattern = ctx.WordToComplete + "*";
    return Directory.GetFiles(".", pattern)
        .Select(f => Path.GetFileName(f))
        .ToArray();
});

rootCommand.AddOption(environmentOption);
rootCommand.AddOption(fileOption);

// 탭 완성 스크립트 생성 커맨드
var completionCommand = new Command("completion", "탭 완성 스크립트 생성");

// Bash 완성
var bashCompletion = new Command("bash", "Bash 완성 스크립트");
bashCompletion.SetHandler(() =>
{
    Console.WriteLine("# Bash completion script");
    Console.WriteLine("# Run: eval \"$(myapp completion bash)\"");
    // 실제 완성 스크립트 출력
});

// PowerShell 완성
var pwshCompletion = new Command("pwsh", "PowerShell 완성 스크립트");
pwshCompletion.SetHandler(() =>
{
    Console.WriteLine("# PowerShell completion script");
    Console.WriteLine("# Add to profile: myapp completion pwsh | Out-String | Invoke-Expression");
    // 실제 완성 스크립트 출력
});

completionCommand.AddCommand(bashCompletion);
completionCommand.AddCommand(pwshCompletion);
rootCommand.AddCommand(completionCommand);

await rootCommand.InvokeAsync(args);
```

### 커스텀 도움말

```csharp
using System.CommandLine;
using System.CommandLine.Help;

var rootCommand = new RootCommand("커스텀 도움말 데모")
{
    Description = "이 도구는 파일을 처리하는 강력한 CLI입니다."
};

var inputOption = new Option<FileInfo>(
    aliases: new[] { "-i", "--input" },
    description: "입력 파일 경로\n" +
                 "지원 형식: .txt, .csv, .json, .xml\n" +
                 "최대 크기: 100MB"
) { IsRequired = true };

var formatOption = new Option<string>(
    aliases: new[] { "-f", "--format" },
    getDefaultValue: () => "auto",
    description: "출력 형식\n" +
                 "  auto   : 자동 감지\n" +
                 "  json   : JSON 형식\n" +
                 "  xml    : XML 형식\n" +
                 "  csv    : CSV 형식"
);

rootCommand.AddOption(inputOption);
rootCommand.AddOption(formatOption);

// 예제 추가
rootCommand.AddExample(new[] { "-i", "data.txt", "-f", "json" });
rootCommand.AddExample(new[] { "--input", "data.csv", "--format", "xml" });

rootCommand.SetHandler((input, format) =>
{
    Console.WriteLine($"Processing: {input.FullName}");
    Console.WriteLine($"Format: {format}");
}, inputOption, formatOption);

await rootCommand.InvokeAsync(args);
```

**생성되는 도움말:**
```
Description:
  이 도구는 파일을 처리하는 강력한 CLI입니다.

Usage:
  myapp [options]

Options:
  -i, --input <input> (REQUIRED)  입력 파일 경로
                                  지원 형식: .txt, .csv, .json, .xml
                                  최대 크기: 100MB
  -f, --format <format>           출력 형식 [default: auto]
                                    auto   : 자동 감지
                                    json   : JSON 형식
                                    xml    : XML 형식
                                    csv    : CSV 형식
  --version                       Show version information
  -?, -h, --help                  Show help and usage information

Examples:
  myapp -i data.txt -f json
  myapp --input data.csv --format xml
```

## 7.5 검증과 에러 처리

### 옵션 검증

```csharp
using System.CommandLine;
using System.CommandLine.Parsing;

var rootCommand = new RootCommand();

// 파일 존재 검증
var inputOption = new Option<FileInfo>("--input");
inputOption.AddValidator(result =>
{
    var file = result.GetValueForOption(inputOption);
    if (file != null && !file.Exists)
    {
        result.ErrorMessage = $"파일을 찾을 수 없습니다: {file.FullName}";
    }
});

// 범위 검증
var portOption = new Option<int>("--port");
portOption.AddValidator(result =>
{
    var port = result.GetValueForOption(portOption);
    if (port < 1024 || port > 65535)
    {
        result.ErrorMessage = $"포트는 1024-65535 범위여야 합니다 (입력: {port})";
    }
});

// 포맷 검증
var formatOption = new Option<string>("--format");
formatOption.AddValidator(result =>
{
    var format = result.GetValueForOption(formatOption);
    var validFormats = new[] { "json", "xml", "yaml", "csv" };

    if (format != null && !validFormats.Contains(format.ToLower()))
    {
        result.ErrorMessage = $"지원하지 않는 형식: {format}\n" +
                            $"가능한 값: {string.Join(", ", validFormats)}";
    }
});

rootCommand.AddOption(inputOption);
rootCommand.AddOption(portOption);
rootCommand.AddOption(formatOption);

rootCommand.SetHandler((input, port, format) =>
{
    Console.WriteLine($"Input: {input?.FullName}");
    Console.WriteLine($"Port: {port}");
    Console.WriteLine($"Format: {format}");
}, inputOption, portOption, formatOption);

await rootCommand.InvokeAsync(args);
```

### 에러 처리

```csharp
using System.CommandLine;

var rootCommand = new RootCommand();

var fileArg = new Argument<FileInfo>("file");

rootCommand.AddArgument(fileArg);

rootCommand.SetHandler(async (file) =>
{
    try
    {
        var content = await File.ReadAllTextAsync(file.FullName);
        Console.WriteLine(content);
        return 0;
    }
    catch (FileNotFoundException ex)
    {
        Console.Error.WriteLine($"에러: 파일을 찾을 수 없습니다 - {ex.Message}");
        return 1;
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.Error.WriteLine($"에러: 접근 권한이 없습니다 - {ex.Message}");
        return 2;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"예기치 않은 에러: {ex.Message}");
        return 99;
    }
}, fileArg);

return await rootCommand.InvokeAsync(args);
```

### 핵심 요약

1. **System.CommandLine**: Microsoft 공식 CLI 프레임워크
2. **강력한 타입 시스템**: Command, Option, Argument
3. **자동 기능**: 도움말, 탭 완성, 파싱
4. **검증**: 내장 및 커스텀 검증기
5. **확장성**: Binder, 커스텀 완성, 미들웨어

다음 챕터에서는 Top-Level Programs를 다루겠습니다.
