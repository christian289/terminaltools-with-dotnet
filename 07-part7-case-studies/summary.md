# Part VII: 사례 연구와 프로젝트 - 요약

## Chapter 16: 실전 프로젝트 구현

### 16.1 파일 관리 도구

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine;

// 파일 정리 도구 예제
var rootCommand = new RootCommand("파일 관리 도구");

var cleanCommand = new Command("clean", "오래된 파일 정리");
var dirArg = new Argument<DirectoryInfo>("directory");
var daysOption = new Option<int>("--days", () => 30, "보관 기간 (일)");

cleanCommand.Arguments.Add(dirArg);
cleanCommand.Options.Add(daysOption);

cleanCommand.SetAction((dir, days) =>
{
    var cutoffDate = DateTime.Now.AddDays(-days);
    var files = dir.GetFiles("*", SearchOption.AllDirectories)
        .Where(f => f.LastWriteTime < cutoffDate)
        .ToList();

    Console.WriteLine($"{days}일 이전 파일 {files.Count}개 발견");

    foreach (var file in files)
    {
        Console.WriteLine($"삭제: {file.FullName}");
        // file.Delete();
    }
}, dirArg, daysOption);

rootCommand.AddCommand(cleanCommand);
await rootCommand.Parse(args).InvokeAsync();
```

### 16.3 로그 분석 도구

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// 간단한 로그 분석기
class LogAnalyzer
{
    public async Task AnalyzeAsync(string logFile)
    {
        var stats = new Dictionary<string, int>();

        await foreach (var line in File.ReadLinesAsync(logFile))
        {
            var level = ExtractLogLevel(line);
            stats[level] = stats.GetValueOrDefault(level) + 1;
        }

        Console.WriteLine("로그 분석 결과:");
        foreach (var (level, count) in stats.OrderByDescending(x => x.Value))
        {
            Console.WriteLine($"  {level}: {count}");
        }
    }

    string ExtractLogLevel(string line)
    {
        if (line.Contains("ERROR")) return "ERROR";
        if (line.Contains("WARN")) return "WARN";
        if (line.Contains("INFO")) return "INFO";
        return "OTHER";
    }
}
```

### 16.5 대화형 REPL 구현

```csharp
using System;
using System.Data;
using System.Threading.Tasks;

// 간단한 REPL
class SimpleRepl
{
    public async Task RunAsync()
    {
        Console.WriteLine("간단한 계산기 REPL");
        Console.WriteLine("종료하려면 'exit' 입력\n");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            if (input == "exit") break;

            try
            {
                var result = Evaluate(input);
                Console.WriteLine($"= {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    double Evaluate(string? expression)
    {
        // 간단한 수식 평가 (실제로는 더 복잡)
        if (string.IsNullOrEmpty(expression)) return 0;
        return new DataTable().Compute(expression, null) as double? ?? 0;
    }
}
```

## Chapter 17: 오픈소스 프로젝트 분석

### 17.1 dotnet CLI 내부 구조

dotnet CLI는 다음과 같은 구조로 되어 있습니다:

- **드라이버**: `dotnet.exe` - 모든 커맨드의 진입점
- **SDK**: 빌드, 테스트, 패키징 기능
- **런타임**: 애플리케이션 실행
- **템플릿 엔진**: `dotnet new`
- **NuGet**: 패키지 관리

```bash
dotnet
├── new (템플릿 엔진)
├── restore (NuGet)
├── build (MSBuild)
├── test (VSTest)
├── publish (배포)
└── run (실행)
```

### 17.2 Entity Framework Core CLI

EF Core CLI는 `Microsoft.EntityFrameworkCore.Tools` 패키지를 통해 구현됩니다:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**핵심 구조:**
- Design-time 서비스
- Migrations 스캐폴딩
- DbContext 발견 로직
- SQL 스크립트 생성

## Chapter 18: 배포와 유지보수

### 18.1 배포 전략

**Framework-dependent (프레임워크 의존)**
```bash
dotnet publish -c Release
# 장점: 작은 크기 (~200KB)
# 단점: .NET 런타임 필요
```

**Self-contained (자체 포함)**
```bash
dotnet publish -c Release -r win-x64 --self-contained
# 장점: 런타임 포함, 독립 실행
# 단점: 큰 크기 (~60MB)
```

**SingleFile (단일 파일)**
```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
# 장점: 단일 실행 파일
# 크기: ~60MB
```

**Native AOT**
```bash
dotnet publish -c Release -r win-x64 -p:PublishAot=true
# 장점: 빠른 시작, 작은 크기 (~3MB)
# 단점: 일부 기능 제약
```

### 18.2 버전 관리

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <Version>1.2.3</Version>
    <AssemblyVersion>1.2.3.0</AssemblyVersion>
    <FileVersion>1.2.3.0</FileVersion>
  </PropertyGroup>
</Project>
```

```csharp
using System;
using System.Reflection;

// 버전 정보 표시
var version = typeof(Program).Assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
    .InformationalVersion ?? "Unknown";

Console.WriteLine($"MyTool v{version}");
```

### 18.3 설치 스크립트

**Linux/macOS:**
```bash
#!/bin/bash
# install.sh

INSTALL_DIR="/usr/local/bin"
APP_NAME="mytool"

curl -L https://github.com/user/repo/releases/download/v1.0/mytool -o $APP_NAME
chmod +x $APP_NAME
sudo mv $APP_NAME $INSTALL_DIR/

echo "Installed to $INSTALL_DIR/$APP_NAME"
```

**Windows (PowerShell):**
```powershell
# install.ps1

$InstallDir = "$env:LOCALAPPDATA\Programs\MyTool"
$AppName = "mytool.exe"

New-Item -ItemType Directory -Force -Path $InstallDir
Invoke-WebRequest -Uri "https://github.com/user/repo/releases/download/v1.0/mytool.exe" -OutFile "$InstallDir\$AppName"

# PATH에 추가
$Path = [Environment]::GetEnvironmentVariable("PATH", "User")
if ($Path -notlike "*$InstallDir*") {
    [Environment]::SetEnvironmentVariable("PATH", "$Path;$InstallDir", "User")
}

Write-Host "Installed to $InstallDir"
```

### 18.4 문서화

README.md 필수 섹션:
- 설치 방법
- 빠른 시작
- 사용 예제
- 옵션 설명
- 트러블슈팅
- 라이선스

```markdown
# MyTool

간단한 파일 처리 도구

## 설치

```bash
dotnet tool install -g mytool
```

## 사용법

```bash
mytool process -i input.txt -o output.json
```

## 옵션

- `-i, --input`: 입력 파일 (필수)
- `-o, --output`: 출력 파일
- `-f, --format`: 출력 형식 (json|xml|yaml)
```

### 18.5 일반 사용자를 위한 실행 스크립트 제공

#### 문제점

개발자나 시스템 운영자가 아닌 일반 사용자는 Terminal 환경에 익숙하지 않으며, 마우스와 GUI 프로그램만 사용할 수 있습니다. CLI 도구를 어떻게 실행해야 하는지 감을 잡지 못하는 경우가 많습니다.

**Windows Batch 파일의 한글 인코딩 문제:**
- Batch 파일(`.bat`, `.cmd`)은 기본적으로 ANSI 인코딩을 사용
- 한글 인자가 포함된 경우 인코딩 문제로 깨지는 현상 발생
- Windows 10+에는 PowerShell 5.1+이 기본 설치되어 있음
- PowerShell Core 6/7이 설치되어 있을 수도 있음

#### 해결 방법

**전략:** PowerShell 스크립트(`.ps1`)로 한글 인자를 처리하고, Batch 파일은 UTF-8 인코딩으로 영문/기호만 사용하여 PowerShell 스크립트를 실행

**예제 시나리오:**
파일 처리 도구 `MyTool.exe`를 실행하는데 한글 파일명과 한글 메시지를 인자로 전달

#### 구현 예제

**1. PowerShell 스크립트 (run-mytool.ps1)**

```powershell
# UTF-8 BOM 인코딩으로 저장
# 한글 인자를 안전하게 처리

param(
    [string]$InputFile = "입력파일.txt",
    [string]$OutputFile = "출력파일.json",
    [string]$Message = "처리 완료했습니다"
)

# 도구 실행 파일 경로
$ToolPath = Join-Path $PSScriptRoot "MyTool.exe"

# 파일 존재 확인
if (-not (Test-Path $ToolPath)) {
    Write-Error "MyTool.exe를 찾을 수 없습니다: $ToolPath"
    exit 1
}

# 한글 파라미터와 함께 실행
Write-Host "MyTool 실행 중..."
Write-Host "입력: $InputFile"
Write-Host "출력: $OutputFile"
Write-Host "메시지: $Message"
Write-Host ""

& $ToolPath `
    --input "$InputFile" `
    --output "$OutputFile" `
    --message "$Message" `
    --verbose

$exitCode = $LASTEXITCODE

if ($exitCode -eq 0) {
    Write-Host "`n✓ 성공적으로 완료되었습니다" -ForegroundColor Green
} else {
    Write-Host "`n✗ 오류가 발생했습니다 (종료 코드: $exitCode)" -ForegroundColor Red
}

# 사용자가 결과를 확인할 수 있도록 대기
Write-Host "`n아무 키나 누르면 종료됩니다..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

exit $exitCode
```

**2. Batch 실행 파일 (run-mytool.bat)**

```batch
@echo off
REM UTF-8 BOM 인코딩으로 저장
REM 영문과 기호만 사용 (한글 금지)

REM PowerShell execution policy check and run
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-mytool.ps1"

REM Exit with PowerShell script's exit code
exit /b %ERRORLEVEL%
```

**3. 다양한 시나리오를 위한 배치 파일들**

**기본 실행 (run-default.bat)**
```batch
@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-mytool.ps1"
exit /b %ERRORLEVEL%
```

**커스텀 인자 실행 (run-custom.bat)**
```batch
@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-mytool.ps1" ^
    -InputFile "데이터.txt" ^
    -OutputFile "결과.json" ^
    -Message "작업 완료"
exit /b %ERRORLEVEL%
```

**드래그 앤 드롭 지원 (run-drop-file.bat)**
```batch
@echo off
REM Drag and drop a file onto this batch file

if "%~1"=="" (
    echo Please drag and drop a file onto this batch file.
    pause
    exit /b 1
)

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-mytool.ps1" ^
    -InputFile "%~1"
exit /b %ERRORLEVEL%
```

#### 고급 PowerShell 스크립트 예제

**대화형 실행 스크립트 (run-interactive.ps1)**

```powershell
# UTF-8 BOM 인코딩으로 저장

# 색상 출력 함수
function Write-ColorText {
    param([string]$Text, [ConsoleColor]$Color)
    Write-Host $Text -ForegroundColor $Color
}

# 헤더 출력
Clear-Host
Write-ColorText "═══════════════════════════════════" Cyan
Write-ColorText "    MyTool 실행 도우미" Cyan
Write-ColorText "═══════════════════════════════════" Cyan
Write-Host ""

# 파일 선택 (Windows Forms 사용)
Add-Type -AssemblyName System.Windows.Forms
$openFileDialog = New-Object System.Windows.Forms.OpenFileDialog
$openFileDialog.Title = "처리할 파일을 선택하세요"
$openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"

if ($openFileDialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
    $inputFile = $openFileDialog.FileName
    Write-ColorText "선택된 파일: $inputFile" Green
} else {
    Write-ColorText "파일 선택이 취소되었습니다." Red
    pause
    exit 1
}

# 출력 파일명 입력
Write-Host ""
$outputFile = Read-Host "출력 파일명을 입력하세요 (기본: output.json)"
if ([string]::IsNullOrWhiteSpace($outputFile)) {
    $outputFile = "output.json"
}

# 메시지 입력
Write-Host ""
$message = Read-Host "처리 완료 메시지를 입력하세요 (선택사항)"
if ([string]::IsNullOrWhiteSpace($message)) {
    $message = "처리가 완료되었습니다"
}

# 실행
Write-Host ""
Write-ColorText "───────────────────────────────────" DarkGray
Write-ColorText "실행 중..." Yellow

$toolPath = Join-Path $PSScriptRoot "MyTool.exe"
$arguments = @(
    "--input", $inputFile,
    "--output", $outputFile,
    "--message", $message,
    "--verbose"
)

$process = Start-Process -FilePath $toolPath `
    -ArgumentList $arguments `
    -NoNewWindow `
    -Wait `
    -PassThru

Write-Host ""
if ($process.ExitCode -eq 0) {
    Write-ColorText "✓ 성공!" Green
    Write-ColorText "결과 파일: $outputFile" Cyan
} else {
    Write-ColorText "✗ 실패 (종료 코드: $($process.ExitCode))" Red
}

Write-Host ""
Write-Host "아무 키나 누르면 종료됩니다..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
```

**실행용 배치 파일 (run-interactive.bat)**
```batch
@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-interactive.ps1"
exit /b %ERRORLEVEL%
```

#### 파일 구조

```
MyTool/
├── MyTool.exe                  # CLI 도구 실행 파일
├── run-mytool.ps1              # PowerShell 실행 스크립트 (한글 처리)
├── run-mytool.bat              # Batch 실행 파일 (영문만)
├── run-default.bat             # 기본 설정 실행
├── run-custom.bat              # 커스텀 인자 실행
├── run-drop-file.bat           # 드래그 앤 드롭 실행
├── run-interactive.ps1         # 대화형 실행 스크립트
└── run-interactive.bat         # 대화형 실행 배치 파일
```

#### 주의사항

1. **인코딩**
   - PowerShell 스크립트: UTF-8 BOM 인코딩 필수
   - Batch 파일: UTF-8 (BOM 없음) 또는 ASCII, 한글 사용 금지

2. **ExecutionPolicy**
   - `-ExecutionPolicy Bypass`: 실행 정책 우회 (일회성)
   - 영구 변경은 관리자 권한 필요: `Set-ExecutionPolicy RemoteSigned`

3. **보안**
   - 신뢰할 수 있는 스크립트만 실행
   - 코드 서명 고려 (프로덕션 환경)

4. **디버깅**
   - PowerShell에서 직접 실행: `.\run-mytool.ps1`
   - 에러 확인: `$Error[0] | Format-List -Force`

#### 테스트 CLI 도구 예제 (C#)

```csharp
using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("파일 처리 도구");

        var inputOption = new Option<FileInfo>("--input") { IsRequired = true };
        var outputOption = new Option<FileInfo>("--output") { IsRequired = true };
        var messageOption = new Option<string>("--message", () => "완료");
        var verboseOption = new Option<bool>("--verbose");

        rootCommand.Options.Add(inputOption);
        rootCommand.Options.Add(outputOption);
        rootCommand.Options.Add(messageOption);
        rootCommand.Options.Add(verboseOption);

        rootCommand.SetAction((input, output, message, verbose) =>
        {
            // UTF-8 출력 설정 (한글 지원)
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (verbose)
            {
                Console.WriteLine($"입력 파일: {input.FullName}");
                Console.WriteLine($"출력 파일: {output.FullName}");
                Console.WriteLine($"메시지: {message}");
                Console.WriteLine();
            }

            // 파일 처리
            var content = File.ReadAllText(input.FullName);
            var result = $"처리 시간: {DateTime.Now}\n내용: {content}\n{message}";
            File.WriteAllText(output.FullName, result, System.Text.Encoding.UTF8);

            Console.WriteLine($"✓ {message}");
            return 0;
        }, inputOption, outputOption, messageOption, verboseOption);

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
```

#### 배포 체크리스트

- [ ] PowerShell 스크립트를 UTF-8 BOM 인코딩으로 저장
- [ ] Batch 파일에 한글이 없는지 확인
- [ ] 사용자 가이드에 실행 방법 명시 (.bat 파일 더블클릭)
- [ ] Windows 10/11에서 테스트
- [ ] 다양한 한글 인자로 테스트 (파일명, 메시지 등)
- [ ] 에러 발생 시 사용자 친화적 메시지 출력

## 핵심 요약

- **실전 프로젝트**: 파일 관리, 로그 분석, REPL
- **오픈소스 분석**: dotnet CLI, EF Core CLI 구조 학습
- **배포 전략**: Framework-dependent, Self-contained, SingleFile, Native AOT
- **버전 관리**: AssemblyVersion, 시맨틱 버저닝
- **설치**: 크로스 플랫폼 설치 스크립트
- **문서화**: README, 예제, 트러블슈팅 가이드
- **일반 사용자 지원**: PowerShell + Batch를 활용한 한글 인자 처리, 드래그 앤 드롭, 대화형 실행
