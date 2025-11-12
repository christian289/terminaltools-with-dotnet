# Hello Terminal

기본 터미널 애플리케이션 예제

## 기능

- 시스템 정보 출력
- 리다이렉션 감지
- 명령행 인자 처리
- 색상 출력

## 실행 방법

```bash
# 기본 실행
dotnet run

# 인자와 함께 실행
dotnet run -- arg1 arg2 arg3

# 출력 리다이렉트
dotnet run > output.txt

# 파이프
echo "test" | dotnet run
```

## 학습 포인트

- `Console` 클래스 기본 사용법
- `Environment` 클래스로 시스템 정보 접근
- 리다이렉션 감지 (`IsInputRedirected`, `IsOutputRedirected`)
- 명령행 인자 파싱
- 콘솔 색상 제어
