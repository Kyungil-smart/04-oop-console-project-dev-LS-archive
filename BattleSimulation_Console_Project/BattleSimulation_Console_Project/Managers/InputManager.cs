using System;

public static class InputManager
{
    private static ConsoleKey _current;
    private static DateTime _lastInputTime; // 입력 딜레이 관리 변수
    private const double InputDelay = 100; // 0.1초 (100ms) 딜레이
    public static bool HasInput => Console.KeyAvailable;

    private static readonly ConsoleKey[] _keys =
    {
        ConsoleKey.UpArrow, 
        ConsoleKey.DownArrow, 
        ConsoleKey.LeftArrow, 
        ConsoleKey.RightArrow,
        ConsoleKey.Enter,
        ConsoleKey.I,
        ConsoleKey.L,
        ConsoleKey.T
    };

    public static bool GetKey(ConsoleKey input)
    {
        return _current == input;
    }

    // GameManager에서만 호출
    public static void GetUserInput()
    {
        // 키 입력이 없으면 패스 (게임 멈춤 방지)
        if (!Console.KeyAvailable) 
        {
            ResetKey(); // 키를 안 누르고 있으면 입력 상태 초기화
            return;
        }
        
        // 마지막 입력 후 InputDelay가 안 지났으면 입력 무시
        if ((DateTime.Now - _lastInputTime).TotalMilliseconds < InputDelay)
        {
            // 입력 버퍼 비우기 (중복 입력 방지)
            while (Console.KeyAvailable) Console.ReadKey(true); 
            return;
        }
        
        ConsoleKey input = Console.ReadKey(true).Key;
        _lastInputTime = DateTime.Now; // 시간 갱신
        
        _current = ConsoleKey.Clear;

        foreach (ConsoleKey key in _keys)
        {
            if (key == input)
            {
                _current = input;
                break;
            }
        }
        // 중복 입력 원천 차단. 입력 한번에 쓰인 값을 제외하고 뒤에 쌓인 키들 버리기.
        while (Console.KeyAvailable)
        {
            Console.ReadKey(true);
        }
    }

    public static void ResetKey()
    {
        _current = ConsoleKey.Clear;
    }
}