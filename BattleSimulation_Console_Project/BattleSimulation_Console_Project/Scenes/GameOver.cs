using System;

public class GameOver : Scene
{
    private Ractangle _overOutline;

    public GameOver()
    {
        Init();
    }

    public void Init()
    {
        _overOutline.Width = AsciiArtData.GameOver.Width + 4;  // 좌우 여백 2칸씩
        _overOutline.Height = AsciiArtData.GameOver.Height + 2; // 상하 여백 1칸씩
        _overOutline.X = 2;
        _overOutline.Y = 3;
    }
    public override void Enter()
    {
        base.Enter();
        NeedsRedraw = true;
    }

    public override void Render()
    {
        if (!NeedsRedraw) return;
        
        // 테두리 그리기
        Console.SetCursorPosition(_overOutline.X, _overOutline.Y);
        _overOutline.Draw();

        // 아스키 아트 그리기 (테두리 안쪽에 위치하도록 보정)
        // Outline의 시작점보다 X는 2칸, Y는 1칸 안쪽으로 밀어 넣음
        AsciiArtData.GameOver.DrawAscii(_overOutline.X + 2, _overOutline.Y + 1, ConsoleColor.DarkRed);
        
        // 안내 문구 추가
        string msg = "Press [Enter] to Title";
        Console.SetCursorPosition(
            _overOutline.X + (_overOutline.Width - msg.Length) / 2, 
            _overOutline.Y + _overOutline.Height + 2
        );
        msg.Print(ConsoleColor.Gray);

        NeedsRedraw = false;
    }

    public override void Update()
    {
        // 엔터 키를 누르면 타이틀로 복귀
        if (InputManager.GetKey(ConsoleKey.Enter))
        {
            SceneManager.Change(new TitleScene()); // TitleScene으로 전환
        }
    }

    public override void Exit()
    {
        Console.ResetColor();
        Console.Clear();
    }
}