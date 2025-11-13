# Chapter 9: Top-Level Programs vs 전통적 구조

## 9.1 Top-Level Programs의 장단점

### Top-Level Programs란?

C# 9.0 (.NET 5)부터 도입된 Top-Level Statements는 프로그램의 진입점을 간소화합니다.

**전통적 방식:**
```csharp
using System;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
```

**Top-Level 방식:**
```csharp
using System;

Console.WriteLine("Hello World!");
```

### 장점

1. **간결한 코드**: 보일러플레이트 제거
2. **빠른 프로토타이핑**: 스크립트처럼 사용 가능
3. **초보자 친화적**: 학습 곡선 완화
4. **현대적 기본값**: .NET 6+ 프로젝트 템플릿 기본값

### 단점

1. **암시적 네임스페이스**: 명시성 감소
2. **대규모 프로젝트에서 제약**: 구조화 어려움
3. **하나의 파일만 가능**: 여러 진입점 불가
4. **혼란 가능성**: 전통적 방식에 익숙한 개발자에게 혼란

## 9.2 암시적 Main과 global using

### 암시적 Main 메서드

Top-Level Statements는 컴파일러가 자동으로 Main 메서드를 생성합니다.

```csharp
using System;

// 작성한 코드
Console.WriteLine("Args count: " + args.Length);
foreach (var arg in args)
{
    Console.WriteLine(arg);
}

// 컴파일러가 생성하는 코드 (개념적)
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Args count: " + args.Length);
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }
    }
}
```

### async Main 지원

```csharp
// Top-Level에서 async/await 직접 사용
using System.Net.Http;

var client = new HttpClient();
var response = await client.GetStringAsync("https://api.github.com");
Console.WriteLine(response.Substring(0, 100));
```

### global using

```csharp
// GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using System.IO;
global using System.Net.Http;

// Program.cs - global using으로 using 문 없이 바로 사용
var files = Directory.GetFiles(".");
var tasks = files.Select(f => File.ReadAllTextAsync(f));
var contents = await Task.WhenAll(tasks);

Console.WriteLine($"Read {contents.Length} files");
```

### ImplicitUsings 활성화

```xml
<!-- .csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

## 9.3 간단한 스크립트 vs 구조화된 애플리케이션

### 간단한 스크립트 (Top-Level 적합)

```csharp
using System;
using System.IO;

// 파일 정리 스크립트
var targetDir = args.Length > 0 ? args[0] : ".";
var files = Directory.GetFiles(targetDir);

Console.WriteLine($"Found {files.Length} files in {targetDir}");

foreach (var file in files)
{
    var info = new FileInfo(file);
    var age = DateTime.Now - info.LastWriteTime;

    if (age.TotalDays > 30)
    {
        Console.WriteLine($"Old file: {info.Name} ({age.TotalDays:F0} days)");
        // File.Delete(file); // 실제 삭제는 주석 처리
    }
}
```

### 구조화된 애플리케이션 (전통적 방식 권장)

```csharp
// Program.cs - 전통적 방식
using MyApp.Services;
using MyApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
            return 0;
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IDataService, DataService>();
                    services.AddHostedService<Worker>();
                });
    }
}

// Services/IDataService.cs
namespace MyApp.Services
{
    public interface IDataService
    {
        Task<IEnumerable<Data>> GetDataAsync();
    }
}

// Services/DataService.cs
namespace MyApp.Services
{
    public class DataService : IDataService
    {
        public async Task<IEnumerable<Data>> GetDataAsync()
        {
            // 구현
            await Task.Delay(100);
            return Enumerable.Empty<Data>();
        }
    }
}
```

## 9.4 마이그레이션 전략

### Top-Level에서 전통적 방식으로

**Step 1: Main 메서드 추가**
```csharp
// Before (Top-Level)
using System;

Console.WriteLine("Hello");
await Task.Delay(1000);
Console.WriteLine("World");

// After (Traditional)
using System;

namespace MyApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello");
            await Task.Delay(1000);
            Console.WriteLine("World");
        }
    }
}
```

**Step 2: 클래스로 구조화**
```csharp
using System;

namespace MyApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = new Application();
            await app.RunAsync(args);
        }
    }

    class Application
    {
        public async Task RunAsync(string[] args)
        {
            Console.WriteLine("Hello");
            await Task.Delay(1000);
            Console.WriteLine("World");
        }
    }
}
```

### 전통적 방식에서 Top-Level로

**Step 1: Main 메서드 내용을 파일 레벨로 이동**
```csharp
// Before
namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Hello {args[0]}!");
        }
    }
}

// After
Console.WriteLine($"Hello {args[0]}!");
```

## 9.5 프로젝트 템플릿 선택 가이드

### 언제 Top-Level을 사용할까?

**✅ 사용 권장:**
- 간단한 유틸리티 스크립트
- 프로토타입 및 실험
- 학습 목적 코드
- 100줄 미만의 간단한 도구
- 파일 변환, 간단한 자동화

**예시:**
```csharp
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// CSV를 JSON으로 변환하는 간단한 도구
if (args.Length == 0)
{
    Console.WriteLine("Usage: csvtojson <input.csv>");
    return 1;
}

var lines = await File.ReadAllLinesAsync(args[0]);
var headers = lines[0].Split(',');
var result = lines.Skip(1).Select(line =>
{
    var values = line.Split(',');
    return headers.Zip(values).ToDictionary(x => x.First, x => x.Second);
});

Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
return 0;
```

### 언제 전통적 방식을 사용할까?

**✅ 사용 권장:**
- 엔터프라이즈 애플리케이션
- Generic Host 사용
- 여러 파일과 클래스
- 의존성 주입 필요
- 복잡한 비즈니스 로직
- 팀 프로젝트

**예시:**
```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Services;

namespace MyApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IDataService, DataService>();
                    services.AddSingleton<ICacheService, RedisCacheService>();
                    services.AddHostedService<DataProcessorWorker>();
                });
    }
}
```

### 하이브리드 접근

Top-Level에서 시작하되, 복잡해지면 클래스로 분리:

```csharp
// Program.cs (Top-Level)
using System.Threading.Tasks;
using MyApp;

var app = new Application();
return await app.RunAsync(args);

// Application.cs
namespace MyApp
{
    using System.Threading.Tasks;

    public class Application
    {
        public async Task<int> RunAsync(string[] args)
        {
            // 복잡한 로직은 여기서
            var processor = new DataProcessor();
            await processor.ProcessAsync();
            return 0;
        }
    }
}
```

### 핵심 요약

1. **Top-Level**: 간결한 스크립트, 프로토타입에 적합
2. **전통적 방식**: 구조화된 애플리케이션, 팀 프로젝트에 적합
3. **마이그레이션**: 프로젝트 성장에 따라 유연하게 전환
4. **선택 기준**: 복잡도, 팀 규모, 유지보수성 고려
5. **하이브리드**: Top-Level + 클래스 분리로 균형 유지

다음 Part에서는 비동기 프로그래밍과 성능 최적화를 다루겠습니다.
