# Chapter 5: 표준 입출력과 파이프라인

## 5.1 stdin, stdout, stderr의 이해

### 파일 디스크립터의 역사

유닉스 시스템에서 모든 입출력은 [파일 디스크립터](https://en.wikipedia.org/wiki/File_descriptor)를 통해 이루어집니다:

- **0 ([stdin](https://en.wikipedia.org/wiki/Standard_streams#Standard_input_(stdin)))**: 표준 입력
- **1 ([stdout](https://en.wikipedia.org/wiki/Standard_streams#Standard_output_(stdout)))**: 표준 출력
- **2 ([stderr](https://en.wikipedia.org/wiki/Standard_streams#Standard_error_(stderr)))**: 표준 에러

이 개념은 "Everything is a file"이라는 유닉스 철학에서 비롯되었습니다.

### .NET에서의 표준 스트림

```csharp
using System;
using System.IO;
using System.Text;

namespace StandardStreams
{
    class Program
    {
        static void Main(string[] args)
        {
            DemonstrateStreams();
            CustomStreamRedirection();
        }

        static void DemonstrateStreams()
        {
            Console.WriteLine("=== 표준 스트림 분석 ===\n");

            // stdin
            var stdin = Console.In;
            Console.WriteLine($"stdin 타입: {stdin.GetType().Name}");
            Console.WriteLine($"stdin 인코딩: {Console.InputEncoding.EncodingName}");
            Console.WriteLine($"stdin 리다이렉트: {Console.IsInputRedirected}");

            // stdout
            var stdout = Console.Out;
            Console.WriteLine($"\nstdout 타입: {stdout.GetType().Name}");
            Console.WriteLine($"stdout 인코딩: {Console.OutputEncoding.EncodingName}");
            Console.WriteLine($"stdout 리다이렉트: {Console.IsOutputRedirected}");

            // stderr
            var stderr = Console.Error;
            Console.Error.WriteLine($"\nstderr 타입: {stderr.GetType().Name}");
            Console.Error.WriteLine($"stderr 리다이렉트: {Console.IsErrorRedirected}");

            // 원본 스트림 접근
            using var stdoutStream = Console.OpenStandardOutput();
            Console.WriteLine($"\nstdout 스트림: {stdoutStream.GetType().Name}");
            Console.WriteLine($"CanRead: {stdoutStream.CanRead}");
            Console.WriteLine($"CanWrite: {stdoutStream.CanWrite}");
            Console.WriteLine($"CanSeek: {stdoutStream.CanSeek}");
        }

        static void CustomStreamRedirection()
        {
            Console.WriteLine("\n=== 스트림 리다이렉션 ===\n");

            // stdout 캡처
            var originalOut = Console.Out;
            var capturedOutput = new StringWriter();

            Console.SetOut(capturedOutput);
            Console.WriteLine("이 메시지는 캡처됩니다");
            Console.WriteLine("이것도 캡처됩니다");

            Console.SetOut(originalOut);
            Console.WriteLine("캡처된 출력:");
            Console.WriteLine(capturedOutput.ToString());

            // stderr 로그 파일로 리다이렉트
            var originalError = Console.Error;
            using (var errorLog = new StreamWriter("error.log", append: true))
            {
                errorLog.AutoFlush = true;
                Console.SetError(errorLog);

                Console.Error.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 에러 발생");
                Console.Error.WriteLine("상세 정보...");

                Console.SetError(originalError);
            }

            Console.WriteLine("\nerror.log 파일에 로그가 기록되었습니다");
        }
    }
}
```

### 리다이렉션 감지와 적응형 동작

```csharp
using System;
using System.IO;

namespace AdaptiveOutput
{
    public class AdaptiveConsole
    {
        private readonly bool isInteractive;
        private readonly bool supportsColor;

        public AdaptiveConsole()
        {
            // 대화형 여부 감지
            isInteractive = !Console.IsInputRedirected &&
                           !Console.IsOutputRedirected;

            // 색상 지원 감지
            supportsColor = isInteractive && SupportsAnsiEscapes();
        }

        private bool SupportsAnsiEscapes()
        {
            // Windows 10 이상에서 ANSI 지원
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return Environment.OSVersion.Version.Major >= 10;
            }

            // Unix 계열은 대부분 지원
            return Environment.GetEnvironmentVariable("TERM") != "dumb";
        }

        public void WriteLine(string message, ConsoleColor? color = null)
        {
            if (color.HasValue && supportsColor)
            {
                Console.ForegroundColor = color.Value;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public void WriteProgress(int current, int total)
        {
            if (isInteractive)
            {
                // 대화형: 프로그레스 바
                var percent = current * 100 / total;
                var barWidth = 40;
                var filled = barWidth * current / total;
                var bar = new string('█', filled) + new string('░', barWidth - filled);

                Console.Write($"\r[{bar}] {percent}%");

                if (current == total)
                {
                    Console.WriteLine();
                }
            }
            else
            {
                // 비대화형: 간헐적 로그
                if (current % (total / 10) == 0 || current == total)
                {
                    var percent = current * 100 / total;
                    Console.WriteLine($"진행률: {percent}%");
                }
            }
        }

        public void WriteError(string message)
        {
            if (supportsColor)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"ERROR: {message}");
                Console.ResetColor();
            }
            else
            {
                Console.Error.WriteLine($"ERROR: {message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var console = new AdaptiveConsole();

            console.WriteLine("프로세스 시작", ConsoleColor.Cyan);

            // 작업 시뮬레이션
            for (int i = 1; i <= 100; i++)
            {
                console.WriteProgress(i, 100);
                System.Threading.Thread.Sleep(20);
            }

            console.WriteLine("프로세스 완료", ConsoleColor.Green);

            // 에러 발생 시뮬레이션
            console.WriteError("샘플 에러 메시지");
        }
    }
}
```

**테스트:**
```bash
# 대화형 모드: 프로그레스 바와 색상
dotnet run

# 파이프: 간헐적 로그, 색상 없음
dotnet run | cat

# 파일로 리다이렉트
dotnet run > output.txt 2> error.txt
```

## 5.2 리다이렉션과 파이프 처리

### 파이프라인 체인

파이프라인은 여러 프로그램을 연결하여 복잡한 작업을 수행합니다:

```bash
# 로그에서 에러를 찾아 정렬하고 카운트
cat app.log | grep "ERROR" | sort | uniq -c | sort -rn

# .NET 도구들의 파이프라인
dotnet run --project Filter | dotnet run --project Transform | dotnet run --project Aggregate
```

### .NET 파이프라인 도구 구현

**1. 필터 도구 (filter.csproj)**
```csharp
using System;

namespace Filter
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("사용법: filter <pattern>");
                return 1;
            }

            var pattern = args[0];
            string? line;
            int matchCount = 0;

            while ((line = Console.ReadLine()) != null)
            {
                if (line.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(line);
                    matchCount++;
                }
            }

            Console.Error.WriteLine($"[필터] {matchCount}개 라인 매칭");
            return 0;
        }
    }
}
```

**2. 변환 도구 (transform.csproj)**
```csharp
using System;
using System.Linq;

namespace Transform
{
    class Program
    {
        static int Main(string[] args)
        {
            var mode = args.Length > 0 ? args[0] : "upper";
            string? line;
            int lineCount = 0;

            while ((line = Console.ReadLine()) != null)
            {
                var transformed = mode switch
                {
                    "upper" => line.ToUpper(),
                    "lower" => line.ToLower(),
                    "reverse" => new string(line.Reverse().ToArray()),
                    "trim" => line.Trim(),
                    _ => line
                };

                Console.WriteLine(transformed);
                lineCount++;
            }

            Console.Error.WriteLine($"[변환] {lineCount}개 라인 처리");
            return 0;
        }
    }
}
```

**3. 집계 도구 (aggregate.csproj)**
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aggregate
{
    class Program
    {
        static int Main(string[] args)
        {
            var mode = args.Length > 0 ? args[0] : "count";
            var lines = new List<string>();
            string? line;

            while ((line = Console.ReadLine()) != null)
            {
                lines.Add(line);
            }

            switch (mode)
            {
                case "count":
                    Console.WriteLine($"총 라인 수: {lines.Count}");
                    break;

                case "unique":
                    var unique = lines.Distinct().OrderBy(l => l);
                    foreach (var l in unique)
                    {
                        Console.WriteLine(l);
                    }
                    Console.Error.WriteLine($"[집계] {unique.Count()}개 고유 라인");
                    break;

                case "frequency":
                    var freq = lines.GroupBy(l => l)
                                   .OrderByDescending(g => g.Count())
                                   .Take(10);

                    foreach (var group in freq)
                    {
                        Console.WriteLine($"{group.Count(),5} {group.Key}");
                    }
                    break;

                case "stats":
                    Console.WriteLine($"총 라인: {lines.Count}");
                    Console.WriteLine($"고유 라인: {lines.Distinct().Count()}");
                    Console.WriteLine($"평균 길이: {lines.Average(l => l.Length):F2}");
                    Console.WriteLine($"최대 길이: {lines.Max(l => l.Length)}");
                    Console.WriteLine($"최소 길이: {lines.Min(l => l.Length)}");
                    break;
            }

            return 0;
        }
    }
}
```

**파이프라인 사용 예시:**
```bash
# 로그에서 "ERROR"를 찾아 대문자로 변환하고 카운트
cat app.log | dotnet run --project Filter "ERROR" | dotnet run --project Transform upper | dotnet run --project Aggregate count

# 파일 목록에서 ".cs"를 찾아 빈도 계산
dir /b | dotnet run --project Filter ".cs" | dotnet run --project Aggregate frequency

# 고유한 라인만 추출
echo -e "apple\nbanana\napple\ncherry\nbanana" | dotnet run --project Filter "a" | dotnet run --project Aggregate unique
```

### 버퍼링 제어

```csharp
using System;
using System.IO;

namespace BufferingControl
{
    class Program
    {
        static void Main(string[] args)
        {
            DemonstrateBuffering();
        }

        static void DemonstrateBuffering()
        {
            // 기본 버퍼링 (효율적)
            Console.WriteLine("버퍼링된 출력 1");
            Console.WriteLine("버퍼링된 출력 2");
            Console.WriteLine("버퍼링된 출력 3");

            // 명시적 플러시
            Console.Out.Flush();

            // AutoFlush 활성화 (실시간 출력)
            var writer = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(writer);

            Console.WriteLine("즉시 플러시됨 1");
            Console.WriteLine("즉시 플러시됨 2");

            // 파이프라인에서 버퍼링 이슈 해결
            ProcessLineByLine();
        }

        static void ProcessLineByLine()
        {
            // 라인별로 즉시 출력 (파이프라인에서 중요)
            string? line;
            while ((line = Console.ReadLine()) != null)
            {
                var result = ProcessData(line);
                Console.WriteLine(result);
                Console.Out.Flush(); // 즉시 전달
            }
        }

        static string ProcessData(string input)
        {
            // 실제 처리 로직
            return input.ToUpper();
        }
    }
}
```

## 5.3 텍스트 인코딩과 라인 엔딩 처리

### 인코딩 이슈

터미널 애플리케이션에서 [문자 인코딩](https://en.wikipedia.org/wiki/Character_encoding) 처리는 매우 중요합니다. 특히 [UTF-8](https://en.wikipedia.org/wiki/UTF-8)과 [Unicode](https://en.wikipedia.org/wiki/Unicode)를 올바르게 처리해야 다국어 지원이 가능합니다.

```csharp
using System;
using System.IO;
using System.Text;

namespace EncodingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            DemonstrateEncodings();
            HandleEncodingIssues();
        }

        static void DemonstrateEncodings()
        {
            Console.WriteLine("=== 인코딩 정보 ===\n");

            Console.WriteLine($"현재 입력 인코딩: {Console.InputEncoding.EncodingName}");
            Console.WriteLine($"현재 출력 인코딩: {Console.OutputEncoding.EncodingName}");

            // UTF-8 설정 (권장)
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine($"\nUTF-8로 변경 후:");
            Console.WriteLine($"입력 인코딩: {Console.InputEncoding.EncodingName}");
            Console.WriteLine($"출력 인코딩: {Console.OutputEncoding.EncodingName}");

            // 다국어 테스트
            Console.WriteLine("\n=== 다국어 출력 테스트 ===");
            Console.WriteLine("한글: 안녕하세요");
            Console.WriteLine("日本語: こんにちは");
            Console.WriteLine("中文: 你好");
            Console.WriteLine("Emoji: 🚀 🎉 ✨");
        }

        static void HandleEncodingIssues()
        {
            Console.WriteLine("\n=== 인코딩 변환 ===\n");

            // 파일을 다른 인코딩으로 읽기
            var testFile = "test_encoding.txt";

            // UTF-8로 쓰기
            File.WriteAllText(testFile, "테스트 한글 текст", Encoding.UTF8);

            // 여러 인코딩으로 읽어보기
            var encodings = new[]
            {
                Encoding.UTF8,
                Encoding.Unicode,
                Encoding.ASCII,
                Encoding.Default
            };

            foreach (var encoding in encodings)
            {
                try
                {
                    var content = File.ReadAllText(testFile, encoding);
                    Console.WriteLine($"{encoding.EncodingName,-30}: {content}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{encoding.EncodingName,-30}: ERROR - {ex.Message}");
                }
            }

            // BOM (Byte Order Mark) 처리
            DetectBom("test_utf8_bom.txt");

            File.Delete(testFile);
        }

        static void DetectBom(string filePath)
        {
            // UTF-8 BOM 포함 파일 생성
            var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            File.WriteAllText(filePath, "UTF-8 with BOM", utf8WithBom);

            // BOM 감지
            using var stream = File.OpenRead(filePath);
            var bom = new byte[4];
            stream.Read(bom, 0, 4);

            Console.WriteLine($"\n{filePath} BOM:");
            Console.WriteLine($"  Bytes: {BitConverter.ToString(bom)}");

            if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
            {
                Console.WriteLine("  Detected: UTF-8 BOM");
            }

            File.Delete(filePath);
        }
    }
}
```

### 라인 엔딩 처리

```csharp
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LineEndingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            DemonstrateLineEndings();
            NormalizeLineEndings();
        }

        static void DemonstrateLineEndings()
        {
            Console.WriteLine("=== 라인 엔딩 분석 ===\n");

            // 플랫폼별 라인 엔딩
            Console.WriteLine($"현재 플랫폼: {Environment.OSVersion.Platform}");
            Console.WriteLine($"NewLine: {string.Join(" ", Environment.NewLine.Select(c => $"0x{(int)c:X2}"))}");

            if (Environment.NewLine == "\r\n")
            {
                Console.WriteLine("Windows 스타일: CR+LF (\\r\\n)");
            }
            else if (Environment.NewLine == "\n")
            {
                Console.WriteLine("Unix/Linux/Mac 스타일: LF (\\n)");
            }
            else if (Environment.NewLine == "\r")
            {
                Console.WriteLine("구식 Mac 스타일: CR (\\r)");
            }
        }

        static void NormalizeLineEndings()
        {
            Console.WriteLine("\n=== 라인 엔딩 정규화 ===\n");

            // 혼합된 라인 엔딩
            var mixedText = "Line 1\nLine 2\r\nLine 3\rLine 4\nLine 5";

            Console.WriteLine("원본 (HEX):");
            PrintHex(mixedText);

            // Unix 스타일로 정규화 (LF only)
            var unixText = NormalizeToLF(mixedText);
            Console.WriteLine("\nUnix 정규화 (LF):");
            PrintHex(unixText);

            // Windows 스타일로 정규화 (CRLF)
            var windowsText = NormalizeToCRLF(mixedText);
            Console.WriteLine("\nWindows 정규화 (CRLF):");
            PrintHex(windowsText);

            // 라인별 읽기 (라인 엔딩 자동 처리)
            Console.WriteLine("\n라인별 읽기:");
            using var reader = new StringReader(mixedText);
            string? line;
            int lineNumber = 1;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine($"  Line {lineNumber++}: {line}");
            }
        }

        static string NormalizeToLF(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        static string NormalizeToCRLF(string text)
        {
            return NormalizeToLF(text).Replace("\n", "\r\n");
        }

        static void PrintHex(string text)
        {
            foreach (var c in text)
            {
                if (c == '\r')
                {
                    Console.Write("[CR]");
                }
                else if (c == '\n')
                {
                    Console.Write("[LF]\n");
                }
                else
                {
                    Console.Write(c);
                }
            }
            Console.WriteLine();
        }
    }

    // 크로스 플랫폼 텍스트 리더
    public class UniversalTextReader : TextReader
    {
        private readonly TextReader baseReader;
        private char? pushedBack;

        public UniversalTextReader(TextReader baseReader)
        {
            this.baseReader = baseReader;
        }

        public override string? ReadLine()
        {
            var sb = new StringBuilder();

            while (true)
            {
                int c;

                if (pushedBack.HasValue)
                {
                    c = pushedBack.Value;
                    pushedBack = null;
                }
                else
                {
                    c = baseReader.Read();
                }

                if (c == -1)
                {
                    return sb.Length > 0 ? sb.ToString() : null;
                }

                if (c == '\r')
                {
                    // CR 다음에 LF가 오는지 확인
                    var next = baseReader.Read();
                    if (next != '\n' && next != -1)
                    {
                        pushedBack = (char)next;
                    }
                    return sb.ToString();
                }

                if (c == '\n')
                {
                    return sb.ToString();
                }

                sb.Append((char)c);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                baseReader.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
```

## 5.4 바이너리 스트림 처리

### 바이너리 데이터 입출력

```csharp
using System;
using System.IO;

namespace BinaryStreamExample
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--binary")
            {
                ProcessBinaryData();
            }
            else
            {
                GenerateBinaryData();
            }
        }

        static void GenerateBinaryData()
        {
            using var stdout = Console.OpenStandardOutput();
            using var writer = new BinaryWriter(stdout);

            // 헤더 (4바이트 매직 넘버)
            writer.Write(0x42494E44); // "BIND"

            // 버전 (2바이트)
            writer.Write((ushort)1);

            // 데이터 개수 (4바이트)
            var data = new[] { 10, 20, 30, 40, 50 };
            writer.Write(data.Length);

            // 데이터
            foreach (var value in data)
            {
                writer.Write(value);
            }

            writer.Flush();

            // 에러 출력으로 로그
            Console.Error.WriteLine($"바이너리 데이터 생성: {data.Length}개 항목");
        }

        static void ProcessBinaryData()
        {
            using var stdin = Console.OpenStandardInput();
            using var reader = new BinaryReader(stdin);

            try
            {
                // 헤더 읽기
                var magic = reader.ReadUInt32();
                if (magic != 0x42494E44)
                {
                    Console.Error.WriteLine($"잘못된 매직 넘버: 0x{magic:X8}");
                    Environment.Exit(1);
                }

                // 버전 읽기
                var version = reader.ReadUInt16();
                Console.Error.WriteLine($"버전: {version}");

                // 데이터 읽기
                var count = reader.ReadInt32();
                Console.Error.WriteLine($"데이터 개수: {count}");

                Console.WriteLine("데이터:");
                for (int i = 0; i < count; i++)
                {
                    var value = reader.ReadInt32();
                    Console.WriteLine($"  [{i}] = {value}");
                }
            }
            catch (EndOfStreamException)
            {
                Console.Error.WriteLine("예기치 않은 스트림 종료");
                Environment.Exit(1);
            }
        }
    }
}
```

**사용:**
```bash
# 바이너리 데이터 생성 및 처리
dotnet run --project Generate > data.bin
dotnet run --project Process --binary < data.bin

# 파이프라인
dotnet run --project Generate | dotnet run --project Process --binary
```

### 스트림 복제와 티잉

```csharp
using System;
using System.IO;

namespace StreamTeeExample
{
    // T자형 스트림: 읽으면서 동시에 파일에 저장
    public class TeeStream : Stream
    {
        private readonly Stream primary;
        private readonly Stream secondary;

        public TeeStream(Stream primary, Stream secondary)
        {
            this.primary = primary;
            this.secondary = secondary;
        }

        public override bool CanRead => primary.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = primary.Read(buffer, offset, count);

            if (bytesRead > 0)
            {
                secondary.Write(buffer, offset, bytesRead);
                secondary.Flush();
            }

            return bytesRead;
        }

        public override void Flush() => primary.Flush();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                secondary.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // stdin을 읽으면서 동시에 파일에 저장
            using var logFile = File.Create("input.log");
            using var tee = new TeeStream(Console.OpenStandardInput(), logFile);
            using var reader = new StreamReader(tee);

            Console.Error.WriteLine("stdin 읽기 시작 (input.log에도 저장)...");

            string? line;
            int lineCount = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineCount++;
                Console.WriteLine($"[{lineCount}] {line.ToUpper()}");
            }

            Console.Error.WriteLine($"\n총 {lineCount}개 라인 처리");
            Console.Error.WriteLine("input.log 파일에 저장됨");
        }
    }
}
```

## 5.5 다른 프로세스와의 통신

### Process 클래스로 파이프라인 구축

```csharp
using System;
using System.Diagnostics;
using System.IO;

namespace ProcessCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            RunExternalCommand();
            CreatePipeline();
            InteractiveProcess();
        }

        static void RunExternalCommand()
        {
            Console.WriteLine("=== 외부 명령 실행 ===\n");

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            if (process != null)
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                Console.WriteLine($"Exit Code: {process.ExitCode}");
                Console.WriteLine($"Output: {output}");

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Error: {error}");
                }
            }
        }

        static void CreatePipeline()
        {
            Console.WriteLine("\n=== 파이프라인 구축 ===\n");

            // echo "hello world" | dotnet run --project Transform upper
            var echo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"echo 'hello world'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var transform = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project Transform upper",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using var echoProcess = Process.Start(echo);
            using var transformProcess = Process.Start(transform);

            if (echoProcess != null && transformProcess != null)
            {
                // 파이프 연결
                var output = echoProcess.StandardOutput.ReadToEnd();
                transformProcess.StandardInput.WriteLine(output);
                transformProcess.StandardInput.Close();

                var result = transformProcess.StandardOutput.ReadToEnd();

                echoProcess.WaitForExit();
                transformProcess.WaitForExit();

                Console.WriteLine($"Result: {result}");
            }
        }

        static void InteractiveProcess()
        {
            Console.WriteLine("\n=== 대화형 프로세스 ===\n");

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "fsi --readline-", // F# Interactive
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            if (process != null)
            {
                // 비동기 출력 읽기
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine($"OUT: {e.Data}");
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine($"ERR: {e.Data}");
                    }
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // 명령 전송
                process.StandardInput.WriteLine("1 + 1;;");
                process.StandardInput.WriteLine("let square x = x * x;;");
                process.StandardInput.WriteLine("square 5;;");
                process.StandardInput.WriteLine("#quit;;");

                process.WaitForExit();
            }
        }
    }
}
```

### 고급 파이프라인 빌더

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdvancedPipeline
{
    public class PipelineBuilder
    {
        private readonly List<ProcessStartInfo> stages = new();

        public PipelineBuilder Add(string fileName, string arguments)
        {
            stages.Add(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            return this;
        }

        public string Execute(string? input = null)
        {
            if (stages.Count == 0)
            {
                throw new InvalidOperationException("파이프라인이 비어있습니다");
            }

            Process? previousProcess = null;
            var processes = new List<Process>();

            try
            {
                for (int i = 0; i < stages.Count; i++)
                {
                    var process = Process.Start(stages[i]);

                    if (process == null)
                    {
                        throw new Exception($"프로세스 시작 실패: {stages[i].FileName}");
                    }

                    processes.Add(process);

                    // 이전 프로세스의 출력을 현재 프로세스의 입력으로
                    if (previousProcess != null)
                    {
                        previousProcess.StandardOutput.BaseStream.CopyToAsync(
                            process.StandardInput.BaseStream);
                        previousProcess.StandardInput.Close();
                    }
                    else if (input != null)
                    {
                        // 첫 프로세스에 입력 제공
                        process.StandardInput.WriteLine(input);
                        process.StandardInput.Close();
                    }

                    previousProcess = process;
                }

                // 마지막 프로세스의 출력 읽기
                var lastProcess = processes[^1];
                var output = lastProcess.StandardOutput.ReadToEnd();

                // 모든 프로세스 종료 대기
                foreach (var process in processes)
                {
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        var error = process.StandardError.ReadToEnd();
                        throw new Exception($"프로세스 에러 (exit {process.ExitCode}): {error}");
                    }
                }

                return output;
            }
            finally
            {
                foreach (var process in processes)
                {
                    process.Dispose();
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 파이프라인 구축 및 실행
            var pipeline = new PipelineBuilder()
                .Add("echo", "hello world\ntest line\nhello again")
                .Add("grep", "hello")
                .Add("sort", "")
                .Add("uniq", "");

            var result = pipeline.Execute();

            Console.WriteLine("파이프라인 결과:");
            Console.WriteLine(result);
        }
    }
}
```

### 핵심 요약

1. **표준 스트림**: stdin(0), stdout(1), stderr(2) 이해와 활용
2. **리다이렉션**: 파이프와 리다이렉션 감지 및 적응형 동작
3. **인코딩**: UTF-8 사용 권장, BOM 처리, 라인 엔딩 정규화
4. **바이너리**: BinaryReader/Writer로 바이너리 프로토콜 구현
5. **프로세스 통신**: Process 클래스로 파이프라인 구축

다음 챕터에서는 Spectre.Console을 활용한 리치 터미널 UI를 다루겠습니다.
