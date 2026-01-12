using System;

public class GameManager
{
    public static bool IsGameOver { get; set; }
    public const string GameName = "Battle Simulation";
    private PlayerCharacter _player;
    private int _lastRunTick, _currentTick;
    
    // 프레임 제한 (약 30 FPS)
    private const int WAIT_TICK = 1000 / 30; // 1초(1000ms) 나누기 30

    public void Run()
    {
        Console.CursorVisible = false;
        
        Init();
        
        // TickCount: 시스템 시작 이후 경과 시간(밀리초)을 가져온다.
        _lastRunTick = Environment.TickCount;
        
        while (!IsGameOver)
        {
            // FPS 제어 
            _currentTick = Environment.TickCount;
            if (_currentTick - _lastRunTick < WAIT_TICK)
            {
                Thread.Sleep(1);
                continue;
            }
            _lastRunTick = _currentTick;
            
            // 키입력 받고
            InputManager.GetUserInput();

            // 개발용 로그 기능_릴리스 단계에서는 주석 처리
            if (InputManager.GetKey(ConsoleKey.L))
            {
                SceneManager.Change("Log");
            }

            // 데이터 처리
            SceneManager.Update();
            
            // 렌더링
            // Console.Clear();
            SceneManager.Render();
        }
    }

    private void Init()
    {
        IsGameOver = false;
        SceneManager.OnChangeScene += InputManager.ResetKey;
        _player = new PlayerCharacter();
        
        SceneManager.AddScene("Title", new TitleScene());
        SceneManager.AddScene("Story", new StoryScene());
        //SceneManager.AddScene("Town", new TownScene(_player));
        SceneManager.AddScene("Log", new LogScene());
        SceneManager.AddScene("GameOver", new GameOver());
        
        SceneManager.Change("Title");
        
        Debug.Log("게임 데이터 초기화 완료");
    }
}