// 파이프라인 필터 도구
using System;

if (args.Length == 0)
{
    Console.Error.WriteLine("사용법: FilterTool <pattern>");
    Console.Error.WriteLine("  stdin에서 라인을 읽어 패턴과 매칭되는 라인만 출력합니다.");
    return 1;
}

var pattern = args[0];
var caseSensitive = args.Length > 1 && args[1] == "--case-sensitive";
var comparison = caseSensitive
    ? StringComparison.Ordinal
    : StringComparison.OrdinalIgnoreCase;

string? line;
int matchCount = 0;
int totalLines = 0;

while ((line = Console.ReadLine()) != null)
{
    totalLines++;

    if (line.Contains(pattern, comparison))
    {
        Console.WriteLine(line);
        matchCount++;
    }
}

Console.Error.WriteLine($"[필터] 총 {totalLines}개 라인 중 {matchCount}개 매칭");
return 0;
