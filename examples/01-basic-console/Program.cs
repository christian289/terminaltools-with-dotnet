// 기본 터미널 애플리케이션 예제
using System;

namespace HelloTerminal
{
    class Program
    {
        static int Main(string[] args)
        {
            // 터미널 정보 출력
            Console.WriteLine("=== Hello Terminal! ===");
            Console.WriteLine();

            Console.WriteLine($"플랫폼: {Environment.OSVersion.Platform}");
            Console.WriteLine($"OS 버전: {Environment.OSVersion.VersionString}");
            Console.WriteLine($"프로세서 수: {Environment.ProcessorCount}");
            Console.WriteLine($"현재 디렉토리: {Environment.CurrentDirectory}");
            Console.WriteLine();

            // 리다이렉션 감지
            Console.WriteLine("입출력 상태:");
            Console.WriteLine($"  입력 리다이렉션: {Console.IsInputRedirected}");
            Console.WriteLine($"  출력 리다이렉션: {Console.IsOutputRedirected}");
            Console.WriteLine($"  에러 리다이렉션: {Console.IsErrorRedirected}");
            Console.WriteLine();

            // 인자 처리
            if (args.Length > 0)
            {
                Console.WriteLine($"인자 개수: {args.Length}");
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine($"  args[{i}] = \"{args[i]}\"");
                }
            }
            else
            {
                Console.WriteLine("인자가 없습니다.");
                Console.WriteLine();
                Console.WriteLine("사용법:");
                Console.WriteLine("  HelloTerminal [arg1] [arg2] ...");
            }

            Console.WriteLine();

            // 색상 지원 확인 및 출력
            if (!Console.IsOutputRedirected)
            {
                Console.WriteLine("색상 테스트:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ✓ 성공 (녹색)");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ⚠ 경고 (노랑)");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  ✗ 에러 (빨강)");
                Console.ResetColor();
            }

            return 0;
        }
    }
}
