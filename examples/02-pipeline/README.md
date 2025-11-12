# 파이프라인 도구 예제

유닉스 스타일의 파이프라인 도구들

## 도구 목록

### 1. FilterTool
stdin에서 특정 패턴과 매칭되는 라인만 출력합니다.

```bash
echo -e "apple\nbanana\napricot" | dotnet run --project FilterTool "ap"
# 출력:
# apple
# apricot
```

### 2. TransformTool
stdin의 각 라인을 변환합니다.

```bash
echo "hello world" | dotnet run --project TransformTool upper
# 출력: HELLO WORLD
```

### 3. AggregateTool
stdin의 데이터를 집계합니다.

```bash
echo -e "apple\nbanana\napple" | dotnet run --project AggregateTool frequency
# 출력:
#     2 apple
#     1 banana
```

## 파이프라인 조합 예제

```bash
# 로그에서 ERROR를 찾아 대문자로 변환
cat app.log | dotnet run --project FilterTool "ERROR" | dotnet run --project TransformTool upper

# 파일 목록에서 .cs 파일만 카운트
ls | dotnet run --project FilterTool ".cs" | dotnet run --project AggregateTool count

# 중복 제거
cat data.txt | dotnet run --project AggregateTool unique
```

## 학습 포인트

- stdin/stdout을 통한 파이프라인 구현
- stderr를 통한 메타데이터 출력
- 유닉스 도구 스타일의 작은 도구 조합
- 리다이렉션과 파이프 처리
