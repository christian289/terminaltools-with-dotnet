# Appendix A: 터미널 컨트롤 시퀀스 레퍼런스

## ANSI 이스케이프 시퀀스

ANSI 이스케이프 시퀀스는 터미널의 색상, 커서 위치, 스타일 등을 제어하는 특수 문자열입니다.

### 기본 형식

```
ESC [ <parameters> <command>
```

- ESC = `\x1b` 또는 `\033` (8진수)
- `[` = CSI (Control Sequence Introducer)
- parameters = 세미콜론으로 구분된 숫자들
- command = 명령 문자

### 색상 코드

**전경색 (30-37, 90-97)**
```
\x1b[30m  Black
\x1b[31m  Red
\x1b[32m  Green
\x1b[33m  Yellow
\x1b[34m  Blue
\x1b[35m  Magenta
\x1b[36m  Cyan
\x1b[37m  White

\x1b[90m  Bright Black (Gray)
\x1b[91m  Bright Red
\x1b[92m  Bright Green
\x1b[93m  Bright Yellow
\x1b[94m  Bright Blue
\x1b[95m  Bright Magenta
\x1b[96m  Bright Cyan
\x1b[97m  Bright White
```

**배경색 (40-47, 100-107)**
```
\x1b[40m  Black background
\x1b[41m  Red background
...
\x1b[107m Bright White background
```

**256색 팔레트**
```csharp
// 전경색
Console.Write($"\x1b[38;5;{colorCode}m");

// 배경색
Console.Write($"\x1b[48;5;{colorCode}m");
```

**True Color (24-bit RGB)**
```csharp
// 전경색
Console.Write($"\x1b[38;2;{r};{g};{b}m");

// 배경색
Console.Write($"\x1b[48;2;{r};{g};{b}m");
```

### 텍스트 스타일

```
\x1b[0m   Reset all
\x1b[1m   Bold
\x1b[2m   Dim
\x1b[3m   Italic
\x1b[4m   Underline
\x1b[5m   Blink
\x1b[7m   Reverse (invert)
\x1b[8m   Hidden
\x1b[9m   Strikethrough

\x1b[21m  Double underline
\x1b[22m  Normal intensity
\x1b[23m  Not italic
\x1b[24m  Not underlined
\x1b[25m  Not blinking
\x1b[27m  Not reversed
\x1b[28m  Not hidden
\x1b[29m  Not strikethrough
```

### 커서 제어

```csharp
// 커서 이동
Console.Write($"\x1b[{n}A");  // n칸 위로
Console.Write($"\x1b[{n}B");  // n칸 아래로
Console.Write($"\x1b[{n}C");  // n칸 오른쪽으로
Console.Write($"\x1b[{n}D");  // n칸 왼쪽으로

// 절대 위치
Console.Write($"\x1b[{row};{col}H");  // (row, col)로 이동
Console.Write($"\x1b[{row};{col}f");  // 동일

// 커서 위치 저장/복원
Console.Write("\x1b[s");  // 저장
Console.Write("\x1b[u");  // 복원

// 커서 표시/숨김
Console.Write("\x1b[?25h");  // 표시
Console.Write("\x1b[?25l");  // 숨김
```

### 화면 제어

```csharp
// 화면 지우기
Console.Write("\x1b[2J");    // 전체 화면
Console.Write("\x1b[1J");    // 커서부터 화면 시작까지
Console.Write("\x1b[0J");    // 커서부터 화면 끝까지

// 라인 지우기
Console.Write("\x1b[2K");    // 전체 라인
Console.Write("\x1b[1K");    // 커서부터 라인 시작까지
Console.Write("\x1b[0K");    // 커서부터 라인 끝까지

// 스크롤
Console.Write($"\x1b[{n}S");  // n줄 스크롤 업
Console.Write($"\x1b[{n}T");  // n줄 스크롤 다운
```

### .NET에서 사용하기

```csharp
public static class AnsiCodes
{
    // 색상
    public const string Reset = "\x1b[0m";
    public const string Red = "\x1b[31m";
    public const string Green = "\x1b[32m";
    public const string Yellow = "\x1b[33m";
    public const string Blue = "\x1b[34m";

    // 스타일
    public const string Bold = "\x1b[1m";
    public const string Underline = "\x1b[4m";

    // 커서
    public static string MoveTo(int row, int col) => $"\x1b[{row};{col}H";
    public static string MoveUp(int n) => $"\x1b[{n}A";
    public static string MoveDown(int n) => $"\x1b[{n}B";

    // 화면
    public const string ClearScreen = "\x1b[2J";
    public const string ClearLine = "\x1b[2K";
}

// 사용 예
Console.WriteLine($"{AnsiCodes.Red}에러 메시지{AnsiCodes.Reset}");
Console.WriteLine($"{AnsiCodes.Bold}{AnsiCodes.Green}성공!{AnsiCodes.Reset}");
```

### Windows에서 ANSI 지원 활성화

```csharp
// Windows 10 이상에서 ANSI 이스케이프 시퀀스 활성화
[DllImport("kernel32.dll", SetLastError = true)]
static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr GetStdHandle(int nStdHandle);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);

const int STD_OUTPUT_HANDLE = -11;
const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

public static bool EnableAnsiSupport()
{
    if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        return true; // Unix는 기본 지원

    var handle = GetStdHandle(STD_OUTPUT_HANDLE);

    if (!GetConsoleMode(handle, out uint mode))
        return false;

    mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

    return SetConsoleMode(handle, mode);
}
```

### 크로스 플랫폼 고려사항

```csharp
public static class TerminalCapabilities
{
    public static bool SupportsAnsi()
    {
        // Windows 10 이상
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return Environment.OSVersion.Version.Major >= 10;
        }

        // Unix 계열
        var term = Environment.GetEnvironmentVariable("TERM");
        return term != "dumb" && !string.IsNullOrEmpty(term);
    }

    public static bool SupportsTrueColor()
    {
        var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");
        return colorTerm == "truecolor" || colorTerm == "24bit";
    }

    public static int GetColorDepth()
    {
        if (SupportsTrueColor())
            return 24; // 16M colors

        var term = Environment.GetEnvironmentVariable("TERM");

        if (term?.Contains("256color") == true)
            return 8; // 256 colors

        if (SupportsAnsi())
            return 4; // 16 colors

        return 0; // No color support
    }
}
```

### 참고 자료

- [ANSI Escape Code (Wikipedia)](https://en.wikipedia.org/wiki/ANSI_escape_code)
- [XTerm Control Sequences](https://invisible-island.net/xterm/ctlseqs/ctlseqs.html)
- [Console Virtual Terminal Sequences (Microsoft)](https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences)
