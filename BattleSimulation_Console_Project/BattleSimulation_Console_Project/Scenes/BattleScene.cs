// Scenes/BattleScene.cs
public class BattleScene : Scene
{
    private Enemy _currentEnemy;
    private PlayerCharacter _player;
    
    private Ractangle _enemyNameOutline;
    private Ractangle _enemyOutline;

    // 배틀 시작 시 적 정보를 받아옴
    public void Init(PlayerCharacter player, Enemy enemy)
    {
        _player = player;
        _currentEnemy = enemy;
        
        _enemyOutline.Width = AsciiArtData.Enemy.Width + 4;  // 좌우 여백 2칸씩
        _enemyOutline.Height = AsciiArtData.Enemy.Height + 2; // 상하 여백 1칸씩
        _enemyOutline.X = 2;
        _enemyOutline.Y = 5;
        
        // 적 이름 외곽선 (상단 배치)
        _enemyNameOutline.Width = _currentEnemy.Name.GetTextWidth() + 4; // 이름 길이 + 여백
        _enemyNameOutline.Height = 3;
        // 아스키 아트 상단 중앙에 배치
        _enemyNameOutline.X = (AsciiArtData.Enemy.Width / 2) - (_enemyNameOutline.Width / 2) + 2; 
        _enemyNameOutline.Y = 0;
    }

    public BattleScene(PlayerCharacter player, Enemy enemy)
    {
        Init(player, enemy);
    }

    public override void Enter()
    {
        base.Enter();
        NeedsRedraw = true;
    }

    public override void Update()
    {
        // 1. 화면 그리기 (UI 박스, 적 정보, 내 정보)
        // 2. InputManager로 메뉴 선택 (공격, 도망)
        // 3. 결과 처리 (승리 시 TownScene으로 복귀)
        
        // 임시 구현 타이틀 복귀
        // 엔터 키를 누르면 타이틀로 복귀
        if (InputManager.GetKey(ConsoleKey.Enter))
        {
            SceneManager.ChangePrevScene();
        }
    }

    public override void Render()
    {
        if (!NeedsRedraw) return;
        
        _enemyNameOutline.Draw();
        // 적 이름 출력
        Console.SetCursorPosition(_enemyNameOutline.X + 2, 1);
        _currentEnemy.Name.Print(_currentEnemy.Color, ConsoleColor.DarkGray);
        
        // 테두리 그리기
        Console.SetCursorPosition(_enemyOutline.X, _enemyOutline.Y);
        _enemyOutline.Draw();

        // 아스키 아트 그리기 (테두리 안쪽에 위치하도록 보정)
        // Outline의 시작점보다 X는 2칸, Y는 1칸 안쪽으로 밀어 넣음
        AsciiArtData.Enemy.DrawAscii(_enemyOutline.X + 2, _enemyOutline.Y + 1, _currentEnemy.Color);
        
        // 안내 문구 추가
        string msg = "Press [Enter] to PrevScene";
        Console.SetCursorPosition(
            _enemyOutline.X + (_enemyOutline.Width - msg.Length) / 2, 
            _enemyOutline.Y + _enemyOutline.Height + 2
        );
        msg.Print(ConsoleColor.Gray);

        NeedsRedraw = false;
    }

    public override void Exit()
    {
        Console.ResetColor();
        Console.Clear();
    }
}