# Appendix C: 유용한 NuGet 패키지 목록

## CLI 프레임워크

### System.CommandLine
Microsoft 공식 CLI 라이브러리

```bash
dotnet add package System.CommandLine
```

**특징:**
- 강력한 커맨드 모델링
- 자동 도움말 생성
- 탭 완성 지원
- 검증과 바인딩

**사용 사례:** 복잡한 CLI 도구, Git/Docker 스타일 서브커맨드

### Spectre.Console
리치 터미널 UI 라이브러리

```bash
dotnet add package Spectre.Console
```

**특징:**
- 테이블, 트리, 차트
- 프로그레스 바와 스피너
- 대화형 프롬프트
- 풍부한 텍스트 스타일링

**사용 사례:** 시각적으로 풍부한 CLI, 대화형 도구

### ConsoleAppFramework
경량 CLI 프레임워크

```bash
dotnet add package ConsoleAppFramework
```

**특징:**
- 간단한 API
- 의존성 주입 지원
- 필터 파이프라인
- 배치 모드

**사용 사례:** 마이크로서비스 도구, 관리 스크립트

### Cocona
ASP.NET Core 스타일의 CLI 프레임워크

```bash
dotnet add package Cocona
```

**특징:**
- ASP.NET Core와 유사한 API
- Generic Host 통합
- 미들웨어 파이프라인
- 강력한 DI 지원

**사용 사례:** 엔터프라이즈 CLI 도구

## 구성 및 설정

### Microsoft.Extensions.Configuration
.NET 구성 시스템

```bash
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
```

### Microsoft.Extensions.Options
옵션 패턴 지원

```bash
dotnet add package Microsoft.Extensions.Options
dotnet add package Microsoft.Extensions.Options.ConfigurationExtensions
```

## 로깅

### Microsoft.Extensions.Logging
.NET 로깅 추상화

```bash
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
```

### Serilog
구조화된 로깅 라이브러리

```bash
dotnet add package Serilog
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Extensions.Hosting
```

### NLog
유연한 로깅 라이브러리

```bash
dotnet add package NLog
dotnet add package NLog.Extensions.Logging
```

## 의존성 주입

### Microsoft.Extensions.DependencyInjection
.NET DI 컨테이너

```bash
dotnet add package Microsoft.Extensions.DependencyInjection
```

### Microsoft.Extensions.Hosting
Generic Host

```bash
dotnet add package Microsoft.Extensions.Hosting
```

## 데이터 처리

### Newtonsoft.Json (Json.NET)
JSON 직렬화

```bash
dotnet add package Newtonsoft.Json
```

### System.Text.Json
.NET 기본 JSON API (별도 설치 불필요, .NET Core 3.0+)

### YamlDotNet
YAML 파서

```bash
dotnet add package YamlDotNet
```

### CsvHelper
CSV 읽기/쓰기

```bash
dotnet add package CsvHelper
```

## HTTP 클라이언트

### Flurl.Http
유창한 HTTP 클라이언트

```bash
dotnet add package Flurl.Http
```

### Refit
타입 안전 REST 클라이언트

```bash
dotnet add package Refit
```

## 파일 시스템

### Glob
Glob 패턴 매칭

```bash
dotnet add package Glob
```

### DotNet.Glob
고급 Glob 지원

```bash
dotnet add package DotNet.Glob
```

### SharpCompress
압축 파일 처리

```bash
dotnet add package SharpCompress
```

## 유틸리티

### Humanizer
문자열 인간화 (복수형, 날짜 등)

```bash
dotnet add package Humanizer
```

**예제:**
```csharp
"Employee".Pluralize() // "Employees"
DateTime.UtcNow.AddHours(-2).Humanize() // "2 hours ago"
```

### CliWrap
프로세스 실행 래퍼

```bash
dotnet add package CliWrap
```

**예제:**
```csharp
await Cli.Wrap("git")
    .WithArguments("status")
    .ExecuteAsync();
```

### Polly
회복력 및 일시적 오류 처리

```bash
dotnet add package Polly
```

**예제:**
```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .RetryAsync(3);

await retryPolicy.ExecuteAsync(() => httpClient.GetAsync(url));
```

## 테스트

### xUnit
테스트 프레임워크

```bash
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
```

### FluentAssertions
유창한 어설션

```bash
dotnet add package FluentAssertions
```

### Moq
모킹 라이브러리

```bash
dotnet add package Moq
```

### Bogus
가짜 데이터 생성

```bash
dotnet add package Bogus
```

## 성능 및 벤치마킹

### BenchmarkDotNet
벤치마크 프레임워크

```bash
dotnet add package BenchmarkDotNet
```

### System.Runtime.CompilerServices.Unsafe
저수준 최적화

```bash
dotnet add package System.Runtime.CompilerServices.Unsafe
```

## 보안

### BCrypt.Net-Next
비밀번호 해싱

```bash
dotnet add package BCrypt.Net-Next
```

### BouncyCastle
암호화 라이브러리

```bash
dotnet add package BouncyCastle.NetCore
```

## 데이터베이스

### Dapper
마이크로 ORM

```bash
dotnet add package Dapper
```

### Entity Framework Core
ORM 프레임워크

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

## 코드 생성

### Scriban
템플릿 엔진

```bash
dotnet add package Scriban
```

### T4
템플릿 생성 (Visual Studio 내장)

## 터미널 특화

### Terminal.Gui
TUI (Text User Interface) 프레임워크

```bash
dotnet add package Terminal.Gui
```

**특징:**
- 윈도우, 메뉴, 다이얼로그
- 키보드/마우스 입력
- 복잡한 TUI 애플리케이션

### Colorful.Console
간단한 색상 출력

```bash
dotnet add package Colorful.Console
```

### Kurukuru
간단한 스피너

```bash
dotnet add package Kurukuru
```

## 권장 조합

### 1. 간단한 CLI 도구
```bash
dotnet add package System.CommandLine
dotnet add package Spectre.Console
```

### 2. 엔터프라이즈 CLI
```bash
dotnet add package Microsoft.Extensions.Hosting
dotnet add package System.CommandLine
dotnet add package Serilog
dotnet add package Microsoft.Extensions.Configuration.Json
```

### 3. 데이터 처리 도구
```bash
dotnet add package System.CommandLine
dotnet add package CsvHelper
dotnet add package YamlDotNet
dotnet add package Newtonsoft.Json
```

### 4. API 클라이언트 CLI
```bash
dotnet add package System.CommandLine
dotnet add package Refit
dotnet add package Spectre.Console
dotnet add package Polly
```

## 설치 스크립트 예제

```bash
#!/bin/bash
# 전체 CLI 도구 스택 설치

dotnet new console -n MyCli
cd MyCli

# 핵심 패키지
dotnet add package Microsoft.Extensions.Hosting
dotnet add package System.CommandLine
dotnet add package Spectre.Console

# 구성
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables

# 로깅
dotnet add package Serilog
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Extensions.Hosting

# 유틸리티
dotnet add package Humanizer
dotnet add package CliWrap

echo "패키지 설치 완료!"
```

## 라이선스 고려사항

대부분의 패키지는 MIT 또는 Apache 2.0 라이선스를 사용하지만, 상업적 사용 전에 반드시 라이선스를 확인하세요:

- **MIT**: 제한 없이 사용 가능
- **Apache 2.0**: 특허 보호 포함, 사용 가능
- **GPL/LGPL**: 오픈소스 프로젝트에 적합, 상업적 사용 주의

NuGet 페이지에서 각 패키지의 라이선스를 확인할 수 있습니다.
