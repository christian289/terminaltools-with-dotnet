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

## 핵심 요약

- **실전 프로젝트**: 파일 관리, 로그 분석, REPL
- **오픈소스 분석**: dotnet CLI, EF Core CLI 구조 학습
- **배포 전략**: Framework-dependent, Self-contained, SingleFile, Native AOT
- **버전 관리**: AssemblyVersion, 시맨틱 버저닝
- **설치**: 크로스 플랫폼 설치 스크립트
- **문서화**: README, 예제, 트러블슈팅 가이드
