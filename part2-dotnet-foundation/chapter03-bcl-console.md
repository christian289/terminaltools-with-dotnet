# Chapter 3: .NET BCL만으로 구현하는 콘솔 애플리케이션

## 3.1 System.Console 클래스 심층 분석

### Console 클래스 개요

[`System.Console`](https://learn.microsoft.com/dotnet/api/system.console) 클래스는 [.NET](https://dotnet.microsoft.com/)의 기본 클래스 라이브러리([BCL](https://learn.microsoft.com/dotnet/standard/framework-libraries))에 포함된 표준 입출력 인터페이스입니다. 외부 라이브러리 없이도 강력한 터미널 애플리케이션을 만들 수 있습니다.

### 주요 속성

```csharp
using System;

namespace ConsoleAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            // 입출력 스트림
            Console.WriteLine($"입력 인코딩: {Console.InputEncoding.EncodingName}");
            Console.WriteLine($"출력 인코딩: {Console.OutputEncoding.EncodingName}");

            // 리다이렉션 감지
            Console.WriteLine($"입력 리다이렉션: {Console.IsInputRedirected}");
            Console.WriteLine($"출력 리다이렉션: {Console.IsOutputRedirected}");
            Console.WriteLine($"에러 리다이렉션: {Console.IsErrorRedirected}");

            // 터미널 크기 (리다이렉션되지 않은 경우에만)
            if (!Console.IsOutputRedirected)
            {
                Console.WriteLine($"버퍼 너비: {Console.BufferWidth}");
                Console.WriteLine($"버퍼 높이: {Console.BufferHeight}");
                Console.WriteLine($"창 너비: {Console.WindowWidth}");
                Console.WriteLine($"창 높이: {Console.WindowHeight}");
            }

            // 커서 위치
            if (!Console.IsOutputRedirected)
            {
                Console.WriteLine($"커서 X: {Console.CursorLeft}");
                Console.WriteLine($"커서 Y: {Console.CursorTop}");
                Console.WriteLine($"커서 크기: {Console.CursorSize}");
                Console.WriteLine($"커서 표시: {Console.CursorVisible}");
            }

            // 타이틀 (Windows만 지원)
            try
            {
                Console.Title = "Console 분석 프로그램";
                Console.WriteLine($"타이틀: {Console.Title}");
            }
            catch (PlatformNotSupportedException)
            {
                Console.WriteLine("타이틀 설정은 이 플랫폼에서 지원되지 않습니다.");
            }
        }
    }
}
```

### 스트림 직접 접근

```csharp
using System;
using System.IO;
using System.Text;

namespace StreamAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            // 표준 스트림 직접 접근
            var stdin = Console.OpenStandardInput();
            var stdout = Console.OpenStandardOutput();
            var stderr = Console.OpenStandardError();

            // 바이너리 데이터 읽기
            byte[] buffer = new byte[1024];
            int bytesRead = stdin.Read(buffer, 0, buffer.Length);

            Console.WriteLine($"읽은 바이트 수: {bytesRead}");

            // 표준 스트림 재설정
            var originalOut = Console.Out;

            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.WriteLine("이 텍스트는 StringWriter로 리다이렉트됩니다.");

                var captured = writer.ToString();

                // 원래 스트림으로 복원
                Console.SetOut(originalOut);
                Console.WriteLine($"캡처된 출력: {captured}");
            }

            // 에러 출력 리다이렉트
            var originalError = Console.Error;

            using (var errorLog = new StreamWriter("error.log", append: true))
            {
                Console.SetError(errorLog);
                Console.Error.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 에러 메시지");

                Console.SetError(originalError);
            }

            Console.WriteLine("에러 로그가 error.log 파일에 기록되었습니다.");
        }
    }
}
```

## 3.2 CommandLineArgs 처리와 파싱

### 기본 인자 처리

```csharp
using System;
using System.Linq;

namespace CommandLineArgs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"인자 개수: {args.Length}");

            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"args[{i}] = \"{args[i]}\"");
            }

            // 환경 변수로 접근 (Windows)
            var commandLine = Environment.CommandLine;
            Console.WriteLine($"\n전체 명령행: {commandLine}");

            // 프로그램 경로
            var exePath = Environment.ProcessPath;
            Console.WriteLine($"실행 파일 경로: {exePath}");
        }
    }
}
```

### 수동 옵션 파서 구현

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManualArgParser
{
    public class CommandLineParser
    {
        private readonly string[] args;
        private readonly Dictionary<string, string> options = new();
        private readonly List<string> positionalArgs = new();

        public CommandLineParser(string[] args)
        {
            this.args = args;
            Parse();
        }

        private void Parse()
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                // Long option: --key=value 또는 --key value
                if (arg.StartsWith("--"))
                {
                    var key = arg[2..];
                    string value;

                    if (key.Contains('='))
                    {
                        var parts = key.Split('=', 2);
                        key = parts[0];
                        value = parts[1];
                    }
                    else if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        value = args[++i];
                    }
                    else
                    {
                        value = "true"; // 플래그
                    }

                    options[key] = value;
                }
                // Short option: -k value 또는 -kvx (플래그 결합)
                else if (arg.StartsWith("-") && arg.Length > 1)
                {
                    var flags = arg[1..];

                    // 단일 옵션 + 값
                    if (flags.Length == 1 && i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        options[flags] = args[++i];
                    }
                    // 플래그 결합: -vxf
                    else
                    {
                        foreach (var flag in flags)
                        {
                            options[flag.ToString()] = "true";
                        }
                    }
                }
                // 위치 인자
                else
                {
                    positionalArgs.Add(arg);
                }
            }
        }

        public string? GetOption(string key, string? defaultValue = null)
        {
            return options.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public bool HasOption(string key)
        {
            return options.ContainsKey(key);
        }

        public bool GetFlag(string key)
        {
            return options.TryGetValue(key, out var value) &&
                   (value == "true" || value == "1" || value.ToLower() == "yes");
        }

        public T GetOption<T>(string key, T defaultValue = default!)
        {
            if (!options.TryGetValue(key, out var value))
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public IReadOnlyList<string> GetPositionalArgs() => positionalArgs.AsReadOnly();

        public void PrintDebug()
        {
            Console.WriteLine("=== Options ===");
            foreach (var (key, value) in options)
            {
                Console.WriteLine($"  {key} = {value}");
            }

            Console.WriteLine("\n=== Positional Arguments ===");
            for (int i = 0; i < positionalArgs.Count; i++)
            {
                Console.WriteLine($"  [{i}] = {positionalArgs[i]}");
            }
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var parser = new CommandLineParser(args);
            parser.PrintDebug();

            // 옵션 사용
            var verbose = parser.GetFlag("verbose") || parser.GetFlag("v");
            var outputFile = parser.GetOption("output") ?? parser.GetOption("o");
            var retryCount = parser.GetOption<int>("retry", 3);

            Console.WriteLine($"\nverbose: {verbose}");
            Console.WriteLine($"output: {outputFile ?? "(none)"}");
            Console.WriteLine($"retry: {retryCount}");

            // 위치 인자
            var files = parser.GetPositionalArgs();
            Console.WriteLine($"\n파일 수: {files.Count}");
            foreach (var file in files)
            {
                Console.WriteLine($"  - {file}");
            }

            return 0;
        }
    }
}
```

**테스트:**
```bash
dotnet run -- -v --output=result.txt --retry 5 file1.txt file2.txt
dotnet run -- -vxf --output result.txt file1.txt
dotnet run -- --verbose --output=result.txt -- -not-an-option.txt
```

### 고급 인자 검증

```csharp
using System;
using System.Collections.Generic;
using System.IO;

namespace AdvancedArgParser
{
    public class ArgumentValidator
    {
        private readonly List<string> errors = new();

        public void ValidateRequired(string? value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                errors.Add($"필수 옵션이 지정되지 않았습니다: --{name}");
            }
        }

        public void ValidateFile(string? path, string name, bool mustExist = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                errors.Add($"파일 경로가 지정되지 않았습니다: --{name}");
                return;
            }

            if (mustExist && !File.Exists(path))
            {
                errors.Add($"파일을 찾을 수 없습니다: {path}");
            }
        }

        public void ValidateChoice(string? value, string name, string[] choices)
        {
            if (string.IsNullOrEmpty(value))
            {
                errors.Add($"값이 지정되지 않았습니다: --{name}");
                return;
            }

            if (!Array.Exists(choices, c => c.Equals(value, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"잘못된 값입니다: --{name}={value}. " +
                          $"가능한 값: {string.Join(", ", choices)}");
            }
        }

        public void ValidateRange(int value, string name, int min, int max)
        {
            if (value < min || value > max)
            {
                errors.Add($"값이 범위를 벗어났습니다: --{name}={value}. " +
                          $"범위: {min}~{max}");
            }
        }

        public bool IsValid => errors.Count == 0;

        public void PrintErrors()
        {
            if (errors.Count == 0) return;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("에러:");
            foreach (var error in errors)
            {
                Console.Error.WriteLine($"  • {error}");
            }
            Console.ResetColor();
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var parser = new CommandLineParser(args);
            var validator = new ArgumentValidator();

            // 검증
            var inputFile = parser.GetOption("input") ?? parser.GetOption("i");
            var outputFile = parser.GetOption("output") ?? parser.GetOption("o");
            var format = parser.GetOption("format") ?? parser.GetOption("f") ?? "json";
            var timeout = parser.GetOption<int>("timeout", 30);

            validator.ValidateRequired(inputFile, "input");
            validator.ValidateFile(inputFile, "input", mustExist: true);
            validator.ValidateChoice(format, "format", new[] { "json", "xml", "csv", "yaml" });
            validator.ValidateRange(timeout, "timeout", 1, 300);

            if (!validator.IsValid)
            {
                validator.PrintErrors();
                Console.Error.WriteLine("\n사용법:");
                PrintUsage();
                return 2; // Usage error
            }

            Console.WriteLine("모든 검증 통과!");
            Console.WriteLine($"입력: {inputFile}");
            Console.WriteLine($"출력: {outputFile ?? "(stdout)"}");
            Console.WriteLine($"형식: {format}");
            Console.WriteLine($"타임아웃: {timeout}초");

            return 0;
        }

        static void PrintUsage()
        {
            Console.Error.WriteLine(@"
  -i, --input <file>      입력 파일 (필수)
  -o, --output <file>     출력 파일 (선택, 기본: stdout)
  -f, --format <format>   출력 형식: json, xml, csv, yaml (기본: json)
      --timeout <sec>     타임아웃 (초, 범위: 1-300, 기본: 30)
  -h, --help              도움말 표시

예시:
  program -i data.txt -o result.json -f json
  program --input data.txt --format xml --timeout 60
");
        }
    }
}
```

## 3.3 표준 입출력 스트림 다루기

### stdin으로 라인 단위 읽기

```csharp
using System;
using System.IO;

namespace StdinExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 방법 1: Console.ReadLine()
            Console.WriteLine("이름을 입력하세요:");
            var name = Console.ReadLine();
            Console.WriteLine($"안녕하세요, {name}님!");

            // 방법 2: 파이프 입력 처리
            if (Console.IsInputRedirected)
            {
                Console.WriteLine("파이프 입력 감지, 모든 라인 읽기...");

                string? line;
                int lineNumber = 1;

                while ((line = Console.ReadLine()) != null)
                {
                    Console.WriteLine($"{lineNumber,4}: {line}");
                    lineNumber++;
                }
            }

            // 방법 3: TextReader 사용
            ProcessInput(Console.In);
        }

        static void ProcessInput(TextReader reader)
        {
            string? line;
            var lines = new System.Collections.Generic.List<string>();

            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            Console.WriteLine($"총 {lines.Count}개 라인 읽음");
        }
    }
}
```

### stdout으로 출력

```csharp
using System;
using System.IO;
using System.Text;

namespace StdoutExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 방법 1: Console.WriteLine()
            Console.WriteLine("일반 출력");

            // 방법 2: Console.Write() - 줄바꿈 없음
            Console.Write("A");
            Console.Write("B");
            Console.Write("C");
            Console.WriteLine(); // 줄바꿈

            // 방법 3: TextWriter 사용
            var writer = Console.Out;
            writer.WriteLine("TextWriter를 통한 출력");
            writer.Flush();

            // 방법 4: 포맷팅
            var name = "홍길동";
            var age = 30;
            var score = 95.5;

            Console.WriteLine("이름: {0}, 나이: {1}, 점수: {2:F2}", name, age, score);
            Console.WriteLine($"이름: {name}, 나이: {age}, 점수: {score:F2}");

            // 방법 5: 테이블 형식
            PrintTable();
        }

        static void PrintTable()
        {
            var data = new[]
            {
                ("홍길동", 30, 95.5),
                ("김철수", 25, 88.0),
                ("이영희", 28, 92.3)
            };

            Console.WriteLine();
            Console.WriteLine($"{"이름",-10} {"나이",5} {"점수",8}");
            Console.WriteLine(new string('-', 25));

            foreach (var (name, age, score) in data)
            {
                Console.WriteLine($"{name,-10} {age,5} {score,8:F2}");
            }
        }
    }
}
```

### stderr로 에러 출력

```csharp
using System;

namespace StderrExample
{
    class Program
    {
        static int Main(string[] args)
        {
            // 일반 출력은 stdout으로
            Console.WriteLine("프로그램 시작");

            // 에러는 stderr로
            if (args.Length == 0)
            {
                Console.Error.WriteLine("에러: 인자가 필요합니다");
                PrintUsage();
                return 1;
            }

            // 경고도 stderr로
            Console.Error.WriteLine("경고: 이 기능은 실험적입니다");

            // 진행 정보는 stderr로 (파이프라인에서 stdout을 오염시키지 않음)
            LogInfo("파일 처리 중...");

            // 실제 출력은 stdout으로
            Console.WriteLine("결과 데이터");

            LogSuccess("처리 완료");

            return 0;
        }

        static void PrintUsage()
        {
            Console.Error.WriteLine(@"
사용법: program <input-file>

옵션:
  -v, --verbose    상세 출력
  -h, --help       도움말 표시
");
        }

        static void LogInfo(string message)
        {
            Console.Error.WriteLine($"[INFO] {message}");
        }

        static void LogSuccess(string message)
        {
            Console.Error.WriteLine($"[SUCCESS] {message}");
        }
    }
}
```

**사용 예시:**
```bash
# stdout만 파일로 저장, stderr는 콘솔에 표시
dotnet run -- input.txt > output.txt

# stderr를 파일로 저장
dotnet run -- input.txt 2> error.log

# stdout과 stderr를 각각 다른 파일로
dotnet run -- input.txt > output.txt 2> error.log

# stderr를 stdout으로 리다이렉트
dotnet run -- input.txt 2>&1 | grep "ERROR"
```

## 3.4 Console 색상과 커서 제어

### 색상 출력

```csharp
using System;

namespace ColorExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 리다이렉션 확인
            if (Console.IsOutputRedirected)
            {
                Console.WriteLine("출력이 리다이렉트되어 색상을 사용하지 않습니다.");
                return;
            }

            // 기본 색상
            Console.WriteLine("\n=== 전경색 (Foreground) ===");

            var colors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));

            foreach (var color in colors)
            {
                Console.ForegroundColor = color;
                Console.Write($"{color,-15}");

                if ((int)color % 4 == 3)
                {
                    Console.WriteLine();
                }
            }

            Console.ResetColor();

            // 배경색
            Console.WriteLine("\n\n=== 배경색 (Background) ===");

            foreach (var color in colors)
            {
                Console.BackgroundColor = color;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {color,-13} ");
                Console.ResetColor();

                if ((int)color % 4 == 3)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();

            // 실용적인 색상 사용
            PrintColoredMessages();
        }

        static void PrintColoredMessages()
        {
            Console.WriteLine("\n=== 실용적 색상 활용 ===\n");

            // 성공 메시지
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ 파일 처리 완료");
            Console.ResetColor();

            // 경고 메시지
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ 경고: 오래된 파일입니다");
            Console.ResetColor();

            // 에러 메시지
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✗ 에러: 파일을 찾을 수 없습니다");
            Console.ResetColor();

            // 정보 메시지
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("ℹ 정보: 처리 중...");
            Console.ResetColor();

            // 강조 텍스트
            Console.Write("일반 텍스트와 ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" 강조된 텍스트 ");
            Console.ResetColor();
            Console.WriteLine("를 혼합");
        }
    }

    // 유틸리티 클래스
    public static class ColorConsole
    {
        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        public static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }
    }
}
```

### 커서 제어

```csharp
using System;
using System.Threading;

namespace CursorExample
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Console.IsOutputRedirected)
            {
                Console.WriteLine("출력이 리다이렉트되어 커서 제어를 사용할 수 없습니다.");
                return;
            }

            // 커서 숨기기
            Console.CursorVisible = false;

            try
            {
                // 커서 위치 제어
                CursorPositionDemo();

                // 프로그레스 바
                ProgressBarDemo();

                // 애니메이션
                SpinnerDemo();

                // 인플레이스 업데이트
                InPlaceUpdateDemo();
            }
            finally
            {
                // 커서 복원
                Console.CursorVisible = true;
            }
        }

        static void CursorPositionDemo()
        {
            Console.Clear();
            Console.WriteLine("=== 커서 위치 제어 ===\n");

            // 특정 위치에 출력
            Console.SetCursorPosition(10, 5);
            Console.Write("(10, 5) 위치");

            Console.SetCursorPosition(20, 8);
            Console.Write("(20, 8) 위치");

            Console.SetCursorPosition(0, 12);
            Console.WriteLine("\n아무 키나 누르세요...");
            Console.ReadKey(true);
        }

        static void ProgressBarDemo()
        {
            Console.Clear();
            Console.WriteLine("=== 프로그레스 바 ===\n");

            var barTop = Console.CursorTop;

            for (int i = 0; i <= 100; i += 2)
            {
                Console.SetCursorPosition(0, barTop);

                var barWidth = 40;
                var filled = barWidth * i / 100;
                var bar = new string('█', filled) + new string('░', barWidth - filled);

                Console.Write($"[{bar}] {i,3}%");

                Thread.Sleep(50);
            }

            Console.WriteLine("\n\n완료!");
            Thread.Sleep(1000);
        }

        static void SpinnerDemo()
        {
            Console.Clear();
            Console.WriteLine("=== 스피너 애니메이션 ===\n");

            var frames = new[] { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
            var spinnerTop = Console.CursorTop;

            for (int i = 0; i < 50; i++)
            {
                Console.SetCursorPosition(0, spinnerTop);
                Console.Write($"{frames[i % frames.Length]} 처리 중...");
                Thread.Sleep(100);
            }

            Console.SetCursorPosition(0, spinnerTop);
            Console.WriteLine("✓ 처리 완료!    ");
            Thread.Sleep(1000);
        }

        static void InPlaceUpdateDemo()
        {
            Console.Clear();
            Console.WriteLine("=== 인플레이스 업데이트 ===\n");

            Console.WriteLine("파일 처리 중:");

            var files = new[] { "file1.txt", "file2.txt", "file3.txt", "file4.txt" };
            var statusTop = Console.CursorTop;

            foreach (var file in files)
            {
                Console.SetCursorPosition(0, statusTop);
                Console.Write($"현재: {file,-20}");

                Thread.Sleep(1000);
            }

            Console.SetCursorPosition(0, statusTop);
            Console.WriteLine($"완료: {files.Length}개 파일 처리됨  ");
        }
    }
}
```

## 3.5 키 입력과 이벤트 처리

### 키 입력 읽기

```csharp
using System;

namespace KeyInputExample
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Console.IsInputRedirected)
            {
                Console.WriteLine("입력이 리다이렉트되어 키 입력을 읽을 수 없습니다.");
                return;
            }

            BasicKeyInput();
            InteractiveMenu();
            ArrowKeyNavigation();
        }

        static void BasicKeyInput()
        {
            Console.WriteLine("=== 기본 키 입력 ===");
            Console.WriteLine("아무 키나 누르세요 (ESC로 종료)...\n");

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                Console.WriteLine($"키: {keyInfo.Key}");
                Console.WriteLine($"문자: {keyInfo.KeyChar} (코드: {(int)keyInfo.KeyChar})");
                Console.WriteLine($"Shift: {(keyInfo.Modifiers & ConsoleModifiers.Shift) != 0}");
                Console.WriteLine($"Ctrl: {(keyInfo.Modifiers & ConsoleModifiers.Control) != 0}");
                Console.WriteLine($"Alt: {(keyInfo.Modifiers & ConsoleModifiers.Alt) != 0}");
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }

        static void InteractiveMenu()
        {
            Console.Clear();
            Console.WriteLine("=== 대화형 메뉴 ===\n");

            var options = new[] { "새 파일", "열기", "저장", "종료" };
            var selected = 0;

            while (true)
            {
                Console.SetCursorPosition(0, 2);

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine($"  {options[i],-20}");
                    Console.ResetColor();
                }

                Console.WriteLine("\n↑↓ 화살표로 선택, Enter로 확인");

                var key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selected = (selected - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selected = (selected + 1) % options.Length;
                        break;

                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.WriteLine($"선택됨: {options[selected]}");
                        Thread.Sleep(1000);
                        return;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        static void ArrowKeyNavigation()
        {
            Console.Clear();
            Console.WriteLine("=== 화살표 키 네비게이션 ===");
            Console.WriteLine("화살표 키로 이동, ESC로 종료\n");

            int x = Console.WindowWidth / 2;
            int y = Console.WindowHeight / 2;

            Console.CursorVisible = false;

            try
            {
                while (true)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write("*");

                    var key = Console.ReadKey(intercept: true);

                    // 이전 위치 지우기
                    Console.SetCursorPosition(x, y);
                    Console.Write(" ");

                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            x = Math.Max(0, x - 1);
                            break;

                        case ConsoleKey.RightArrow:
                            x = Math.Min(Console.WindowWidth - 1, x + 1);
                            break;

                        case ConsoleKey.UpArrow:
                            y = Math.Max(3, y - 1);
                            break;

                        case ConsoleKey.DownArrow:
                            y = Math.Min(Console.WindowHeight - 1, y + 1);
                            break;

                        case ConsoleKey.Escape:
                            Console.Clear();
                            return;
                    }

                    // 위치 표시
                    Console.SetCursorPosition(0, 0);
                    Console.Write($"위치: ({x}, {y})   ");
                }
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }
    }
}
```

### Ctrl+C 처리

```csharp
using System;
using System.Threading;

namespace CtrlCExample
{
    class Program
    {
        private static bool keepRunning = true;

        static void Main(string[] args)
        {
            // Ctrl+C 핸들러 등록
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\n\nCtrl+C 감지!");
                Console.WriteLine("정리 작업 수행 중...");

                // 기본 동작(프로세스 종료) 취소
                e.Cancel = true;

                // 플래그 설정
                keepRunning = false;
            };

            Console.WriteLine("Ctrl+C를 눌러 우아하게 종료하세요...");
            Console.WriteLine("작업 시작:");

            int count = 0;

            while (keepRunning)
            {
                Console.WriteLine($"작업 {++count} 수행 중...");
                Thread.Sleep(1000);
            }

            Console.WriteLine("정리 완료. 프로그램 종료.");
        }
    }
}
```

### 핵심 요약

1. **System.Console**: .NET BCL의 강력한 콘솔 API
2. **인자 파싱**: 수동 파서 구현으로 옵션과 인자 처리
3. **스트림**: stdin, stdout, stderr 직접 제어
4. **색상**: ConsoleColor를 통한 컬러 출력
5. **커서**: 위치 제어로 인터랙티브 UI 구현
6. **키 입력**: Console.ReadKey()로 실시간 입력 처리

다음 챕터에서는 표준 입출력과 파이프라인을 더 깊이 다루겠습니다.
