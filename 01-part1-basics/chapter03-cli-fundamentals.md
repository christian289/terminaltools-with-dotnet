# Chapter 3: 터미널 애플리케이션의 기본 구조와 용어

## 3.1 CLI 애플리케이션의 기본 구성 요소

터미널 애플리케이션을 이해하고 개발하기 위해서는 먼저 기본적인 구조와 용어를 명확히 알아야 합니다.

### 기본 실행 형태

```bash
command [options] <arguments>
```

이 패턴은 대부분의 CLI 도구에서 공통적으로 사용됩니다.

## 3.2 Command (명령)

### 정의

**Command**는 실행하고자 하는 프로그램 또는 동작을 지정하는 가장 기본적인 요소입니다.

### 예시

```bash
# git이 command
git status

# dotnet이 command
dotnet build

# ls가 command
ls -la
```

### Subcommand (하위 명령)

많은 현대적인 CLI 도구는 계층적 명령 구조를 사용합니다:

```bash
# git: 주 명령, commit: 하위 명령
git commit -m "message"

# dotnet: 주 명령, ef: 하위 명령, migrations: 하위 명령, add: 하위 명령
dotnet ef migrations add InitialCreate

# docker: 주 명령, container: 하위 명령, ls: 하위 명령
docker container ls
```

**특징:**
- 관련된 기능을 논리적으로 그룹화
- 명령어 충돌 방지
- 직관적인 계층 구조 제공

## 3.3 Argument (인자)

### 정의

**Argument**는 명령에 전달되는 **위치 기반 매개변수**입니다. 순서가 중요하며, 일반적으로 명령이 작동할 대상을 지정합니다.

### 표기법

- `<argument>` - **필수 인자** (반드시 제공해야 함)
- `[argument]` - **선택적 인자** (생략 가능)
- `<argument>...` - **가변 인자** (여러 개 제공 가능)

### 예시

```bash
# source와 destination이 위치 기반 인자
cp <source> <destination>
cp file1.txt file2.txt

# 가변 인자: 여러 파일을 받을 수 있음
rm <files>...
rm file1.txt file2.txt file3.txt

# 필수 인자와 선택적 인자
git checkout <branch> [file]
git checkout main          # branch만 (필수)
git checkout main README.md  # branch와 file (선택)
```

### 실제 사례

```bash
# dotnet new: template이 필수 인자, name은 선택적 인자
dotnet new <template> [-n|--name <name>]
dotnet new console -n MyApp

# tar: archive가 필수 인자, files가 가변 인자
tar -czf <archive.tar.gz> <files>...
tar -czf backup.tar.gz file1.txt file2.txt dir/
```

## 3.4 Option/Flag (옵션/플래그)

### 정의

**Option**(또는 **Flag**)은 명령의 동작 방식을 수정하는 **이름 기반 매개변수**입니다. 순서와 무관하게 작동합니다.

### 종류

#### 1. Short Option (짧은 옵션)
- 단일 문자 사용
- 하이픈 하나(`-`)로 시작
- 여러 옵션을 결합 가능

```bash
# 단일 옵션
ls -l        # long format
ls -a        # show all (hidden files)

# 옵션 결합
ls -la       # -l과 -a를 결합
ls -l -a     # 분리해서 사용 가능

# 값을 받는 옵션
gcc -o output.exe source.c
```

#### 2. Long Option (긴 옵션)
- 완전한 단어 사용
- 하이픈 두 개(`--`)로 시작
- 더 읽기 쉽고 자기 설명적

```bash
# 플래그 (Boolean)
dotnet build --no-restore
git commit --amend

# 값을 받는 옵션
dotnet new console --name MyApp
git log --since="2 weeks ago"

# 등호 사용
npm install --save-dev=typescript
```

#### 3. 별칭 (Aliases)
많은 옵션은 짧은 형태와 긴 형태 모두 제공:

```bash
# 동일한 의미
ls -a
ls --all

# 동일한 의미
git commit -m "message"
git commit --message "message"

# 동일한 의미
dotnet build -c Release
dotnet build --configuration Release
```

### 옵션의 값 전달 방식

```bash
# 공백으로 구분
command --option value
dotnet new console --name MyApp

# 등호로 연결
command --option=value
dotnet new console --name=MyApp

# 짧은 옵션 (공백 또는 붙여쓰기)
gcc -o output
gcc -ooutput

# 여러 값 (반복 또는 쉼표)
git add --ignore-errors file1 file2
docker run -e VAR1=value1 -e VAR2=value2 image
```

## 3.5 CLI 문법 표기법 (Syntax Notation)

터미널 애플리케이션의 도움말과 문서에서 사용되는 표준 표기법입니다.

### 기본 기호

| 기호 | 의미 | 예시 |
|------|------|------|
| `<>` | 필수 매개변수 | `git clone <repository>` |
| `[]` | 선택적 매개변수 | `git log [<options>]` |
| `...` | 반복 가능 (0개 이상) | `rm <file>...` |
| `\|` | 선택 (OR) | `git diff [<commit>] \| [--cached]` |
| `()` | 그룹화 | `git log [(-p\|-u)] [--stat]` |

### 실제 예시

```bash
# Git
git clone <repository> [<directory>]
git log [<options>] [<revision-range>] [[--] <path>...]
git commit [-a | --interactive] [-m <msg>]

# Docker
docker run [OPTIONS] <image> [COMMAND] [ARG...]
docker build [OPTIONS] <path>

# .NET CLI
dotnet new <template> [-n|--name <name>] [--output <output>]
dotnet build [<project>] [-c|--configuration <configuration>]
```

## 3.6 일반적인 CLI 패턴

### 1. CRUD 작업 패턴

```bash
# Create
command create <name> [options]
docker container create nginx

# Read/List
command list [filter]
docker container ls

# Update
command update <name> [options]
kubectl scale deployment <name> --replicas=3

# Delete
command delete <name>
docker container rm <container-id>
```

### 2. 도움말 옵션

대부분의 CLI 도구는 다음 옵션을 지원합니다:

```bash
# 도움말 보기
command --help
command -h
command help

# 하위 명령 도움말
command subcommand --help
git commit --help
```

### 3. 버전 확인

```bash
command --version
command -v
command version

# 예시
dotnet --version
git --version
node --version
```

### 4. Verbose/Quiet 옵션

```bash
# 상세 출력
command --verbose
command -v

# 조용한 모드 (최소 출력)
command --quiet
command -q

# 예시
npm install --verbose
npm install --quiet
```

### 5. Dry-run 패턴

실제 실행 없이 무엇을 할지 미리 보기:

```bash
command --dry-run
command -n

# 예시
apt-get install package --dry-run
kubectl apply -f config.yaml --dry-run
```

## 3.7 실전 예제 분석

### 예제 1: Git Commit

```bash
git commit -m "Add new feature" --author="John Doe <john@example.com>"
```

**분석:**
- **Command**: `git`
- **Subcommand**: `commit`
- **Option**: `-m` (message, 값 필요)
- **Argument**: `"Add new feature"` (커밋 메시지)
- **Option**: `--author` (값 필요)
- **Argument**: `"John Doe <john@example.com>"` (작성자 정보)

### 예제 2: Docker Run

```bash
docker run -d -p 8080:80 --name webserver nginx:latest
```

**분석:**
- **Command**: `docker`
- **Subcommand**: `run`
- **Option**: `-d` (detached mode, 플래그)
- **Option**: `-p` (port mapping, 값 필요)
- **Argument**: `8080:80` (포트 매핑 값)
- **Option**: `--name` (컨테이너 이름, 값 필요)
- **Argument**: `webserver` (컨테이너 이름 값)
- **Argument**: `nginx:latest` (이미지 이름, 필수)

### 예제 3: .NET CLI

```bash
dotnet new console -n MyApp --framework net9.0 --output ./src
```

**분석:**
- **Command**: `dotnet`
- **Subcommand**: `new`
- **Argument**: `console` (템플릿 이름, 필수)
- **Option**: `-n` (name의 짧은 형태, 값 필요)
- **Argument**: `MyApp` (프로젝트 이름)
- **Option**: `--framework` (타겟 프레임워크, 값 필요)
- **Argument**: `net9.0` (프레임워크 값)
- **Option**: `--output` (출력 디렉토리, 값 필요)
- **Argument**: `./src` (출력 경로)

## 3.8 특수 구분자와 규칙

### Double Dash (--) 구분자

`--`는 옵션의 끝을 표시하며, 이후의 모든 것은 인자로 취급됩니다:

```bash
# -v가 옵션처럼 보이지만 파일 이름으로 취급
grep pattern -- -v

# --로 옵션과 인자 명확히 구분
rm -rf -- *.txt

# Git에서 파일 이름과 브랜치 구분
git checkout -- file.txt  # file.txt를 체크아웃 (브랜치가 아님)
```

### 특수 인자: stdin을 나타내는 하이픈 (-)

많은 도구에서 `-`는 표준 입력(stdin)을 의미합니다:

```bash
# 표준 입력에서 읽기
cat - < input.txt

# 파이프와 함께 사용
echo "hello" | grep -
```

### 환경 변수와 결합

```bash
# 환경 변수로 옵션 전달
export DOCKER_HOST=tcp://localhost:2375
docker ps

# 명령줄에서 환경 변수 설정
NODE_ENV=production npm start
```

## 3.9 좋은 CLI UX를 위한 원칙

### 1. 일관성 (Consistency)

```bash
# 좋은 예: 일관된 패턴
myapp create user
myapp create project
myapp delete user
myapp delete project

# 나쁜 예: 일관성 없음
myapp user-create
myapp new-project
myapp remove-user
myapp project-delete
```

### 2. 명확성 (Clarity)

```bash
# 좋은 예: 명확한 옵션 이름
myapp --input-file data.json --output-format xml

# 나쁜 예: 모호한 약어
myapp -i data.json -of xml
```

### 3. 안전한 기본값 (Safe Defaults)

```bash
# 위험한 작업에는 확인 요청
rm important-file.txt  # 확인 없이 삭제 (위험)
rm -i important-file.txt  # 확인 요청 (안전)

# 또는 --force 플래그 요구
myapp delete-all-data --force
```

### 4. 점진적 복잡성 (Progressive Disclosure)

```bash
# 기본: 간단한 사용
myapp build

# 중급: 일부 옵션 추가
myapp build --output dist

# 고급: 세밀한 제어
myapp build --output dist --minify --sourcemap --env production
```

### 5. 유용한 에러 메시지

```bash
# 나쁜 예
Error: Invalid input

# 좋은 예
Error: Invalid input file 'data.txt'
  - File does not exist
  - Expected location: ./data/data.txt

Hint: Use 'myapp --help' for usage information
```

## 3.10 요약

### 핵심 용어 정리

| 용어 | 설명 | 표기법 | 예시 |
|------|------|--------|------|
| Command | 실행할 프로그램/동작 | `command` | `git`, `dotnet` |
| Subcommand | 하위 명령 | `command subcommand` | `git commit`, `docker run` |
| Argument | 위치 기반 매개변수 | `<arg>` (필수), `[arg]` (선택) | `git clone <url>` |
| Option | 이름 기반 매개변수 | `-o` (짧은), `--option` (긴) | `git log --oneline` |
| Flag | 값 없는 옵션 (Boolean) | `-f`, `--force` | `rm -f file` |

### 표기법 요약

```bash
# 완전한 패턴
command [global-options] subcommand [options] <required-arg> [optional-arg] [args]...

# 실제 예시
git [--version] clone [--depth <depth>] <repository> [<directory>]
```

### 다음 단계

이제 기본 용어와 구조를 이해했으니, 다음 챕터에서는 .NET을 사용하여 이러한 CLI 패턴을 실제로 구현하는 방법을 배우게 됩니다.

---

**참고 자료:**
- [POSIX Utility Conventions](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html)
- [GNU Coding Standards: Command-Line Interfaces](https://www.gnu.org/prep/standards/html_node/Command_002dLine-Interfaces.html)
- [Command Line Interface Guidelines](https://clig.dev/)
- [12 Factor CLI Apps](https://medium.com/@jdxcode/12-factor-cli-apps-dd3c227a0e46)
