using System;

public class TownScene : Scene
{
    private Ractangle _titleOutline;
    // 맵 크기: 실제 플레이 영역(20x40) + 테두리(2) = 22x42
    private Tile[,] _field = new Tile[22, 42];
    private PlayerCharacter _player;
    
    // 맵 그릴 시작 위치 (화면 중앙이나 상단 여백을 위해)
    private int _startX = 5;
    private int _startY = 5;
    
    public TownScene(PlayerCharacter player) => Init(player);

    public void Init(PlayerCharacter player)
    {
        _player = player;
        int height = _field.GetLength(0);
        int width = _field.GetLength(1);
        
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Vector pos = new Vector(x, y);
                _field[y, x] = new Tile(pos);
                
                // 테두리에 벽(Wall) 오브젝트 배치
                if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                {
                    _field[y, x].OnTileObject = new Wall();
                }
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
        
        // 데이터 연결
        _player.Field = _field;
        _player.Position = new Vector(4, 2);
        _field[_player.Position.Y, _player.Position.X].OnTileObject = _player;

        // 아이템 배치
        _field[3, 5].OnTileObject = new Potion() {Name = "Potion1"};
        _field[2, 15].OnTileObject = new Potion() {Name = "Potion2"};
        _field[7, 3].OnTileObject = new Potion() {Name = "Potion3"};
        _field[9, 19].OnTileObject = new Potion() {Name = "Potion4"};
        
        NeedsRedraw = true;
        Debug.Log("타운 씬 진입");
    }

    public override void Update()
    {
        // 유효한 키가 눌렸을 때만 로직 수행 & 렌더링 요청
        if (InputManager.GetKey(ConsoleKey.UpArrow) || 
            InputManager.GetKey(ConsoleKey.DownArrow) ||
            InputManager.GetKey(ConsoleKey.LeftArrow) || 
            InputManager.GetKey(ConsoleKey.RightArrow) ||
            InputManager.GetKey(ConsoleKey.I) ||
            InputManager.GetKey(ConsoleKey.T) ||
            InputManager.GetKey(ConsoleKey.Enter))
        {
            _player.Update(); // 플레이어 이동 로직 실행
            NeedsRedraw = true;   // 화면 갱신 요청
        }
    }

    public override void Render()
    {
        if(!NeedsRedraw) return;
        
        // 게임 이름 외곽선 (상단 배치)
        _titleOutline.Width = GameManager.GameName.Length + 4;
        _titleOutline.Height = 3;
        // 로고 중앙에 배치
        _titleOutline.X = (_field.GetLength(1) / 2) - (_titleOutline.Width / 2) + _startX; 
        _titleOutline.Y = 0;
        
        _titleOutline.Draw();

        //게임 이름 출력 (외곽선 안쪽)
        Console.SetCursorPosition(_titleOutline.X + 2, 1);
        GameManager.GameName.Print(ConsoleColor.Yellow, ConsoleColor.DarkRed);
        
        // 플레이어 그리기 (맵 위에 덮어쓰기)
        // _player.Render() 내부에서도 SetCursorPosition을 써야 정확히 그려집니다.
        // 만약 _player.Render()가 단순히 Console.Write라면, 위치를 잡아줘야 합니다.
        // 예시:
        // Console.SetCursorPosition(_startX + _player.Position.X, _startY + _player.Position.Y);
        
        // ★ [수정 2] 맵 위쪽 여백(게이지가 그려지는 곳) 청소
        // 플레이어가 맨 윗줄에 있을 때 게이지가 맵 바깥( row 0, 1 )에 그려지는데,
        // PrintField는 맵 안쪽( row 2~ )만 덮어쓰므로 바깥쪽 잔상을 직접 지워줘야 함.
        ClearOutSideArea();
        
        PrintField();
        _player.Render(_startX, _startY);
        
        NeedsRedraw = false;
    }
    // 맵 위쪽 잔상 제거용 메서드
    private void ClearOutSideArea()
    {
        // 게이지가 그려질 수 있는 _startY 위쪽 2줄을 공백으로 덮어씀
        string cleanLine = new string(' ', _field.GetLength(1) + 4);
        for (int i = 1; i <= 2; i++)
        {
            int y = _startY - i;
            if (y >= 0)
            {
                Console.SetCursorPosition(0, y);
                Console.Write(cleanLine);
            }
        }
    }

    public override void Exit()
    {
        // 나갈 때 정리
        if (_player.Field != null && _player.Position.Y < _field.GetLength(0) && _player.Position.X < _field.GetLength(1))
        {
            _field[_player.Position.Y, _player.Position.X].OnTileObject = null;
        }
        _player.Field = null;
    }

    private void PrintField()
    {
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            // 왼쪽 여백 청소 (게이지가 왼쪽으로 빠져나간 경우 대비)
            Console.SetCursorPosition(0, _startY + y);
            Console.Write(new string(' ', _startX));
            
            // 절대 좌표로 커서 이동 후 한 글자 출력
            Console.SetCursorPosition(_startX, _startY + y);
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                _field[y, x].Print();
            }
            
            // 오른쪽 여백 청소 (게이지가 오른쪽으로 삐져나간 잔상 제거)
            Console.SetCursorPosition(_startX + _field.GetLength(1), _startY + y);
            Console.Write("      "); // 넉넉하게 공백 6칸 정도 출력
        }
    }
    
    private void DrawTopUI()
    {
        // 맵 바로 위쪽 한 줄 (_startY - 2 위치)
        int uiY = _startY - 2;
    
        // 1. 기존 잔상 지우기 (깔끔하게)
        Console.SetCursorPosition(0, uiY);
        Console.Write(new string(' ', Console.WindowWidth)); 

        // 2. 현재 맵 이름 출력 (왼쪽 정렬)
        Console.SetCursorPosition(_startX, uiY);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[ 태초 마을 ]"); // 나중에 맵 이름 변수로 교체 가능
        Console.ResetColor();

        // 3. (선택) 조작 가이드 출력 (오른쪽 정렬)
        string guide = "I:인벤토리  T:자해(?)";
        int guideX = _startX + _field.GetLength(1) - guide.Length + 2; // 맵 우측 끝에 맞춤
    
        if (guideX > _startX) // 겹치지 않을 때만 출력
        {
            Console.SetCursorPosition(guideX, uiY);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(guide);
            Console.ResetColor();
        }
    }
}