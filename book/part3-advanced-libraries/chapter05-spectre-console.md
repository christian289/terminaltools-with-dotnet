# Chapter 5: Spectre.Console로 만드는 리치 터미널 UI

## 5.1 Spectre.Console 아키텍처 이해

### Spectre.Console이란?

Spectre.Console은 .NET을 위한 현대적인 터미널 UI 라이브러리입니다. 아름답고 기능적인 콘솔 애플리케이션을 쉽게 만들 수 있게 해줍니다.

**주요 특징:**
- 풍부한 텍스트 포맷팅 (마크업, 색상, 스타일)
- 테이블, 트리, 차트등 복잡한 레이아웃
- 프로그레스 바와 스피너
- 대화형 프롬프트
- ANSI 이스케이프 시퀀스 자동 처리

### 설치 및 기본 설정

```bash
dotnet new console -n SpectreDemo
cd SpectreDemo
dotnet add package Spectre.Console
```

**Program.cs:**
```csharp
using Spectre.Console;

// 간단한 출력
AnsiConsole.WriteLine("Hello, [bold yellow]Spectre.Console[/]!");

// 마크업 사용
AnsiConsole.Markup("[underline red]Error:[/] File not found\n");

// 스타일 객체 사용
AnsiConsole.Write(
    new Markup("[bold green]Success![/]")
        .Centered()
);
```

### 핵심 개념

**1. IRenderable**: 렌더링 가능한 모든 것의 기반 인터페이스

```csharp
using Spectre.Console;

// Markup
IRenderable markup = new Markup("[blue]Styled text[/]");

// Text
IRenderable text = new Text("Plain text", new Style(Color.Green));

// Rule (구분선)
IRenderable rule = new Rule("[yellow]Section[/]");

// 렌더링
AnsiConsole.Write(markup);
AnsiConsole.Write(text);
AnsiConsole.Write(rule);
```

**2. AnsiConsole**: 모든 출력의 중심

```csharp
using Spectre.Console;

class Program
{
    static void Main()
    {
        // 기본 출력
        AnsiConsole.WriteLine("Plain text");

        // 마크업
        AnsiConsole.Markup("[bold]Bold[/] [italic]Italic[/] [underline]Underline[/]\n");

        // 스타일
        var style = new Style(Color.Blue, Color.White, Decoration.Bold);
        AnsiConsole.Write("Styled text", style);

        // 줄바꿈
        AnsiConsole.WriteLine();
    }
}
```

## 5.2 테이블, 트리, 차트 렌더링

### 테이블

```csharp
using Spectre.Console;

class TableExamples
{
    static void Main()
    {
        // 기본 테이블
        BasicTable();

        // 고급 테이블
        AdvancedTable();

        // 중첩 테이블
        NestedTable();
    }

    static void BasicTable()
    {
        var table = new Table();

        // 컬럼 추가
        table.AddColumn("이름");
        table.AddColumn("나이");
        table.AddColumn("직업");

        // 행 추가
        table.AddRow("홍길동", "30", "개발자");
        table.AddRow("김철수", "25", "디자이너");
        table.AddRow("이영희", "28", "기획자");

        AnsiConsole.Write(table);
    }

    static void AdvancedTable()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .Title("[yellow]직원 목록[/]")
            .Caption("[grey]총 3명[/]");

        // 컬럼 정렬 및 스타일
        table.AddColumn(new TableColumn("[blue]이름[/]")
            .Centered());
        table.AddColumn(new TableColumn("[green]나이[/]")
            .RightAligned());
        table.AddColumn(new TableColumn("[yellow]급여[/]")
            .RightAligned());
        table.AddColumn(new TableColumn("[cyan]부서[/]"));

        // 데이터 추가 (마크업 사용 가능)
        table.AddRow("[bold]홍길동[/]", "30", "$[green]75,000[/]", "개발");
        table.AddRow("[bold]김철수[/]", "25", "$[green]60,000[/]", "디자인");
        table.AddRow("[bold]이영희[/]", "28", "$[green]70,000[/]", "기획");

        // 구분선
        table.AddEmptyRow();
        table.AddRow("[bold yellow]평균[/]", "27.7", "$[bold green]68,333[/]", "");

        AnsiConsole.Write(table);
    }

    static void NestedTable()
    {
        var parentTable = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]프로젝트 현황[/]");

        parentTable.AddColumn("프로젝트");
        parentTable.AddColumn("상세 정보");

        // 중첩된 테이블
        var detailsTable = new Table()
            .Border(TableBorder.None)
            .AddColumn("항목")
            .AddColumn("값");

        detailsTable.AddRow("진행률", "[green]85%[/]");
        detailsTable.AddRow("기간", "2024-01 ~ 2024-06");
        detailsTable.AddRow("팀원", "5명");

        parentTable.AddRow("[bold]프로젝트 A[/]", detailsTable);

        AnsiConsole.Write(parentTable);
    }
}
```

### 트리 구조

```csharp
using Spectre.Console;

class TreeExamples
{
    static void Main()
    {
        FileSystemTree();
        HierarchyTree();
    }

    static void FileSystemTree()
    {
        var root = new Tree("[yellow]project/[/]");

        // src 폴더
        var src = root.AddNode("[blue]src/[/]");
        src.AddNode("[green]Program.cs[/]");
        src.AddNode("[green]Utils.cs[/]");

        var models = src.AddNode("[blue]Models/[/]");
        models.AddNode("[green]User.cs[/]");
        models.AddNode("[green]Product.cs[/]");

        // tests 폴더
        var tests = root.AddNode("[blue]tests/[/]");
        tests.AddNode("[green]UnitTests.cs[/]");
        tests.AddNode("[green]IntegrationTests.cs[/]");

        // 파일들
        root.AddNode("[cyan]README.md[/]");
        root.AddNode("[grey]package.json[/]");
        root.AddNode("[grey].gitignore[/]");

        AnsiConsole.Write(root);
    }

    static void HierarchyTree()
    {
        var root = new Tree("[bold yellow]CEO[/]")
            .Style(Style.Parse("yellow"))
            .Guide(TreeGuide.Line);

        var dev = root.AddNode("[bold cyan]개발 이사[/]");
        dev.AddNode("백엔드 팀장");
        dev.AddNode("프론트엔드 팀장");
        dev.AddNode("DevOps 팀장");

        var sales = root.AddNode("[bold green]영업 이사[/]");
        sales.AddNode("국내 영업 팀장");
        sales.AddNode("해외 영업 팀장");

        var hr = root.AddNode("[bold magenta]인사 이사[/]");
        hr.AddNode("채용 담당자");
        hr.AddNode("교육 담당자");

        AnsiConsole.Write(root);
    }
}
```

### 차트

```csharp
using Spectre.Console;

class ChartExamples
{
    static void Main()
    {
        BarChart();
        BreakdownChart();
    }

    static void BarChart()
    {
        AnsiConsole.Write(new BarChart()
            .Width(60)
            .Label("[green bold underline]월별 매출[/]")
            .CenterLabel()
            .AddItem("1월", 120, Color.Red)
            .AddItem("2월", 150, Color.Orange1)
            .AddItem("3월", 180, Color.Yellow)
            .AddItem("4월", 160, Color.Green)
            .AddItem("5월", 200, Color.Blue)
            .AddItem("6월", 210, Color.Purple));
    }

    static void BreakdownChart()
    {
        AnsiConsole.Write(new BreakdownChart()
            .Width(60)
            .AddItem("백엔드", 45, Color.Blue)
            .AddItem("프론트엔드", 30, Color.Green)
            .AddItem("DevOps", 15, Color.Orange1)
            .AddItem("기타", 10, Color.Grey));

        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            new Rule("[yellow]팀별 업무 비중[/]")
                .RuleStyle(Style.Parse("grey"))
                .LeftJustified()
        );
    }
}
```

## 5.3 프로그레스 바와 스피너

### 프로그레스 바

```csharp
using Spectre.Console;
using System.Threading;

class ProgressExamples
{
    static void Main()
    {
        SimpleProgress();
        MultipleProgress();
        CustomProgress();
    }

    static void SimpleProgress()
    {
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask("[green]파일 다운로드[/]");

                while (!ctx.IsFinished)
                {
                    task.Increment(1.5);
                    Thread.Sleep(50);
                }
            });
    }

    static void MultipleProgress()
    {
        AnsiConsole.Progress()
            .AutoRefresh(true)
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn(),
            })
            .Start(ctx =>
            {
                var download = ctx.AddTask("[yellow]다운로드[/]", maxValue: 100);
                var extract = ctx.AddTask("[blue]압축 해제[/]", maxValue: 100);
                var install = ctx.AddTask("[green]설치[/]", maxValue: 100);

                // 다운로드
                while (!download.IsFinished)
                {
                    download.Increment(2);
                    Thread.Sleep(30);
                }

                // 압축 해제
                while (!extract.IsFinished)
                {
                    extract.Increment(3);
                    Thread.Sleep(20);
                }

                // 설치
                while (!install.IsFinished)
                {
                    install.Increment(5);
                    Thread.Sleep(40);
                }
            });
    }

    static void CustomProgress()
    {
        AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn()
                {
                    CompletedStyle = new Style(Color.Green),
                    RemainingStyle = new Style(Color.Grey),
                    IndeterminateStyle = new Style(Color.Yellow),
                },
                new PercentageColumn(),
                new DownloadedColumn(),
                new TransferSpeedColumn(),
            })
            .Start(ctx =>
            {
                var task = ctx.AddTask("[cyan]대용량 파일 전송[/]", maxValue: 1000);
                task.MaxValue = 1000;

                while (!ctx.IsFinished)
                {
                    task.Increment(10);
                    Thread.Sleep(100);
                }
            });
    }
}

// 커스텀 컬럼
public class DownloadedColumn : ProgressColumn
{
    protected override bool NoWrap => true;

    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        var downloaded = task.Value;
        var total = task.MaxValue;
        return new Text($"{downloaded:F0}/{total:F0} MB");
    }
}

public class TransferSpeedColumn : ProgressColumn
{
    private double lastValue;
    private DateTime lastTime = DateTime.Now;

    protected override bool NoWrap => true;

    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        var now = DateTime.Now;
        var timeDiff = (now - lastTime).TotalSeconds;

        if (timeDiff > 0)
        {
            var valueDiff = task.Value - lastValue;
            var speed = valueDiff / timeDiff;

            lastValue = task.Value;
            lastTime = now;

            return new Text($"{speed:F1} MB/s", new Style(Color.Grey));
        }

        return new Text("-- MB/s", new Style(Color.Grey));
    }
}
```

### 스피너와 스테이터스

```csharp
using Spectre.Console;
using System.Threading;

class SpinnerExamples
{
    static void Main()
    {
        SimpleSpinner();
        StatusSpinner();
        LiveDisplay();
    }

    static void SimpleSpinner()
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .Start("[yellow]작업 수행 중...[/]", ctx =>
            {
                Thread.Sleep(2000);
                ctx.Status("[yellow]데이터 로딩...[/]");
                Thread.Sleep(2000);
                ctx.Status("[yellow]처리 중...[/]");
                Thread.Sleep(2000);
            });
    }

    static void StatusSpinner()
    {
        AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star)
            .Start("[bold blue]초기화 중...[/]", ctx =>
            {
                AnsiConsole.MarkupLine("[grey]데이터베이스 연결 중...[/]");
                Thread.Sleep(1000);

                ctx.Status("[bold blue]설정 로드 중...[/]");
                AnsiConsole.MarkupLine("[grey]환경 변수 읽기...[/]");
                Thread.Sleep(1000);

                ctx.Status("[bold blue]서비스 시작 중...[/]");
                AnsiConsole.MarkupLine("[grey]HTTP 서버 시작...[/]");
                Thread.Sleep(1000);

                ctx.Status("[bold green]완료![/]");
                AnsiConsole.MarkupLine("[green]✓ 모든 서비스가 시작되었습니다.[/]");
            });
    }

    static void LiveDisplay()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("작업")
            .AddColumn("상태")
            .AddColumn("진행률");

        AnsiConsole.Live(table)
            .AutoClear(false)
            .Start(ctx =>
            {
                var tasks = new[] { "다운로드", "검증", "설치" };

                foreach (var taskName in tasks)
                {
                    table.AddRow(taskName, "[yellow]진행중[/]", "[dim]0%[/]");
                    ctx.Refresh();

                    for (int i = 0; i <= 100; i += 10)
                    {
                        table.UpdateCell(table.Rows.Count - 1, 2, $"[green]{i}%[/]");
                        ctx.Refresh();
                        Thread.Sleep(100);
                    }

                    table.UpdateCell(table.Rows.Count - 1, 1, "[green]완료[/]");
                    ctx.Refresh();
                }
            });
    }
}
```

## 5.4 프롬프트와 선택 UI

### 텍스트 입력

```csharp
using Spectre.Console;

class PromptExamples
{
    static void Main()
    {
        TextPrompts();
        ValidationPrompts();
        SecretPrompts();
    }

    static void TextPrompts()
    {
        // 기본 텍스트 입력
        var name = AnsiConsole.Ask<string>("당신의 [green]이름[/]은?");

        // 기본값 제공
        var age = AnsiConsole.Prompt(
            new TextPrompt<int>("당신의 [green]나이[/]는?")
                .DefaultValue(25)
                .ValidationErrorMessage("[red]올바른 숫자를 입력하세요[/]")
        );

        // 선택 사항
        var email = AnsiConsole.Prompt(
            new TextPrompt<string>("[grey][[선택]][/] [green]이메일[/]:")
                .AllowEmpty()
        );

        AnsiConsole.MarkupLine($"\n[yellow]입력 정보:[/]");
        AnsiConsole.MarkupLine($"이름: [blue]{name}[/]");
        AnsiConsole.MarkupLine($"나이: [blue]{age}[/]");
        AnsiConsole.MarkupLine($"이메일: [blue]{email ?? "(없음)"}[/]");
    }

    static void ValidationPrompts()
    {
        // 커스텀 검증
        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("사용자 이름:")
                .Validate(username =>
                {
                    if (username.Length < 3)
                        return ValidationResult.Error("[red]3자 이상 입력하세요[/]");

                    if (!username.All(char.IsLetterOrDigit))
                        return ValidationResult.Error("[red]영숫자만 사용 가능합니다[/]");

                    return ValidationResult.Success();
                })
        );

        // 범위 검증
        var port = AnsiConsole.Prompt(
            new TextPrompt<int>("포트 번호:")
                .Validate(port =>
                {
                    return port switch
                    {
                        < 1024 => ValidationResult.Error("[red]1024 이상의 포트를 사용하세요[/]"),
                        > 65535 => ValidationResult.Error("[red]65535 이하의 포트를 사용하세요[/]"),
                        _ => ValidationResult.Success()
                    };
                })
        );

        AnsiConsole.MarkupLine($"\n사용자: [green]{username}[/]");
        AnsiConsole.MarkupLine($"포트: [green]{port}[/]");
    }

    static void SecretPrompts()
    {
        // 비밀번호 입력
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("비밀번호:")
                .Secret()
        );

        var confirm = AnsiConsole.Prompt(
            new TextPrompt<string>("비밀번호 확인:")
                .Secret()
                .Validate(confirm =>
                {
                    if (confirm != password)
                        return ValidationResult.Error("[red]비밀번호가 일치하지 않습니다[/]");
                    return ValidationResult.Success();
                })
        );

        AnsiConsole.MarkupLine("[green]✓ 비밀번호가 설정되었습니다[/]");
    }
}
```

### 선택 메뉴

```csharp
using Spectre.Console;

class SelectionExamples
{
    static void Main()
    {
        SingleSelection();
        MultiSelection();
        NestedSelection();
    }

    static void SingleSelection()
    {
        var fruit = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("좋아하는 [green]과일[/]을 선택하세요:")
                .PageSize(10)
                .MoreChoicesText("[grey](더 보려면 위아래 화살표를 사용하세요)[/]")
                .AddChoices(new[]
                {
                    "사과", "바나나", "체리", "포도", "레몬",
                    "오렌지", "딸기", "수박", "키위", "망고"
                })
        );

        AnsiConsole.MarkupLine($"선택한 과일: [yellow]{fruit}[/]");
    }

    static void MultiSelection()
    {
        var languages = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("사용 가능한 [green]프로그래밍 언어[/]를 선택하세요:")
                .NotRequired()
                .PageSize(10)
                .InstructionsText(
                    "[grey](스페이스바로 선택/해제, " +
                    "엔터로 확인)[/]")
                .AddChoiceGroup("백엔드", new[]
                {
                    "C#", "Java", "Python", "Go", "Rust"
                })
                .AddChoiceGroup("프론트엔드", new[]
                {
                    "JavaScript", "TypeScript", "Dart"
                })
        );

        AnsiConsole.MarkupLine("\n선택한 언어:");
        foreach (var lang in languages)
        {
            AnsiConsole.MarkupLine($"  • [yellow]{lang}[/]");
        }
    }

    static void NestedSelection()
    {
        var category = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("카테고리를 선택하세요:")
                .AddChoices("전자제품", "의류", "식품")
        );

        var items = category switch
        {
            "전자제품" => new[] { "노트북", "스마트폰", "태블릿" },
            "의류" => new[] { "티셔츠", "청바지", "재킷" },
            "식품" => new[] { "과일", "채소", "음료" },
            _ => Array.Empty<string>()
        };

        var item = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow]{category}[/] 항목을 선택하세요:")
                .AddChoices(items)
        );

        AnsiConsole.MarkupLine($"\n선택: [green]{category}[/] > [yellow]{item}[/]");
    }
}
```

### 확인 프롬프트

```csharp
using Spectre.Console;

class ConfirmExamples
{
    static void Main()
    {
        // 예/아니오 확인
        if (AnsiConsole.Confirm("계속 진행하시겠습니까?"))
        {
            AnsiConsole.MarkupLine("[green]진행합니다...[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]취소되었습니다.[/]");
            return;
        }

        // 기본값 지정
        var deleteAll = AnsiConsole.Confirm(
            "[red]모든 파일을 삭제하시겠습니까?[/]",
            defaultValue: false
        );

        if (deleteAll)
        {
            // 추가 확인
            var reallyDelete = AnsiConsole.Prompt(
                new TextPrompt<string>("정말로 삭제하려면 '[red]DELETE[/]'를 입력하세요:")
                    .Validate(input =>
                    {
                        if (input == "DELETE")
                            return ValidationResult.Success();
                        return ValidationResult.Error("[red]정확히 입력해주세요[/]");
                    })
            );

            AnsiConsole.MarkupLine("[red]✓ 모든 파일이 삭제되었습니다[/]");
        }
    }
}
```

## 5.5 마크업과 스타일링

### 마크업 문법

```csharp
using Spectre.Console;

class MarkupExamples
{
    static void Main()
    {
        BasicMarkup();
        ColorMarkup();
        CombinedMarkup();
        EscapeMarkup();
    }

    static void BasicMarkup()
    {
        AnsiConsole.MarkupLine("[bold]굵게[/]");
        AnsiConsole.MarkupLine("[italic]기울임[/]");
        AnsiConsole.MarkupLine("[underline]밑줄[/]");
        AnsiConsole.MarkupLine("[strikethrough]취소선[/]");
        AnsiConsole.MarkupLine("[dim]흐리게[/]");
        AnsiConsole.MarkupLine("[invert]반전[/]");
        AnsiConsole.MarkupLine("[conceal]숨김[/]");
        AnsiConsole.MarkupLine("[blink]깜빡임[/]");
    }

    static void ColorMarkup()
    {
        // 전경색
        AnsiConsole.MarkupLine("[red]빨강[/]");
        AnsiConsole.MarkupLine("[green]초록[/]");
        AnsiConsole.MarkupLine("[blue]파랑[/]");
        AnsiConsole.MarkupLine("[yellow]노랑[/]");

        // 배경색
        AnsiConsole.MarkupLine("[white on blue]파란 배경[/]");
        AnsiConsole.MarkupLine("[black on yellow]노란 배경[/]");

        // RGB 색상
        AnsiConsole.MarkupLine("[rgb(255,0,255)]마젠타[/]");
        AnsiConsole.MarkupLine("[#FF6347]토마토색[/]");

        // 조합
        AnsiConsole.MarkupLine("[bold red on white]강조된 텍스트[/]");
    }

    static void CombinedMarkup()
    {
        AnsiConsole.MarkupLine(
            "이것은 [bold yellow]중요한[/] " +
            "[underline green]성공[/] 메시지이며, " +
            "[italic grey]참고사항[/]이 있습니다."
        );

        AnsiConsole.MarkupLine(
            "[bold red on white] ERROR [/] " +
            "[red]파일을 찾을 수 없습니다: [/]" +
            "[yellow]config.json[/]"
        );

        // 링크 (일부 터미널에서 지원)
        AnsiConsole.MarkupLine("[link=https://github.com]GitHub[/]");
    }

    static void EscapeMarkup()
    {
        // 대괄호 이스케이프
        AnsiConsole.MarkupLine("이중 대괄호 [[로 이스케이프]] 합니다");

        // Markup.Escape 사용
        var userInput = "[bold]사용자 입력[/]";
        AnsiConsole.MarkupLine($"입력: {Markup.Escape(userInput)}");

        // 안전한 출력
        AnsiConsole.WriteLine("마크업 없이 출력: [bold]그대로[/]");
    }
}
```

### 스타일 객체

```csharp
using Spectre.Console;

class StyleExamples
{
    static void Main()
    {
        // Style 객체 생성
        var errorStyle = new Style(
            foreground: Color.Red,
            background: Color.Black,
            decoration: Decoration.Bold
        );

        var successStyle = new Style(Color.Green);
        var warningStyle = Style.Parse("bold yellow");

        // 사용
        AnsiConsole.Write("에러: ", errorStyle);
        AnsiConsole.WriteLine("파일을 찾을 수 없습니다");

        AnsiConsole.Write("성공: ", successStyle);
        AnsiConsole.WriteLine("저장되었습니다");

        // Panel에 스타일 적용
        var panel = new Panel("중요한 메시지")
            .Border(BoxBorder.Double)
            .BorderColor(Color.Red)
            .Header("[bold red]경고[/]")
            .HeaderAlignment(Justify.Center)
            .Padding(2, 1);

        AnsiConsole.Write(panel);
    }
}
```

### 종합 예제: CLI 도구

```csharp
using Spectre.Console;
using System;
using System.Threading;

class ComprehensiveExample
{
    static void Main()
    {
        ShowBanner();

        var config = GatherConfiguration();

        ProcessData(config);

        ShowSummary(config);
    }

    static void ShowBanner()
    {
        var rule = new Rule("[bold blue]데이터 처리 도구 v1.0[/]")
            .RuleStyle(Style.Parse("blue"))
            .LeftJustified();

        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }

    static Configuration GatherConfiguration()
    {
        var config = new Configuration();

        config.InputFile = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]입력 파일:[/]")
                .Validate(file =>
                {
                    if (string.IsNullOrWhiteSpace(file))
                        return ValidationResult.Error("[red]파일명을 입력하세요[/]");
                    return ValidationResult.Success();
                })
        );

        config.Format = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]출력 형식:[/]")
                .AddChoices("JSON", "XML", "CSV", "YAML")
        );

        config.Verbose = AnsiConsole.Confirm(
            "상세 로그를 출력하시겠습니까?",
            defaultValue: false
        );

        return config;
    }

    static void ProcessData(Configuration config)
    {
        AnsiConsole.WriteLine();

        AnsiConsole.Progress()
            .AutoRefresh(true)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(),
            })
            .Start(ctx =>
            {
                var read = ctx.AddTask("[yellow]파일 읽기[/]");
                var parse = ctx.AddTask("[blue]데이터 파싱[/]");
                var transform = ctx.AddTask("[cyan]변환[/]");
                var write = ctx.AddTask("[green]결과 저장[/]");

                // 시뮬레이션
                SimulateTask(read, config.Verbose);
                SimulateTask(parse, config.Verbose);
                SimulateTask(transform, config.Verbose);
                SimulateTask(write, config.Verbose);
            });
    }

    static void SimulateTask(ProgressTask task, bool verbose)
    {
        for (int i = 0; i <= 100; i += 5)
        {
            task.Value = i;
            Thread.Sleep(50);

            if (verbose && i % 20 == 0)
            {
                AnsiConsole.MarkupLine($"[grey]  {task.Description}: {i}%[/]");
            }
        }
    }

    static void ShowSummary(Configuration config)
    {
        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]항목[/]").Centered())
            .AddColumn(new TableColumn("[bold]값[/]"));

        table.AddRow("입력 파일", $"[yellow]{config.InputFile}[/]");
        table.AddRow("출력 형식", $"[cyan]{config.Format}[/]");
        table.AddRow("상세 로그", config.Verbose ? "[green]예[/]" : "[grey]아니오[/]");
        table.AddRow("상태", "[green]완료[/]");

        AnsiConsole.Write(table);

        var panel = new Panel("[green]✓[/] 모든 작업이 성공적으로 완료되었습니다!")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Green)
            .Padding(1, 0);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
    }
}

class Configuration
{
    public string InputFile { get; set; } = "";
    public string Format { get; set; } = "";
    public bool Verbose { get; set; }
}
```

### 핵심 요약

1. **Spectre.Console**: 현대적인 .NET 터미널 UI 라이브러리
2. **테이블/트리/차트**: 복잡한 데이터를 시각적으로 표현
3. **프로그레스**: 진행률 표시와 실시간 업데이트
4. **프롬프트**: 대화형 입력과 선택 UI
5. **마크업**: 풍부한 텍스트 스타일링

다음 챕터에서는 ConsoleAppFramework를 다루겠습니다.
