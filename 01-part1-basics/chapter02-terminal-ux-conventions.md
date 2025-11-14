# Chapter 2: 터미널 UX의 전통과 규약

## 2.1 POSIX 표준과 명령행 인터페이스 규약

### POSIX란?

[POSIX](https://en.wikipedia.org/wiki/POSIX)(Portable Operating System Interface)는 [IEEE](https://www.ieee.org/)가 정의한 유닉스 운영체제의 표준 인터페이스입니다. POSIX는 명령행 도구의 동작 방식에 대한 가이드라인을 제공합니다.

### GNU 스타일 명령행 인자 규약

**Short Options (단축 옵션)**
```bash
# 단일 문자 옵션, 하이픈(-) 하나
command -v          # verbose
command -f file.txt # file 지정
command -vf file.txt # 옵션 결합
command -v -f file.txt # 분리
```

**Long Options (긴 옵션)**
```bash
# 단어 형태 옵션, 하이픈(--) 두 개
command --verbose
command --file=file.txt
command --file file.txt
```

**인자(Arguments)**
```bash
# 옵션이 아닌 값들
command arg1 arg2 arg3
cp source.txt destination.txt

# -- 구분자: 이후는 모두 인자로 취급
command -v -- -not-an-option.txt
```

### .NET에서 POSIX 스타일 구현

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace PosixStyleCli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Root Command 정의
            var rootCommand = new RootCommand("POSIX 스타일 파일 처리 도구");

            // Short option: -v, Long option: --verbose
            var verboseOption = new Option<bool>(
                aliases: new[] { "-v", "--verbose" },
                description: "상세 출력 활성화"
            );

            // Short option: -o, Long option: --output
            var outputOption = new Option<FileInfo?>(
                aliases: new[] { "-o", "--output" },
                description: "출력 파일 경로"
            );

            // Short option: -f, Long option: --force
            var forceOption = new Option<bool>(
                aliases: new[] { "-f", "--force" },
                description: "강제 실행 (기존 파일 덮어쓰기)"
            );

            // Arguments: 입력 파일들
            var inputArgument = new Argument<FileInfo[]>(
                name: "input-files",
                description: "처리할 입력 파일들"
            ) { Arity = ArgumentArity.OneOrMore };

            rootCommand.Options.Add(verboseOption);
            rootCommand.Options.Add(outputOption);
            rootCommand.Options.Add(forceOption);
            rootCommand.Arguments.Add(inputArgument);

            rootCommand.SetAction((verbose, output, force, inputs) =>
            {
                ProcessFiles(inputs, output, verbose, force);
            }, verboseOption, outputOption, forceOption, inputArgument);

            return await rootCommand.Parse(args).InvokeAsync();
        }

        static void ProcessFiles(FileInfo[] inputs, FileInfo? output, bool verbose, bool force)
        {
            if (verbose)
            {
                Console.WriteLine($"처리할 파일 수: {inputs.Length}");
                Console.WriteLine($"출력 파일: {output?.FullName ?? "(stdout)"}");
                Console.WriteLine($"강제 모드: {force}");
            }

            // 출력 파일 존재 확인
            if (output != null && output.Exists && !force)
            {
                Console.Error.WriteLine($"에러: 출력 파일이 이미 존재합니다: {output.FullName}");
                Console.Error.WriteLine("강제 실행하려면 -f 또는 --force 옵션을 사용하세요.");
                Environment.Exit(1);
            }

            var outputWriter = output != null
                ? new StreamWriter(output.FullName)
                : Console.Out;

            try
            {
                foreach (var input in inputs)
                {
                    if (!input.Exists)
                    {
                        Console.Error.WriteLine($"경고: 파일을 찾을 수 없습니다: {input.FullName}");
                        continue;
                    }

                    if (verbose)
                    {
                        Console.WriteLine($"처리 중: {input.Name}");
                    }

                    var content = File.ReadAllText(input.FullName);
                    outputWriter.WriteLine($"=== {input.Name} ===");
                    outputWriter.WriteLine(content);
                }

                if (verbose)
                {
                    Console.WriteLine("처리 완료!");
                }
            }
            finally
            {
                if (output != null)
                {
                    outputWriter.Dispose();
                }
            }
        }
    }
}
```

**사용 예시:**
```bash
# Short options
dotnet run -- -v file1.txt file2.txt
dotnet run -- -o output.txt file1.txt
dotnet run -- -vf -o output.txt file1.txt file2.txt

# Long options
dotnet run -- --verbose file1.txt file2.txt
dotnet run -- --output=output.txt file1.txt
dotnet run -- --verbose --force --output output.txt file1.txt file2.txt

# 혼합
dotnet run -- -v --output=result.txt file1.txt file2.txt
```

## 2.2 Exit Code와 에러 처리 관례

### Exit Code 규약

유닉스 시스템에서 프로그램은 종료 시 [exit code](https://en.wikipedia.org/wiki/Exit_status)를 반환합니다:

- **0**: 성공
- **1**: 일반 에러
- **2**: 명령행 사용법 에러
- **126**: 실행 불가능
- **127**: 명령을 찾을 수 없음
- **128+n**: 시그널 n으로 종료
- **130**: Ctrl+C (SIGINT)

### 관례적인 Exit Code 사용

```bash
# 성공
$ grep "pattern" file.txt
...
$ echo $?
0

# 찾지 못함 (에러는 아님)
$ grep "notfound" file.txt
$ echo $?
1

# 파일이 없음 (에러)
$ grep "pattern" nonexistent.txt
grep: nonexistent.txt: No such file or directory
$ echo $?
2
```

### .NET에서 Exit Code 구현

```csharp
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.CommandLine;

namespace ExitCodeExample
{
    // Exit Code 정의
    public static class ExitCodes
    {
        public const int Success = 0;
        public const int GeneralError = 1;
        public const int UsageError = 2;
        public const int FileNotFound = 3;
        public const int PermissionDenied = 4;
        public const int Timeout = 5;
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("파일 검색 도구");

            var patternArgument = new Argument<string>(
                "pattern",
                "검색할 패턴"
            );

            var fileArgument = new Argument<FileInfo>(
                "file",
                "검색할 파일"
            );

            var timeoutOption = new Option<int>(
                aliases: new[] { "-t", "--timeout" },
                getDefaultValue: () => 30,
                description: "타임아웃 (초)"
            );

            rootCommand.Arguments.Add(patternArgument);
            rootCommand.Arguments.Add(fileArgument);
            rootCommand.Options.Add(timeoutOption);

            rootCommand.SetAction((pattern, file, timeout) =>
            {
                return SearchFile(pattern, file, timeout);
            }, patternArgument, fileArgument, timeoutOption);

            return await rootCommand.Parse(args).InvokeAsync();
        }

        static int SearchFile(string pattern, FileInfo file, int timeout)
        {
            try
            {
                // 파일 존재 확인
                if (!file.Exists)
                {
                    Console.Error.WriteLine($"{file.Name}: 파일을 찾을 수 없습니다");
                    return ExitCodes.FileNotFound;
                }

                // 읽기 권한 확인
                try
                {
                    using var _ = file.OpenRead();
                }
                catch (UnauthorizedAccessException)
                {
                    Console.Error.WriteLine($"{file.Name}: 읽기 권한이 없습니다");
                    return ExitCodes.PermissionDenied;
                }

                // 검색 실행
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                bool found = false;

                try
                {
                    using var reader = file.OpenText();
                    string? line;
                    int lineNumber = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        lineNumber++;

                        if (line.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"{file.Name}:{lineNumber}: {line}");
                            found = true;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.Error.WriteLine($"타임아웃: {timeout}초 초과");
                    return ExitCodes.Timeout;
                }

                // 찾지 못한 경우 (에러는 아님)
                return found ? ExitCodes.Success : ExitCodes.GeneralError;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"예기치 않은 에러: {ex.Message}");
                return ExitCodes.GeneralError;
            }
        }
    }
}
```

**셸 스크립트에서 활용:**
```bash
#!/bin/bash

# .NET 도구 실행 후 exit code 확인
dotnet run -- "error" logfile.txt

case $? in
    0)
        echo "패턴을 찾았습니다"
        ;;
    1)
        echo "패턴을 찾지 못했습니다"
        ;;
    3)
        echo "파일이 없습니다"
        exit 1
        ;;
    4)
        echo "권한이 없습니다"
        exit 1
        ;;
    5)
        echo "타임아웃 발생"
        exit 1
        ;;
esac
```

## 2.3 옵션과 인자 처리 패턴

### 명령행 구문 패턴

**기본 패턴:**
```
command [options] [arguments]
```

**서브커맨드 패턴:**
```
command [global-options] subcommand [subcommand-options] [arguments]
```

### Git 스타일 서브커맨드

```bash
git [--version] [--help] <command> [<args>]

git clone <repository> [<directory>]
git add <pathspec>...
git commit [-m <msg>] [--amend]
git push [<repository>] [<refspec>...]
```

### .NET에서 서브커맨드 구현

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine;

namespace SubcommandExample
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Root command (global options)
            var rootCommand = new RootCommand("파일 관리 도구");

            var verboseOption = new Option<bool>(
                aliases: new[] { "-v", "--verbose" },
                description: "상세 출력"
            );

            rootCommand.AddGlobalOption(verboseOption);

            // Subcommand: list
            var listCommand = new Command("list", "파일 목록 표시")
            {
                CreateListCommand()
            };

            // Subcommand: copy
            var copyCommand = new Command("copy", "파일 복사")
            {
                CreateCopyCommand()
            };

            // Subcommand: delete
            var deleteCommand = new Command("delete", "파일 삭제")
            {
                CreateDeleteCommand()
            };

            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(copyCommand);
            rootCommand.AddCommand(deleteCommand);

            return await rootCommand.InvokeAsync(args);
        }

        static Command CreateListCommand()
        {
            var command = new Command("list", "파일 목록 표시");

            var pathArgument = new Argument<DirectoryInfo>(
                "path",
                getDefaultValue: () => new DirectoryInfo("."),
                description: "목록을 표시할 디렉토리"
            );

            var recursiveOption = new Option<bool>(
                aliases: new[] { "-r", "--recursive" },
                description: "하위 디렉토리 포함"
            );

            command.Arguments.Add(pathArgument);
            command.Options.Add(recursiveOption);

            command.SetAction((path, recursive, verbose) =>
            {
                ListFiles(path, recursive, verbose);
            }, pathArgument, recursiveOption,
               new Option<bool>(new[] { "-v", "--verbose" })); // global option 접근

            return command;
        }

        static Command CreateCopyCommand()
        {
            var command = new Command("copy", "파일 복사");

            var sourceArgument = new Argument<FileInfo>(
                "source",
                description: "원본 파일"
            );

            var destArgument = new Argument<FileInfo>(
                "destination",
                description: "대상 파일"
            );

            var forceOption = new Option<bool>(
                aliases: new[] { "-f", "--force" },
                description: "기존 파일 덮어쓰기"
            );

            command.Arguments.Add(sourceArgument);
            command.Arguments.Add(destArgument);
            command.Options.Add(forceOption);

            command.SetAction((source, dest, force, verbose) =>
            {
                CopyFile(source, dest, force, verbose);
            }, sourceArgument, destArgument, forceOption,
               new Option<bool>(new[] { "-v", "--verbose" }));

            return command;
        }

        static Command CreateDeleteCommand()
        {
            var command = new Command("delete", "파일 삭제");

            var fileArgument = new Argument<FileInfo[]>(
                "files",
                description: "삭제할 파일들"
            ) { Arity = ArgumentArity.OneOrMore };

            var forceOption = new Option<bool>(
                aliases: new[] { "-f", "--force" },
                description: "확인 없이 삭제"
            );

            command.Arguments.Add(fileArgument);
            command.Options.Add(forceOption);

            command.SetAction((files, force, verbose) =>
            {
                DeleteFiles(files, force, verbose);
            }, fileArgument, forceOption,
               new Option<bool>(new[] { "-v", "--verbose" }));

            return command;
        }

        static void ListFiles(DirectoryInfo path, bool recursive, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine($"디렉토리: {path.FullName}");
                Console.WriteLine($"재귀 검색: {recursive}");
                Console.WriteLine();
            }

            var searchOption = recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            var files = path.GetFiles("*", searchOption);

            foreach (var file in files)
            {
                if (verbose)
                {
                    Console.WriteLine($"{file.LastWriteTime:yyyy-MM-dd HH:mm:ss} {file.Length,10:N0} {file.FullName}");
                }
                else
                {
                    Console.WriteLine(file.Name);
                }
            }

            Console.WriteLine($"\n총 {files.Length}개 파일");
        }

        static void CopyFile(FileInfo source, FileInfo dest, bool force, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine($"원본: {source.FullName}");
                Console.WriteLine($"대상: {dest.FullName}");
            }

            if (!source.Exists)
            {
                Console.Error.WriteLine($"에러: 원본 파일을 찾을 수 없습니다: {source.Name}");
                Environment.Exit(3);
            }

            if (dest.Exists && !force)
            {
                Console.Error.WriteLine($"에러: 대상 파일이 이미 존재합니다: {dest.Name}");
                Console.Error.WriteLine("덮어쓰려면 -f 또는 --force 옵션을 사용하세요.");
                Environment.Exit(1);
            }

            source.CopyTo(dest.FullName, force);

            if (verbose)
            {
                Console.WriteLine($"복사 완료: {source.Length:N0} bytes");
            }
        }

        static void DeleteFiles(FileInfo[] files, bool force, bool verbose)
        {
            foreach (var file in files)
            {
                if (!file.Exists)
                {
                    Console.Error.WriteLine($"경고: 파일을 찾을 수 없습니다: {file.Name}");
                    continue;
                }

                if (!force)
                {
                    Console.Write($"'{file.Name}'을(를) 삭제하시겠습니까? (y/N): ");
                    var answer = Console.ReadLine();
                    if (answer?.ToLower() != "y")
                    {
                        if (verbose)
                        {
                            Console.WriteLine("건너뜀");
                        }
                        continue;
                    }
                }

                file.Delete();

                if (verbose)
                {
                    Console.WriteLine($"삭제됨: {file.Name}");
                }
            }
        }
    }
}
```

**사용 예시:**
```bash
# list 서브커맨드
dotnet run -- list
dotnet run -- -v list /path/to/dir -r

# copy 서브커맨드
dotnet run -- copy source.txt dest.txt
dotnet run -- -v copy source.txt dest.txt -f

# delete 서브커맨드
dotnet run -- delete file1.txt file2.txt
dotnet run -- delete file1.txt file2.txt -f
```

## 2.4 도움말과 매뉴얼 페이지 작성 규칙

### man 페이지 형식

전통적인 유닉스 man 페이지는 다음 섹션으로 구성됩니다:

1. **NAME**: 명령어 이름과 간단한 설명
2. **SYNOPSIS**: 사용법
3. **DESCRIPTION**: 상세 설명
4. **OPTIONS**: 옵션 목록
5. **EXAMPLES**: 사용 예시
6. **SEE ALSO**: 관련 명령어
7. **BUGS**: 알려진 버그
8. **AUTHOR**: 작성자

### --help 옵션 규약

```bash
# 간단한 도움말
command --help
command -h

# 상세한 도움말 (일부 도구)
command --help-all
command help
```

### .NET에서 도움말 구현

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Help;

namespace HelpExample
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand(
                "데이터 처리 도구 - 텍스트 파일을 처리하고 변환합니다"
            );

            // 옵션들에 상세한 설명 추가
            var inputOption = new Option<FileInfo>(
                aliases: new[] { "-i", "--input" },
                description: "입력 파일 경로\n" +
                           "지원 형식: .txt, .csv, .json"
            ) { IsRequired = true };

            var outputOption = new Option<FileInfo?>(
                aliases: new[] { "-o", "--output" },
                description: "출력 파일 경로\n" +
                           "지정하지 않으면 표준 출력(stdout)으로 출력됩니다"
            );

            var formatOption = new Option<string>(
                aliases: new[] { "-f", "--format" },
                getDefaultValue: () => "text",
                description: "출력 형식\n" +
                           "가능한 값: text, json, xml, csv"
            );

            formatOption.AddValidator(result =>
            {
                var value = result.GetValueOrDefault<string>();
                var validFormats = new[] { "text", "json", "xml", "csv" };

                if (!validFormats.Contains(value?.ToLower()))
                {
                    result.ErrorMessage = $"지원하지 않는 형식입니다: {value}\n" +
                                        $"가능한 값: {string.Join(", ", validFormats)}";
                }
            });

            rootCommand.Options.Add(inputOption);
            rootCommand.Options.Add(outputOption);
            rootCommand.Options.Add(formatOption);

            // 예제 추가
            rootCommand.AddExample("-i data.txt -o result.json -f json");
            rootCommand.AddExample("-i data.csv -f xml");
            rootCommand.AddExample("--input log.txt --format text");

            rootCommand.SetAction((input, output, format) =>
            {
                ProcessData(input, output, format);
            }, inputOption, outputOption, formatOption);

            return await rootCommand.Parse(args).InvokeAsync();
        }

        static void ProcessData(FileInfo input, FileInfo? output, string format)
        {
            Console.WriteLine($"입력: {input.FullName}");
            Console.WriteLine($"출력: {output?.FullName ?? "(stdout)"}");
            Console.WriteLine($"형식: {format}");
        }
    }
}
```

**생성되는 도움말 예시:**
```bash
$ dotnet run -- --help

Description:
  데이터 처리 도구 - 텍스트 파일을 처리하고 변환합니다

Usage:
  DataProcessor [options]

Options:
  -i, --input <input> (REQUIRED)   입력 파일 경로
                                    지원 형식: .txt, .csv, .json
  -o, --output <output>             출력 파일 경로
                                    지정하지 않으면 표준 출력(stdout)으로 출력됩니다
  -f, --format <format>             출력 형식 [default: text]
                                    가능한 값: text, json, xml, csv
  --version                         Show version information
  -?, -h, --help                    Show help and usage information

Examples:
  -i data.txt -o result.json -f json
  -i data.csv -f xml
  --input log.txt --format text
```

### 사용자 정의 도움말 포맷터

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.IO;

namespace CustomHelpExample
{
    // 커스텀 도움말 포맷터
    public class ColorfulHelpBuilder : HelpBuilder
    {
        public ColorfulHelpBuilder(IConsole console) : base(console)
        {
        }

        public override void Write(HelpContext context)
        {
            if (context.Command is RootCommand)
            {
                WriteHeader(context);
            }

            base.Write(context);

            if (context.Command is RootCommand)
            {
                WriteFooter(context);
            }
        }

        private void WriteHeader(HelpContext context)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔══════════════════════════════════════╗
║     데이터 처리 도구 v1.0.0          ║
╚══════════════════════════════════════╝
");
            Console.ResetColor();
        }

        private void WriteFooter(HelpContext context)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("추가 도움말:");
            Console.ResetColor();
            Console.WriteLine("  온라인 문서: https://docs.example.com");
            Console.WriteLine("  이슈 리포트: https://github.com/example/issues");
            Console.WriteLine("  이메일: support@example.com");
            Console.WriteLine();
        }
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("데이터 처리 도구");

            var inputOption = new Option<FileInfo>(
                new[] { "-i", "--input" },
                "입력 파일"
            ) { IsRequired = true };

            rootCommand.Options.Add(inputOption);

            rootCommand.SetAction((input) =>
            {
                Console.WriteLine($"처리: {input.FullName}");
            }, inputOption);

            // 커스텀 도움말 적용
            var commandLineBuilder = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseHelpBuilder(context => new ColorfulHelpBuilder(context.Console))
                .Build();

            return await commandLineBuilder.Parse(args).InvokeAsync();
        }
    }
}
```

## 2.5 진행 표시와 상호작용 피드백

### 진행률 표시 패턴

**스피너 (Spinner)**
```
⠋ 처리 중...
⠙ 처리 중...
⠹ 처리 중...
⠸ 처리 중...
```

**프로그레스 바 (Progress Bar)**
```
[████████████░░░░░░░░] 60% (300/500)
```

**퍼센트 표시**
```
처리 중: 45% 완료...
```

### .NET에서 진행률 구현

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ProgressExample
{
    // 간단한 프로그레스 바
    public class SimpleProgressBar : IDisposable
    {
        private readonly int total;
        private int current;
        private readonly Stopwatch sw;
        private readonly bool isInteractive;

        public SimpleProgressBar(int total)
        {
            this.total = total;
            this.current = 0;
            this.sw = Stopwatch.StartNew();
            this.isInteractive = !Console.IsOutputRedirected;

            if (isInteractive)
            {
                Console.CursorVisible = false;
            }
        }

        public void Report(int value)
        {
            current = value;

            if (!isInteractive)
            {
                // 비대화형 모드: 10% 단위로만 출력
                if (value % (total / 10) == 0)
                {
                    var percent = value * 100 / total;
                    Console.WriteLine($"진행률: {percent}%");
                }
                return;
            }

            var percent = current * 100 / total;
            var barWidth = 40;
            var filled = barWidth * current / total;
            var bar = new string('█', filled) + new string('░', barWidth - filled);

            var elapsed = sw.Elapsed;
            var estimatedTotal = current > 0
                ? TimeSpan.FromTicks(elapsed.Ticks * total / current)
                : TimeSpan.Zero;
            var remaining = estimatedTotal - elapsed;

            Console.Write($"\r[{bar}] {percent,3}% ({current}/{total}) ");
            Console.Write($"경과: {elapsed:mm\\:ss} ");
            Console.Write($"남음: {remaining:mm\\:ss}    ");
        }

        public void Dispose()
        {
            if (isInteractive)
            {
                Console.WriteLine();
                Console.CursorVisible = true;
            }
            sw.Stop();
        }
    }

    // 스피너
    public class Spinner : IDisposable
    {
        private readonly string[] frames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
        private readonly CancellationTokenSource cts;
        private readonly Task animationTask;
        private readonly string message;
        private bool isInteractive;

        public Spinner(string message = "처리 중")
        {
            this.message = message;
            this.isInteractive = !Console.IsOutputRedirected;
            this.cts = new CancellationTokenSource();

            if (isInteractive)
            {
                Console.CursorVisible = false;
                animationTask = Task.Run(() => Animate());
            }
            else
            {
                Console.WriteLine($"{message}...");
                animationTask = Task.CompletedTask;
            }
        }

        private void Animate()
        {
            int frameIndex = 0;

            while (!cts.Token.IsCancellationRequested)
            {
                Console.Write($"\r{frames[frameIndex]} {message}...    ");
                frameIndex = (frameIndex + 1) % frames.Length;
                Thread.Sleep(80);
            }

            Console.Write("\r");
        }

        public void Dispose()
        {
            cts.Cancel();
            animationTask.Wait();

            if (isInteractive)
            {
                Console.CursorVisible = true;
                Console.WriteLine($"✓ {message} 완료    ");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 프로그레스 바 예제 ===\n");

            // 프로그레스 바
            using (var progress = new SimpleProgressBar(100))
            {
                for (int i = 0; i <= 100; i++)
                {
                    progress.Report(i);
                    Thread.Sleep(50);
                }
            }

            Console.WriteLine("\n=== 스피너 예제 ===\n");

            // 스피너
            using (var spinner = new Spinner("데이터 로딩"))
            {
                Thread.Sleep(3000); // 작업 시뮬레이션
            }

            Console.WriteLine("\n=== 다단계 작업 ===\n");

            var tasks = new[]
            {
                ("파일 다운로드", 2000),
                ("압축 해제", 1500),
                ("데이터 처리", 2500),
                ("결과 저장", 1000)
            };

            foreach (var (taskName, duration) in tasks)
            {
                using (var spinner = new Spinner(taskName))
                {
                    Thread.Sleep(duration);
                }
            }

            Console.WriteLine("\n모든 작업 완료!");
        }
    }
}
```

### 대화형 프롬프트

```csharp
using System;

namespace InteractivePromptExample
{
    public static class Prompt
    {
        // 예/아니오 확인
        public static bool Confirm(string message, bool defaultValue = false)
        {
            var defaultText = defaultValue ? "Y/n" : "y/N";
            Console.Write($"{message} ({defaultText}): ");

            var input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }

            return input == "y" || input == "yes";
        }

        // 텍스트 입력
        public static string Input(string message, string? defaultValue = null)
        {
            if (defaultValue != null)
            {
                Console.Write($"{message} [{defaultValue}]: ");
            }
            else
            {
                Console.Write($"{message}: ");
            }

            var input = Console.ReadLine()?.Trim();

            return string.IsNullOrEmpty(input) ? defaultValue ?? "" : input;
        }

        // 비밀번호 입력
        public static string Password(string message)
        {
            Console.Write($"{message}: ");
            var password = "";

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            return password;
        }

        // 선택 메뉴
        public static int Choice(string message, string[] options)
        {
            Console.WriteLine(message);

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"  {i + 1}) {options[i]}");
            }

            while (true)
            {
                Console.Write($"선택 (1-{options.Length}): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int choice) &&
                    choice >= 1 && choice <= options.Length)
                {
                    return choice - 1;
                }

                Console.WriteLine("올바른 번호를 선택하세요.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 대화형 프롬프트 예제 ===\n");

            // 예/아니오 확인
            if (Prompt.Confirm("계속 진행하시겠습니까?", defaultValue: true))
            {
                Console.WriteLine("진행합니다...\n");
            }
            else
            {
                Console.WriteLine("취소되었습니다.");
                return;
            }

            // 텍스트 입력
            var name = Prompt.Input("이름을 입력하세요", defaultValue: "사용자");
            Console.WriteLine($"안녕하세요, {name}님!\n");

            // 비밀번호 입력
            var password = Prompt.Password("비밀번호");
            Console.WriteLine($"비밀번호 길이: {password.Length}자\n");

            // 선택 메뉴
            var formats = new[] { "JSON", "XML", "CSV", "YAML" };
            var choice = Prompt.Choice("출력 형식을 선택하세요:", formats);
            Console.WriteLine($"\n선택한 형식: {formats[choice]}");

            // 파일 삭제 확인 예제
            Console.WriteLine("\n=== 파일 삭제 시뮬레이션 ===");
            var files = new[] { "file1.txt", "file2.txt", "file3.txt" };

            foreach (var file in files)
            {
                if (Prompt.Confirm($"'{file}'을(를) 삭제하시겠습니까?"))
                {
                    Console.WriteLine($"  ✓ {file} 삭제됨");
                }
                else
                {
                    Console.WriteLine($"  - {file} 건너뜀");
                }
            }
        }
    }
}
```

### 핵심 요약

1. **POSIX 규약**: Short options (-v), Long options (--verbose), Arguments
2. **Exit Code**: 0=성공, 1=일반 에러, 2=사용법 에러, 기타 특정 에러 코드
3. **서브커맨드**: git/docker 스타일의 계층적 명령 구조
4. **도움말**: --help, 예제 포함, 명확한 설명
5. **진행 표시**: 프로그레스 바, 스피너, 대화형 환경 감지

다음 챕터에서는 .NET BCL만으로 콘솔 애플리케이션을 구현하는 방법을 자세히 살펴보겠습니다.
