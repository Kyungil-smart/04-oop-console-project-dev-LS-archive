using System;

public class TitleScene : Scene
{
    private Ractangle _titleOutline;
    private Ractangle _logoOutline;
    private Ractangle _creditsOutline;
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
        _titleMenu.Add("게임 종료", GameQuit);
        _titleMenu.Add("게임 종료", GameQuit);
        _titleMenu.Add("게임 종료", GameQuit);
        _titleMenu.Add("게임 종료", GameQuit);
    }

    public override void Enter()
    {
        base.Enter();
        NeedsRedraw = true;
        _titleMenu.Reset();
        Debug.Log("타이틀 씬 진입");
    }

    public override void Update()
    {
        if (InputManager.GetKey(ConsoleKey.UpArrow))
        {
            _titleMenu.SelectUp();
            NeedsRedraw = true;
        } 
        
        if (InputManager.GetKey(ConsoleKey.DownArrow))
        {
            _titleMenu.SelectDown();
            NeedsRedraw = true;
        }

        if (InputManager.GetKey(ConsoleKey.Enter))
        {
            _titleMenu.Select();
            NeedsRedraw = true;
        }
    }
    
    public override void Render()
    {
        // 그려야 할 상황이 아니면 바로 리턴
        if (!NeedsRedraw) return;
        
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
        int menuX = _logoOutline.X + (_logoOutline.Width / 2) - (_titleMenu._outline.Width / 2);

        //로고 아래 타이틀 메뉴 출력
        _titleMenu.Render(menuX, _logoOutline.Y + _logoOutline.Height + 1);

        NeedsRedraw = false;
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
        // 팝업 창 크기 및 위치 설정 (화면 중앙)
        int width = 55;
        int height = 10;
        int x = (Console.WindowWidth - width) / 2;
        int y = (Console.WindowHeight - height) / 2;

        // 팝업 아웃라인
        _creditsOutline.Draw(x, y, width, height);

        // 내용 출력
        Console.SetCursorPosition(x + 2, y + 2);
        "CREDITS".Print(ConsoleColor.Cyan);

        Console.SetCursorPosition(x + 2, y + 4);
        "- 개발: 이성규".Print(ConsoleColor.White);
        Console.SetCursorPosition(x + 2, y + 5);
        "- 아스키 리스스 생성: ASCII Art Archive".Print(ConsoleColor.White);
        Console.SetCursorPosition(x + 2, y + 6);
        "- 기본 프레임 워크 제공: 프로그래밍 강사 김재성님".Print(ConsoleColor.White);

        Console.SetCursorPosition(x + 2, y + 8);
        "[Press Any Key to Close]".Print(ConsoleColor.DarkGray);

        // 사용자가 키를 누를 때까지 여기서 대기
        // 메인 루프에 들어가 Console.Clear()가 실행되는 것을 방지.
        Console.ReadKey(true);
    }
}