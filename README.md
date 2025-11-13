# .NET으로 만드는 현대적인 터미널 애플리케이션

## 전자책 구성

이 전자책은 .NET을 사용하여 전문적인 터미널 애플리케이션을 개발하는 방법을 다룹니다.

### Part I: 터미널의 역사와 기초

- [Chapter 1: 터미널 환경의 역사와 철학](./part1-basics/chapter01-history-and-philosophy.md)

  - 유닉스 철학과 터미널 도구의 탄생
  - 파이프라인과 필터 패러다임
  - 현대 개발 환경에서 터미널의 역할
  - SSH와 원격 시스템 관리

- [Chapter 2: 터미널 UX의 전통과 규약](./part1-basics/chapter02-terminal-ux-conventions.md)
  - POSIX 표준과 명령행 인터페이스 규약
  - Exit Code와 에러 처리 관례
  - 옵션과 인자 처리 패턴
  - 도움말과 매뉴얼 페이지 작성 규칙
  - 진행 표시와 상호작용 피드백

### Part II: .NET 기초로 시작하는 콘솔 애플리케이션

- [Chapter 3: .NET BCL만으로 구현하는 콘솔 애플리케이션](./part2-dotnet-foundation/chapter03-bcl-console.md)

  - System.Console 클래스 심층 분석
  - CommandLineArgs 처리와 파싱
  - 표준 입출력 스트림 다루기
  - Console 색상과 커서 제어
  - 키 입력과 이벤트 처리

- [Chapter 4: 표준 입출력과 파이프라인](./part2-dotnet-foundation/chapter04-stdio-pipeline.md)
  - stdin, stdout, stderr의 이해
  - 리다이렉션과 파이프 처리
  - 텍스트 인코딩과 라인 엔딩 처리
  - 바이너리 스트림 처리
  - 다른 프로세스와의 통신

### Part III: 고급 라이브러리를 활용한 개발

- [Chapter 5: Spectre.Console로 만드는 리치 터미널 UI](./part3-advanced-libraries/chapter05-spectre-console.md)

  - Spectre.Console 아키텍처 이해
  - 테이블, 트리, 차트 렌더링
  - 프로그레스 바와 스피너
  - 프롬프트와 선택 UI
  - 마크업과 스타일링

- Chapter 6: ConsoleAppFramework와 구조적 설계

  - ConsoleAppFramework 핵심 개념
  - 커맨드와 서브커맨드 구조
  - 의존성 주입과 설정 관리

- Chapter 7: System.CommandLine으로 만드는 현대적 CLI
  - System.CommandLine 아키텍처
  - 커맨드, 옵션, 인자 모델링
  - 자동 완성과 도움말 생성

### Part IV: Generic Host와 엔터프라이즈 패턴

- [Chapter 8: Generic Host 기반 콘솔 애플리케이션](./part4-generic-host/chapter08-generic-host.md)

  - IHost와 IHostBuilder 이해
  - 의존성 주입 컨테이너 활용
  - 구성(Configuration) 관리
  - 로깅과 모니터링
  - 백그라운드 서비스와 생명주기

- Chapter 9: Top-Level Programs vs 전통적 구조
  - Top-Level Programs의 장단점
  - 프로젝트 템플릿 선택 가이드

### Part V: 고급 기능과 최적화

- Chapter 10: 비동기 프로그래밍과 성능

  - 콘솔 애플리케이션에서의 async/await
  - 취소 토큰과 타임아웃 처리
  - 병렬 처리와 동시성
  - 메모리 최적화 (Span<T>, Memory<T>)
  - Native AOT와 트리밍

- Chapter 11: 크로스 플랫폼 고려사항

  - Windows, Linux, macOS 차이점
  - ANSI 이스케이프 시퀀스
  - 환경 변수와 시스템 정보

- Chapter 12: 테스팅과 디버깅
  - 콘솔 출력 테스팅 전략
  - 통합 테스트와 E2E 테스트

### Part VI: 실전 패턴과 베스트 프랙티스

- Chapter 13: 실전 디자인 패턴

  - Command 패턴과 CLI 설계
  - Strategy 패턴과 출력 포맷터

- Chapter 14: 도구 간 연동과 자동화

  - JSON, XML, YAML 출력 포맷
  - CI/CD 파이프라인 통합

- Chapter 15: 보안과 권한 관리
  - 민감한 정보 처리
  - 안전한 패스워드 입력

### Part VII: 사례 연구와 프로젝트

- Chapter 16: 실전 프로젝트 구현

  - 파일 관리 도구 만들기
  - 로그 분석 및 모니터링 도구

- Chapter 17: 오픈소스 프로젝트 분석

  - dotnet CLI 내부 구조
  - Entity Framework Core CLI

- Chapter 18: 배포와 유지보수
  - 배포 전략 (SingleFile, Framework-dependent)
  - 버전 관리와 업데이트

### 부록 (Appendix)

- [Appendix A: 터미널 컨트롤 시퀀스 레퍼런스](./appendix/appendix-a-ansi-codes.md)
- [Appendix B: .NET CLI 도구 템플릿](./appendix/appendix-b-cli-templates.md)
- [Appendix C: 성능 벤치마크 결과](./appendix/appendix-c-benchmarks.md)
- [Appendix D: 트러블슈팅 가이드](./appendix/appendix-d-troubleshooting.md)
- [Appendix E: 유용한 NuGet 패키지 목록](./appendix/appendix-e-nuget-packages.md)
- [Appendix F: 기술 용어집 (Technical Glossary)](./appendix/appendix-f-glossary.md)

## 예제 코드

모든 예제 코드는 `/examples` 디렉토리에서 확인할 수 있습니다:

```
examples/
├── 01-basic-console/          # 기본 콘솔 예제
├── 02-pipeline/               # 파이프라인 도구들
├── 03-spectre-demos/          # Spectre.Console 데모
├── 04-generic-host/           # Generic Host 예제
├── 05-file-manager/           # 실전 프로젝트: 파일 관리 도구
└── 06-log-analyzer/           # 실전 프로젝트: 로그 분석기
```

## 학습 경로

### 초급자 (1-4주)

1. Part I: 터미널의 기초 개념 이해
2. Part II: .NET BCL로 기본 도구 만들기
3. Chapter 3-4 예제 실습

### 중급자 (4-8주)

1. Part III: 고급 라이브러리 활용
2. Part IV: Generic Host 패턴
3. Chapter 5, 8 예제 구현

### 고급자 (8주+)

1. Part V-VII: 최적화와 실전 프로젝트
2. 오픈소스 프로젝트 분석
3. 자신만의 CLI 도구 개발

## 필요 사전 지식

- C# 기본 문법
- .NET 6+ 개발 환경
- 터미널/셸 기본 사용법
- Git 기초

## 개발 환경

- .NET 9 SDK (또는 .NET 8)
- Visual Studio 2022 / VS Code / Rider
- Windows, Linux, 또는 macOS

## 라이선스

이 전자책과 예제 코드는 MIT 라이선스 하에 배포됩니다.

## 피드백 및 기여

오타, 개선 사항, 또는 추가 예제 제안은 GitHub Issues를 통해 제출해주세요.

---

**저자**: .NET 터미널 도구 개발 커뮤니티
**최종 업데이트**: 2025-11-12
**버전**: 1.0
