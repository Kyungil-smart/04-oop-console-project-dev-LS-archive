using System;

public class TitleScene : Scene
{
    private Ractangle _titleOutline;
    private Ractangle _logoOutline;
    private Ractangle _creditsOutline;
    private MenuList _titleMenu;
    
    private bool _isShowCredits = false;

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
        
        // 게임 이름 외곽선 (상단 배치)
        _titleOutline.Width = GameManager.GameName.Length + 4;
        _titleOutline.Height = 3;
        // 로고 중앙에 배치
        _titleOutline.X = (AsciiArtData.Title.Width / 2) - (_titleOutline.Width / 2) + 2; 
        _titleOutline.Y = 0;
        
        // 메인 로고 외곽선
        _logoOutline.X = 2;
        _logoOutline.Y = 3;
        _logoOutline.Height = AsciiArtData.Title.Height + 3;
        _logoOutline.Width = AsciiArtData.Title.Width + 2;
        
        
        // 크레딧 창 정보 미리 설정
        int crW = 55;
        int crH = 10;
        _creditsOutline.Width = crW;
        _creditsOutline.Height = crH;
        _creditsOutline.X = (Console.WindowWidth - crW) / 2;
        _creditsOutline.Y = (Console.WindowHeight - crH) / 2;
    }

    public override void Enter()
    {
        base.Enter();
        NeedsRedraw = true;
        _isShowCredits = false;
        _titleMenu.Reset();
        Debug.Log("타이틀 씬 진입");
    }

    public override void Update()
    {
        // 크레딧 모드일 때의 입력 처리 (창 닫기)
        if (_isShowCredits)
        {
            // 크레딧 닫기
            if (InputManager.GetKey(ConsoleKey.Enter))
            {
                CloseCredits();
            }
            return; // 크레딧 떠 있으면 아래 메뉴 조작 코드는 실행 안 함
        }
        
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
        
        // 상태에 따라 그리는 메서드 분리
        if (_isShowCredits)
        {
            RenderCredits();
        }
        else
        {
            RenderTitle();
        }

        NeedsRedraw = false;
    }

    // --- [화면 그리기 분리] ---
    private void RenderTitle()
    {
        _titleOutline.Draw();

        //게임 이름 출력 (외곽선 안쪽)
        Console.SetCursorPosition(_titleOutline.X + 2, 1);
        GameManager.GameName.Print(ConsoleColor.Yellow, ConsoleColor.DarkRed);
        
        _logoOutline.Draw();
        
        // 아스키 아트 로고 출력 (외곽선 안쪽 1칸 여백)
        AsciiArtData.Title.DrawAscii(_logoOutline.X + 1, _logoOutline.Y + 1, ConsoleColor.Yellow);

        // 메뉴 출력 (로고 아래쪽 중앙 정렬)
        int menuX = _logoOutline.X + (_logoOutline.Width / 2) - (_titleMenu._outline.Width / 2);

        //로고 아래 타이틀 메뉴 출력
        _titleMenu.Render(menuX, _logoOutline.Y + _logoOutline.Height + 1);
    }
    
    private void RenderCredits()
    {
        // 팝업 아웃라인
        _creditsOutline.Draw();
        
        int x = _creditsOutline.X;
        int y = _creditsOutline.Y;
        
        // 내용 출력
        Console.SetCursorPosition(x + 2, y + 2);
        "CREDITS".Print(ConsoleColor.Cyan);

        Console.SetCursorPosition(x + 2, y + 4);
        "- 개발: 이성규".Print(ConsoleColor.White);
        Console.SetCursorPosition(x + 2, y + 5);
        "- 아스키 리소스: ASCII Art Archive".Print(ConsoleColor.White);
        Console.SetCursorPosition(x + 2, y + 6);
        "- 프레임워크: 김재성 강사님".Print(ConsoleColor.White);

        Console.SetCursorPosition(x + 2, y + 8);
        "[Press Enter to Close]".Print(ConsoleColor.DarkGray);
    }
    
    public void ViewCredits()
    {
        // 기존 방식에서 렌더 분리. 상태 체크용 변수만 변경
        _isShowCredits = true;
        NeedsRedraw = true; // 화면 갱신 요청
    }
    
    public void CloseCredits()
    {
        // 크레딧 창이 있던 자리를 공백으로 덮어쓰기
        EraseCreditsArea();
        
        _isShowCredits = false; // 상태 복구
        NeedsRedraw = true;     // 메뉴 다시 그리기 요청
    }
    
    // 크레딧 창 크기만큼 공백 출력
    private void EraseCreditsArea()
    {
        // 해당 영역을 공백 문자열로 덮어쓰기
        string emptyLine = new string(' ', _creditsOutline.Width + 2); // 외곽선 포함 넉넉하게

        for (int i = 0; i <= _creditsOutline.Height; i++)
        {
            Console.SetCursorPosition(_creditsOutline.X, _creditsOutline.Y + i);
            Console.Write(emptyLine);
        }
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
        // SceneManager.Change("Town");
    
        // 새로운 게임 시작
        PlayerCharacter newPlayer = new PlayerCharacter();
        newPlayer.playerName = "콘솔 전사"; // 혹은 입력받은 이름
    
        // 생성자를 통해 맵과 아이템을 새로 배치(Init)
        SceneManager.Change(new TownScene(newPlayer));
    }
}