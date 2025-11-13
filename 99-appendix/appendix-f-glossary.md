# Appendix F: 기술 용어집 (Technical Glossary)

이 부록은 책 전체에서 사용된 기술 용어들을 카테고리별로 정리한 종합 참조 자료입니다.

## 1. 기술 및 프로토콜

### POSIX
[Portable Operating System Interface](https://en.wikipedia.org/wiki/POSIX) - IEEE가 정의한 유닉스 운영체제의 표준 인터페이스입니다. 명령행 도구의 동작 방식에 대한 가이드라인을 제공합니다.

### SSH (Secure Shell)
[https://en.wikipedia.org/wiki/Secure_Shell](https://en.wikipedia.org/wiki/Secure_Shell) - 암호화된 네트워크 프로토콜로, 원격 시스템에 안전하게 접속하여 명령을 실행할 수 있게 합니다.

### ANSI Escape Codes
[https://en.wikipedia.org/wiki/ANSI_escape_code](https://en.wikipedia.org/wiki/ANSI_escape_code) - 터미널에서 색상, 커서 위치, 텍스트 스타일을 제어하는 특수 문자 시퀀스입니다.

### UTF-8
[https://en.wikipedia.org/wiki/UTF-8](https://en.wikipedia.org/wiki/UTF-8) - 유니코드 문자를 인코딩하는 가장 널리 사용되는 방식입니다.

### Unicode
[https://en.wikipedia.org/wiki/Unicode](https://en.wikipedia.org/wiki/Unicode) - 전 세계의 모든 문자를 표현하기 위한 국제 표준 문자 집합입니다.

### TCP/IP
[https://en.wikipedia.org/wiki/Internet_protocol_suite](https://en.wikipedia.org/wiki/Internet_protocol_suite) - 인터넷에서 데이터를 주고받기 위한 프로토콜 집합입니다.

### HTTP/HTTPS
[https://en.wikipedia.org/wiki/HTTP](https://en.wikipedia.org/wiki/HTTP) - 웹 통신을 위한 애플리케이션 계층 프로토콜입니다.

---

## 2. .NET 핵심 기술

### .NET
[https://dotnet.microsoft.com/](https://dotnet.microsoft.com/) - Microsoft가 개발한 크로스 플랫폼 오픈소스 개발 플랫폼입니다.

### BCL (Base Class Library)
[https://learn.microsoft.com/dotnet/standard/framework-libraries](https://learn.microsoft.com/dotnet/standard/framework-libraries) - .NET의 기본 클래스 라이브러리로, 기본적인 기능을 제공하는 클래스들의 집합입니다.

### CLR (Common Language Runtime)
[https://learn.microsoft.com/dotnet/standard/clr](https://learn.microsoft.com/dotnet/standard/clr) - .NET 애플리케이션의 실행 환경을 제공하는 런타임입니다.

### .NET Core
[https://learn.microsoft.com/dotnet/core/introduction](https://learn.microsoft.com/dotnet/core/introduction) - 크로스 플랫폼 .NET 구현으로, .NET 5 이후 통합되었습니다.

### .NET Framework
[https://learn.microsoft.com/dotnet/framework/](https://learn.microsoft.com/dotnet/framework/) - Windows 전용 .NET 구현입니다.

### Native AOT
[https://learn.microsoft.com/dotnet/core/deploying/native-aot/](https://learn.microsoft.com/dotnet/core/deploying/native-aot/) - Ahead-of-Time 컴파일로 네이티브 실행 파일을 생성하는 기술입니다.

### Generic Host
[https://learn.microsoft.com/dotnet/core/extensions/generic-host](https://learn.microsoft.com/dotnet/core/extensions/generic-host) - 의존성 주입, 구성, 로깅 등을 제공하는 애플리케이션 호스팅 모델입니다.

### Top-Level Statements
[https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/top-level-statements](https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/top-level-statements) - C# 9.0에서 도입된 간결한 프로그램 작성 방식입니다.

### Source Generator
[https://learn.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview](https://learn.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview) - 컴파일 시 코드를 자동 생성하는 기능입니다.

---

## 3. 라이브러리 및 프레임워크

### Spectre.Console
- **공식 사이트**: [https://spectreconsole.net/](https://spectreconsole.net/)
- **GitHub**: [https://github.com/spectreconsole/spectre.console](https://github.com/spectreconsole/spectre.console)
- **NuGet**: [https://www.nuget.org/packages/Spectre.Console](https://www.nuget.org/packages/Spectre.Console)

.NET을 위한 현대적인 터미널 UI 라이브러리입니다.

### ConsoleAppFramework
- **GitHub**: [https://github.com/Cysharp/ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework)
- **NuGet**: [https://www.nuget.org/packages/ConsoleAppFramework](https://www.nuget.org/packages/ConsoleAppFramework)

Cysharp가 개발한 경량 CLI 프레임워크입니다.

### System.CommandLine
- **GitHub**: [https://github.com/dotnet/command-line-api](https://github.com/dotnet/command-line-api)
- **NuGet**: [https://www.nuget.org/packages/System.CommandLine](https://www.nuget.org/packages/System.CommandLine)
- **Microsoft 문서**: [https://learn.microsoft.com/dotnet/standard/commandline/](https://learn.microsoft.com/dotnet/standard/commandline/)

Microsoft 공식 CLI 프레임워크입니다.

### ASP.NET Core
[https://learn.microsoft.com/aspnet/core/](https://learn.microsoft.com/aspnet/core/) - 웹 애플리케이션과 API를 구축하기 위한 프레임워크입니다.

### Entity Framework Core
[https://learn.microsoft.com/ef/core/](https://learn.microsoft.com/ef/core/) - .NET용 ORM (Object-Relational Mapper)입니다.

### Serilog
[https://serilog.net/](https://serilog.net/) - 구조화된 로깅을 위한 라이브러리입니다.

### NLog
[https://nlog-project.org/](https://nlog-project.org/) - 유연한 로깅 라이브러리입니다.

### BenchmarkDotNet
[https://benchmarkdotnet.org/](https://benchmarkdotnet.org/) - .NET 코드의 성능을 측정하는 라이브러리입니다.

---

## 4. 클래스 및 타입

### System.Console
[https://learn.microsoft.com/dotnet/api/system.console](https://learn.microsoft.com/dotnet/api/system.console) - 콘솔 입출력을 위한 기본 클래스입니다.

### Stream
[https://learn.microsoft.com/dotnet/api/system.io.stream](https://learn.microsoft.com/dotnet/api/system.io.stream) - 바이트 시퀀스의 추상화를 제공하는 기본 클래스입니다.

### StreamReader / StreamWriter
- [StreamReader](https://learn.microsoft.com/dotnet/api/system.io.streamreader) - 스트림에서 문자를 읽습니다.
- [StreamWriter](https://learn.microsoft.com/dotnet/api/system.io.streamwriter) - 스트림에 문자를 씁니다.

### TextReader / TextWriter
- [TextReader](https://learn.microsoft.com/dotnet/api/system.io.textreader) - 문자 읽기를 위한 추상 클래스입니다.
- [TextWriter](https://learn.microsoft.com/dotnet/api/system.io.textwriter) - 문자 쓰기를 위한 추상 클래스입니다.

### BinaryReader / BinaryWriter
- [BinaryReader](https://learn.microsoft.com/dotnet/api/system.io.binaryreader) - 이진 데이터를 읽습니다.
- [BinaryWriter](https://learn.microsoft.com/dotnet/api/system.io.binarywriter) - 이진 데이터를 씁니다.

### Task<T>
[https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) - 비동기 작업을 나타냅니다.

### ValueTask
[https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) - 메모리 효율적인 비동기 작업 타입입니다.

### IEnumerable / IAsyncEnumerable
- [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1) - 컬렉션 순회를 위한 인터페이스입니다.
- [IAsyncEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.iasyncenumerable-1) - 비동기 컬렉션 순회를 위한 인터페이스입니다.

### CancellationToken
[https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) - 비동기 작업의 취소를 전파하는 토큰입니다.

### BackgroundService
[https://learn.microsoft.com/dotnet/api/microsoft.extensions.hosting.backgroundservice](https://learn.microsoft.com/dotnet/api/microsoft.extensions.hosting.backgroundservice) - 백그라운드 작업을 위한 기본 클래스입니다.

### IHostedService
[https://learn.microsoft.com/dotnet/api/microsoft.extensions.hosting.ihostedservice](https://learn.microsoft.com/dotnet/api/microsoft.extensions.hosting.ihostedservice) - 호스트된 서비스를 정의하는 인터페이스입니다.

### IConfiguration
[https://learn.microsoft.com/dotnet/api/microsoft.extensions.configuration.iconfiguration](https://learn.microsoft.com/dotnet/api/microsoft.extensions.configuration.iconfiguration) - 구성 데이터에 접근하기 위한 인터페이스입니다.

### IOptions<T>
[https://learn.microsoft.com/dotnet/api/microsoft.extensions.options.ioptions-1](https://learn.microsoft.com/dotnet/api/microsoft.extensions.options.ioptions-1) - 옵션 패턴을 구현하는 인터페이스입니다.

### ILogger
[https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger) - 로깅을 위한 인터페이스입니다.

### IServiceProvider / IServiceCollection
- [IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider) - 의존성 주입 컨테이너입니다.
- [IServiceCollection](https://learn.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) - 서비스를 등록하는 컬렉션입니다.

### HttpClient
[https://learn.microsoft.com/dotnet/api/system.net.http.httpclient](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient) - HTTP 요청을 보내고 응답을 받는 클래스입니다.

### Process / ProcessStartInfo
- [Process](https://learn.microsoft.com/dotnet/api/system.diagnostics.process) - 프로세스를 시작하고 제어합니다.
- [ProcessStartInfo](https://learn.microsoft.com/dotnet/api/system.diagnostics.processstartinfo) - 프로세스 시작 정보를 담습니다.

### FileInfo / DirectoryInfo
- [FileInfo](https://learn.microsoft.com/dotnet/api/system.io.fileinfo) - 파일 정보와 작업을 제공합니다.
- [DirectoryInfo](https://learn.microsoft.com/dotnet/api/system.io.directoryinfo) - 디렉터리 정보와 작업을 제공합니다.

### Encoding
[https://learn.microsoft.com/dotnet/api/system.text.encoding](https://learn.microsoft.com/dotnet/api/system.text.encoding) - 문자 인코딩을 나타냅니다.

---

## 5. 개념 및 패턴

### stdin / stdout / stderr
[https://en.wikipedia.org/wiki/Standard_streams](https://en.wikipedia.org/wiki/Standard_streams) - 표준 입력, 표준 출력, 표준 에러 스트림입니다.

### async/await
[https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/) - 비동기 프로그래밍을 위한 C# 키워드입니다.

### Dependency Injection (DI)
[https://learn.microsoft.com/dotnet/core/extensions/dependency-injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection) - 객체의 의존성을 외부에서 주입하는 패턴입니다.

### Middleware
[https://learn.microsoft.com/aspnet/core/fundamentals/middleware/](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/) - 요청 처리 파이프라인의 구성 요소입니다.

### Pipeline
[https://en.wikipedia.org/wiki/Pipeline_(Unix)](https://en.wikipedia.org/wiki/Pipeline_(Unix)) - 프로그램들을 연결하여 데이터를 순차적으로 처리하는 방식입니다.

### Command Pattern
[https://en.wikipedia.org/wiki/Command_pattern](https://en.wikipedia.org/wiki/Command_pattern) - 요청을 객체로 캡슐화하는 디자인 패턴입니다.

### Strategy Pattern
[https://en.wikipedia.org/wiki/Strategy_pattern](https://en.wikipedia.org/wiki/Strategy_pattern) - 알고리즘 군을 정의하고 상호 교환 가능하게 만드는 패턴입니다.

### Builder Pattern
[https://en.wikipedia.org/wiki/Builder_pattern](https://en.wikipedia.org/wiki/Builder_pattern) - 복잡한 객체의 생성 과정을 단계별로 분리하는 패턴입니다.

### Observer Pattern
[https://en.wikipedia.org/wiki/Observer_pattern](https://en.wikipedia.org/wiki/Observer_pattern) - 객체의 상태 변화를 관찰하고 통지하는 패턴입니다.

### Decorator Pattern
[https://en.wikipedia.org/wiki/Decorator_pattern](https://en.wikipedia.org/wiki/Decorator_pattern) - 객체에 동적으로 새로운 기능을 추가하는 패턴입니다.

### Exit Code
[https://en.wikipedia.org/wiki/Exit_status](https://en.wikipedia.org/wiki/Exit_status) - 프로그램 종료 시 반환하는 상태 코드입니다.

### File Descriptor
[https://en.wikipedia.org/wiki/File_descriptor](https://en.wikipedia.org/wiki/File_descriptor) - 유닉스 시스템에서 파일이나 입출력 리소스를 참조하는 추상적 지시자입니다.

### Redirection
[https://en.wikipedia.org/wiki/Redirection_(computing)](https://en.wikipedia.org/wiki/Redirection_(computing)) - 표준 입출력을 파일이나 다른 위치로 변경하는 것입니다.

---

## 6. 도구 및 유틸리티

### Git
[https://git-scm.com/](https://git-scm.com/) - 분산 버전 관리 시스템입니다.

### GitHub
[https://github.com/](https://github.com/) - Git 호스팅 및 협업 플랫폼입니다.

### Docker
[https://www.docker.com/](https://www.docker.com/) - 컨테이너 기반 애플리케이션 배포 플랫폼입니다.

### Kubernetes
[https://kubernetes.io/](https://kubernetes.io/) - 컨테이너 오케스트레이션 플랫폼입니다.

### NuGet
[https://www.nuget.org/](https://www.nuget.org/) - .NET 패키지 관리자입니다.

### dotnet CLI
[https://learn.microsoft.com/dotnet/core/tools/](https://learn.microsoft.com/dotnet/core/tools/) - .NET 개발을 위한 명령줄 도구입니다.

### MSBuild
[https://learn.microsoft.com/visualstudio/msbuild/msbuild](https://learn.microsoft.com/visualstudio/msbuild/msbuild) - Microsoft 빌드 엔진입니다.

### npm
[https://www.npmjs.com/](https://www.npmjs.com/) - Node.js 패키지 관리자입니다.

---

## 7. 데이터 형식

### JSON
[https://www.json.org/](https://www.json.org/) - JavaScript Object Notation, 경량 데이터 교환 형식입니다.

### XML
[https://www.w3.org/XML/](https://www.w3.org/XML/) - Extensible Markup Language, 구조화된 데이터 표현 형식입니다.

### YAML
[https://yaml.org/](https://yaml.org/) - YAML Ain't Markup Language, 사람이 읽기 쉬운 데이터 직렬화 형식입니다.

### CSV
[https://en.wikipedia.org/wiki/Comma-separated_values](https://en.wikipedia.org/wiki/Comma-separated_values) - Comma-Separated Values, 표 형식 데이터를 위한 텍스트 형식입니다.

### BOM (Byte Order Mark)
[https://en.wikipedia.org/wiki/Byte_order_mark](https://en.wikipedia.org/wiki/Byte_order_mark) - 텍스트 파일의 인코딩과 바이트 순서를 나타내는 특수 문자입니다.

---

## 8. 시스템 및 플랫폼

### Windows
[https://www.microsoft.com/windows](https://www.microsoft.com/windows) - Microsoft의 운영체제입니다.

### Linux
[https://www.linux.org/](https://www.linux.org/) - 오픈소스 유닉스 계열 운영체제입니다.

### macOS
[https://www.apple.com/macos/](https://www.apple.com/macos/) - Apple의 운영체제입니다.

### WSL (Windows Subsystem for Linux)
[https://learn.microsoft.com/windows/wsl/](https://learn.microsoft.com/windows/wsl/) - Windows에서 Linux 환경을 실행할 수 있게 하는 호환성 계층입니다.

### Cross-platform
[https://en.wikipedia.org/wiki/Cross-platform_software](https://en.wikipedia.org/wiki/Cross-platform_software) - 여러 플랫폼에서 실행 가능한 소프트웨어입니다.

### Environment Variable
[https://en.wikipedia.org/wiki/Environment_variable](https://en.wikipedia.org/wiki/Environment_variable) - 운영체제에서 프로세스의 동작에 영향을 주는 동적 값입니다.

---

## 9. 성능 및 최적화

### Garbage Collection (GC)
[https://learn.microsoft.com/dotnet/standard/garbage-collection/](https://learn.microsoft.com/dotnet/standard/garbage-collection/) - 자동 메모리 관리 시스템입니다.

### JIT (Just-In-Time) Compilation
[https://learn.microsoft.com/dotnet/standard/managed-execution-process](https://learn.microsoft.com/dotnet/standard/managed-execution-process) - 실행 시점에 코드를 네이티브 코드로 컴파일하는 방식입니다.

### Reflection
[https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/reflection](https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/reflection) - 런타임에 타입 정보를 검사하고 조작하는 기능입니다.

### Trimming
[https://learn.microsoft.com/dotnet/core/deploying/trimming/trim-self-contained](https://learn.microsoft.com/dotnet/core/deploying/trimming/trim-self-contained) - 사용하지 않는 코드를 제거하여 애플리케이션 크기를 줄이는 기술입니다.

### Span<T> / Memory<T>
[https://learn.microsoft.com/dotnet/standard/memory-and-spans/](https://learn.microsoft.com/dotnet/standard/memory-and-spans/) - 메모리 효율적인 데이터 처리를 위한 타입입니다.

---

## 10. 의존성 주입 관련

### AddSingleton / AddScoped / AddTransient
[https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#service-lifetimes](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#service-lifetimes) - 서비스의 생명주기를 정의하는 메서드들입니다.

- **AddSingleton**: 애플리케이션 생명주기 동안 하나의 인스턴스만 생성
- **AddScoped**: 요청당 하나의 인스턴스 생성
- **AddTransient**: 매번 새로운 인스턴스 생성

### appsettings.json
[https://learn.microsoft.com/aspnet/core/fundamentals/configuration/](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/) - .NET 애플리케이션의 구성 파일입니다.

### ServiceProvider
[https://learn.microsoft.com/dotnet/api/system.iserviceprovider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider) - 등록된 서비스를 가져오는 컨테이너입니다.

### LogLevel
[https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.loglevel](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.loglevel) - 로그 메시지의 심각도 수준을 정의합니다.

---

## 11. CLI 및 명령 패턴

### RootCommand
System.CommandLine에서 최상위 명령을 나타내는 클래스입니다.

### Option (CLI)
[https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html) - 명령행에서 플래그나 값을 지정하는 방식입니다.

### Argument (CLI)
명령행에서 위치 기반으로 전달되는 매개변수입니다.

### Handler
이벤트나 명령을 처리하는 함수 또는 메서드입니다.

### Subcommand
[https://en.wikipedia.org/wiki/Command_(computing)](https://en.wikipedia.org/wiki/Command_(computing)) - 메인 명령 아래의 하위 명령입니다. (예: `git commit`, `git push`)

### Short Option (-)
단일 문자로 표현되는 옵션입니다. (예: `-v`, `-f`)

### Long Option (--)
단어로 표현되는 옵션입니다. (예: `--verbose`, `--force`)

---

## 12. 표준 및 명세

### IEEE
[https://www.ieee.org/](https://www.ieee.org/) - Institute of Electrical and Electronics Engineers, 기술 표준을 제정하는 국제 조직입니다.

### RFC (Request for Comments)
[https://www.ietf.org/standards/rfcs/](https://www.ietf.org/standards/rfcs/) - 인터넷 기술 표준 문서입니다.

---

## 13. 기타 중요 개념

### CI/CD
[https://en.wikipedia.org/wiki/CI/CD](https://en.wikipedia.org/wiki/CI/CD) - Continuous Integration / Continuous Deployment, 지속적 통합 및 배포 방식입니다.

### Character Encoding
[https://en.wikipedia.org/wiki/Character_encoding](https://en.wikipedia.org/wiki/Character_encoding) - 문자를 바이트로 변환하는 방식입니다.

### Benchmark
[https://en.wikipedia.org/wiki/Benchmark_(computing)](https://en.wikipedia.org/wiki/Benchmark_(computing)) - 성능을 측정하고 비교하기 위한 테스트입니다.

### Memory Allocation
[https://en.wikipedia.org/wiki/Memory_management](https://en.wikipedia.org/wiki/Memory_management) - 프로그램이 메모리를 할당하고 해제하는 과정입니다.

---

## 참고사항

- 이 용어집의 모든 링크는 2025년 1월 기준으로 유효성이 검증되었습니다.
- Microsoft Docs 링크는 언어 설정에 따라 자동으로 한국어 또는 영어로 표시될 수 있습니다.
- 일부 용어는 여러 카테고리에 속할 수 있으며, 가장 관련성 높은 카테고리에 배치되었습니다.
- 책의 각 챕터에서 해당 용어가 처음 등장할 때 관련 링크가 제공됩니다.

---

**총 키워드 수**: 100+ 개
**마지막 업데이트**: 2025-01-13
