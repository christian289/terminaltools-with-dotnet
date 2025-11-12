# .NET으로 만드는 현대적인 터미널 애플리케이션

## 소개

이 저장소는 .NET을 사용하여 전문적인 터미널 애플리케이션을 개발하는 방법을 다루는 전자책과 예제 코드를 포함합니다.

### 다루는 내용

- 터미널 도구의 역사적 배경 및 UX 특징
- .NET BCL만 사용한 기본 개발 방법
- Spectre.Console, ConsoleAppFramework 등 고급 라이브러리 활용
- Generic Host를 통한 엔터프라이즈급 애플리케이션 구조
- 파이프라인(stdin/stdout) 연계 개발
- 크로스 플랫폼 고려사항 및 최적화

## 전자책 구성

전체 목차는 [book/INDEX.md](./book/INDEX.md)를 참고하세요.

### 📚 Part I: 터미널의 역사와 기초
- [Chapter 1: 터미널 환경의 역사와 철학](./book/part1-basics/chapter01-history-and-philosophy.md)
- [Chapter 2: 터미널 UX의 전통과 규약](./book/part1-basics/chapter02-terminal-ux-conventions.md)

### 💻 Part II: .NET 기초로 시작하는 콘솔 애플리케이션
- [Chapter 3: .NET BCL만으로 구현하는 콘솔 애플리케이션](./book/part2-dotnet-foundation/chapter03-bcl-console.md)
- [Chapter 4: 표준 입출력과 파이프라인](./book/part2-dotnet-foundation/chapter04-stdio-pipeline.md)

### 🎨 Part III: 고급 라이브러리를 활용한 개발
- [Chapter 5: Spectre.Console로 만드는 리치 터미널 UI](./book/part3-advanced-libraries/chapter05-spectre-console.md)
- Chapter 6: ConsoleAppFramework와 구조적 설계
- Chapter 7: System.CommandLine으로 만드는 현대적 CLI

### 🏗️ Part IV: Generic Host와 엔터프라이즈 패턴
- [Chapter 8: Generic Host 기반 콘솔 애플리케이션](./book/part4-generic-host/chapter08-generic-host.md)
- Chapter 9: Top-Level Programs vs 전통적 구조

### ⚡ Part V-VII: 고급 주제
- Part V: 비동기 프로그래밍, 크로스 플랫폼, 테스팅
- Part VI: 실전 패턴과 베스트 프랙티스
- Part VII: 사례 연구와 프로젝트

## 예제 코드

### 1. 기본 콘솔 예제
```bash
cd examples/01-basic-console
# HelloTerminal: 시스템 정보, 리다이렉션 감지, 색상 출력
```

### 2. 파이프라인 도구들
```bash
cd examples/02-pipeline
# FilterTool: 패턴 필터링
# TransformTool: 텍스트 변환
# AggregateTool: 데이터 집계
```

### 3. Spectre.Console 데모
```bash
cd examples/03-spectre-demos
# 테이블, 차트, 프로그레스 바, 프롬프트 예제
```

## 빠른 시작

```bash
# 저장소 클론
git clone https://github.com/your-username/terminaltools-with-dotnet.git
cd terminaltools-with-dotnet

# 예제 실행 (dotnet이 설치되어 있어야 함)
cd examples/01-basic-console
dotnet run -- hello world

# 파이프라인 예제
echo -e "apple\nbanana\napple" | dotnet run --project ../02-pipeline/FilterTool "ap"
```

## 학습 경로

### 초급자 (1-4주)
1. Part I (Chapter 1-2): 터미널 기초 개념
2. Part II (Chapter 3-4): .NET BCL 활용
3. 기본 예제 실습

### 중급자 (4-8주)
1. Part III (Chapter 5-7): 고급 라이브러리
2. Part IV (Chapter 8-9): Generic Host
3. 파이프라인 도구 구현

### 고급자 (8주+)
1. Part V-VII: 최적화와 실전 프로젝트
2. 오픈소스 프로젝트 분석
3. 자신만의 CLI 도구 개발

## 부록 (Appendix)

- [Appendix A: ANSI 컨트롤 시퀀스 레퍼런스](./book/appendix/appendix-a-ansi-codes.md)
- [Appendix C: 유용한 NuGet 패키지 목록](./book/appendix/appendix-c-nuget-packages.md)

## 개발 환경

- .NET 9 SDK (또는 .NET 8 이상)
- Visual Studio 2022 / VS Code / JetBrains Rider
- Windows, Linux, 또는 macOS

## 필요 사전 지식

- C# 기본 문법
- .NET 개발 환경 설정
- 터미널/셸 기본 사용법 (cd, ls, pipe 등)
- Git 기초

## 기여하기

오타, 개선 사항, 또는 추가 예제 제안은 환영합니다!

1. Fork this repository
2. Create your feature branch (`git checkout -b feature/amazing-example`)
3. Commit your changes (`git commit -m 'Add amazing example'`)
4. Push to the branch (`git push origin feature/amazing-example`)
5. Open a Pull Request

## 라이선스

MIT License

## 작성자

.NET 터미널 도구 개발 커뮤니티

---

**최종 업데이트**: 2025-11-12
**버전**: 1.0

## 주요 주제 키워드

`dotnet` `csharp` `terminal` `cli` `console-application` `spectre-console` `system-commandline` `generic-host` `pipeline` `stdin-stdout` `ansi-escape-codes` `cross-platform`
