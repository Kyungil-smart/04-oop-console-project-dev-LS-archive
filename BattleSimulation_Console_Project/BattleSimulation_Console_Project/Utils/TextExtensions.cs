using System;

public static class TextExtensions
{
    // 문자열(String) 출력
    public static void Print(this string text, ConsoleColor? color = null, ConsoleColor? backColor = null)
    {
        // 색상 변경 전, 기존 색상 저장
        ConsoleColor originalColor = Console.ForegroundColor;
        ConsoleColor originalBack = Console.BackgroundColor;

        // 색상이 지정되었다면(null이 아니라면) 변경
        if (color.HasValue) Console.ForegroundColor = color.Value;
        if (backColor.HasValue) Console.BackgroundColor = backColor.Value;

        // 텍스트 출력
        Console.Write(text);

        // 색상 복구 (ResetColor 대신 메서드 호출시점의 색으로 복구)
        if (color.HasValue) Console.ForegroundColor = originalColor;
        if (backColor.HasValue) Console.BackgroundColor = originalBack;
    }

    // 문자(Char) 출력
    public static void Print(this char text, ConsoleColor? color = null, ConsoleColor? backColor = null)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        ConsoleColor originalBack = Console.BackgroundColor;

        if (color.HasValue) Console.ForegroundColor = color.Value;
        if (backColor.HasValue) Console.BackgroundColor = backColor.Value;

        Console.Write(text);

        if (color.HasValue) Console.ForegroundColor = originalColor;
        if (backColor.HasValue) Console.BackgroundColor = originalBack;
    }

    // 줄바꿈 (WriteLine)
    public static void PrintLine(this string text, ConsoleColor? color = null, ConsoleColor? backColor = null)
    {
        text.Print(color, backColor);
        Console.WriteLine();
    }
    
    public static int GetTextWidth(this string text)
    {
        int width = 0;
        foreach (char c in text)
        {
            width += c.GetCharacterWidth();
        }
        return width;
    }

    public static int GetCharacterWidth(this char character)
    {
        // 한글 음절(가-힣), CJK 호환문자, 전각 기호/문자 범위는 2칸으로 처리
        if ((character >= '\uAC00' && character <= '\uD7A3') || // 한글 완성형
            (character >= '\u1100' && character <= '\u11FF') || // 한글 자모
            (character >= '\u3130' && character <= '\u318F') || // 한글 호환 자모
            (character >= '\uFF01' && character <= '\uFF60') || // 전각 기호/영숫자
            (character >= '\uFFE0' && character <= '\uFFE6'))   // 전각 특수기호
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
    
    public static void DrawAscii(this AsciiArt art, int x, int y, ConsoleColor color)
    {
        for (int i = 0; i < art.Height; i++)
        {
            // 얼마가 들어오든 최소한 0을 보장해 에러방지
            Console.SetCursorPosition(Math.Max(0, x), Math.Max(0, y + i));
            art.Lines[i].Print(color);
        }
    }
}