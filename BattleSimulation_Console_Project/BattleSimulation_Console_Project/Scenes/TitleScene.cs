using System;

public class TitleScene : Scene
{
    private Ractangle _titleOutline;
    private Ractangle _logoOutline;
    private MenuList _titleMenu;

    public TitleScene()
    {
        Init();
    }

    public void Init()
    {
        _titleMenu = new MenuList();
        _titleMenu.Add("게임 시작", GameStart);
        _titleMenu.Add("크레딧", ViewCredits);
        _titleMenu.Add("게임 종료", GameQuit);
    }

    public override void Enter()
    {
        _titleMenu.Reset();
        Debug.Log("타이틀 씬 진입");
    }

    public override void Update()
    {
        if (InputManager.GetKey(ConsoleKey.UpArrow))
        {
            _titleMenu.SelectUp();
        } 
        
        if (InputManager.GetKey(ConsoleKey.DownArrow))
        {
            _titleMenu.SelectDown();
        }

        if (InputManager.GetKey(ConsoleKey.Enter))
        {
            _titleMenu.Select();
        }
    }
    
    public override void Render()
    {
        // 게임 이름 외곽선 (상단 배치)
        _titleOutline.Width = GameManager.GameName.Length + 4;
        _titleOutline.Height = 3;
        // 로고 중앙에 배치
        _titleOutline.X = (AsciiArtData.Title.Width / 2) - (_titleOutline.Width / 2) + 2; 
        _titleOutline.Y = 0; 
        _titleOutline.Draw();

        //게임 이름 출력 (외곽선 안쪽)
        Console.SetCursorPosition(_titleOutline.X + 2, 1);
        GameManager.GameName.Print(ConsoleColor.Yellow, ConsoleColor.DarkRed);
        
        // 메인 로고 외곽선
        _logoOutline.X = 2;
        _logoOutline.Y = 3;
        _logoOutline.Height = AsciiArtData.Title.Height + 3;
        _logoOutline.Width = AsciiArtData.Title.Width + 2;
        _logoOutline.Draw();
        
        // 아스키 아트 로고 출력 (외곽선 안쪽 1칸 여백)
        AsciiArtData.Title.DrawAscii(_logoOutline.X + 1, _logoOutline.Y + 1, ConsoleColor.Yellow);

        // 메뉴 출력 (로고 아래쪽 중앙 정렬)
        int menuX = _logoOutline.X + (_logoOutline.Width / 2) - (_titleOutline.Width / 2);

        //로고 아래 타이틀 메뉴 출력
        _titleMenu.Render(menuX, _logoOutline.Y + _logoOutline.Height + 1);
    }

    public override void Exit()
    {
    }

    public void GameQuit()
    {
        GameManager.IsGameOver = true;
    }

    public void GameStart()
    {
        SceneManager.Change("Town");
    }

    public void ViewCredits()
    {
    }
}