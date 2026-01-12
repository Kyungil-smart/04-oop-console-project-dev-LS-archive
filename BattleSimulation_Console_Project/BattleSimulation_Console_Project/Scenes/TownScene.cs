using System;
using System.Text;

public class TownScene : Scene
{
    private Ractangle _mapOutline;
    private Ractangle _infoOutline;
    // 맵 크기: 실제 플레이 영역(20x40) + 테두리(2) = 22x42
    private Tile[,] _field = new Tile[22, 42];
    private PlayerCharacter _player;
    
    // 맵 그릴 시작 위치 (테두리나 여백을 위해서 띄움)
    private int _mapStartX = 2;
    private int _mapStartY = 2;
    
    // UI는 맵 끝나는 지점에서 4칸 정도 띄워서 시작
    private int _uiStartX; 
    private int _uiStartY = 2;
    
    // 최적화용 스트링 빌더
    private StringBuilder _sb = new StringBuilder();
    
    // 인벤토리 상태 추적 (이전 프레임의 상태)
    private bool _wasInventoryOpen = false;
    
    public TownScene(PlayerCharacter player) => Init(player);

    public void Init(PlayerCharacter player)
    {
        _player = player;
        int height = _field.GetLength(0);
        int width = _field.GetLength(1);
        
        // UI 시작 X좌표 자동 계산 (맵 너비 + 여백)
        _uiStartX = _mapStartX + width + 4;

        // 맵 외곽선 설정
        _mapOutline.Width = width + 2;
        _mapOutline.Height = height + 2;
        _mapOutline.X = _mapStartX - 1;
        _mapOutline.Y = _mapStartY - 1;
        
        // 정보창 외곽선 설정
        _infoOutline.Width = width + 2;
        _infoOutline.Height = height + 2;
        _infoOutline.X = _uiStartX - 1;
        _infoOutline.Y = _uiStartY - 1;
        
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

        _player._inventory.Init(_mapStartX, _mapStartY, _field.GetLength(1), _field.GetLength(0));
        
        // 아이템 배치
        _field[3, 5].OnTileObject = new Potion() {Name = "Potion1"};
        _field[2, 15].OnTileObject = new Potion() {Name = "Potion2"};
        _field[7, 3].OnTileObject = new Potion() {Name = "Potion3"};
        _field[9, 19].OnTileObject = new Potion() {Name = "Potion4"};
        
        DrawTopBar(); //상단 정보바 출력
        PrintField(); // 맵 전체 그리기
        DrawInfoPanel(); // UI 전체 그리기
        
        // 플레이어 초기 위치 그리기
        _player.Render(_mapStartX, _mapStartY);
        
        NeedsRedraw = true;
        Debug.Log("타운 씬 진입");
    }

    public override void Update()
    {
        if (InputManager.GetKey(ConsoleKey.I))
        {
            _player.HandleControl();
            NeedsRedraw = true;
            return;
        }
        
        if (!_player.IsActiveControl) // 인벤토리 활성화 상태
        {
            bool hasInput = false;
            if (InputManager.GetKey(ConsoleKey.UpArrow)) 
            {
                _player._inventory.SelectUp();
                hasInput = true;
            }
            else if (InputManager.GetKey(ConsoleKey.DownArrow))
            {
                _player._inventory.SelectDown();
                hasInput = true;
            }
            else if (InputManager.GetKey(ConsoleKey.Enter)) 
            {
                _player._inventory.Select();
                hasInput = true;
            }

            // ★ 입력이 있었을 때만 화면 갱신 요청! (무한 갱신 방지)
            if (hasInput)
            {
                NeedsRedraw = true;
            }
            return;
        }
        
        // 일반 로직
        if (InputManager.GetKey(ConsoleKey.UpArrow) ||
            InputManager.GetKey(ConsoleKey.DownArrow) ||
            InputManager.GetKey(ConsoleKey.LeftArrow) ||
            InputManager.GetKey(ConsoleKey.RightArrow) ||
            InputManager.GetKey(ConsoleKey.T))
        {
            _player.Update();
            NeedsRedraw = true;
        }
        
        //if (InputManager.HasInput) NeedsRedraw = true;
    }

    public override void Render()
    {
        if(!NeedsRedraw) return;
        
        // 인벤토리가 방금 닫혔는지 체크
        bool inventoryJustClosed = _wasInventoryOpen && _player.IsActiveControl;
        
        if (inventoryJustClosed)
        {
            // 인벤토리 잔상 제거 (이때 테두리도 같이 지워질 수 있음)
            RestoreInventoryBackground();
            
            // 지워진 테두리를 복구하기 위해 맵을 다시 그림
            PrintField();
            
            // 인벤토리가 정보창(오른쪽 UI)까지 침범해서 인벤토리 테두리도 다시 그림
            _infoOutline.Draw();
            
            _player.Render(_mapStartX, _mapStartY);
        }
        else if (!_player.IsActiveControl) // 인벤토리 열려있음
        {
            _player._inventory.Render(); 
        }
        else // 일반 이동
        {
            // 플레이어 이동 처리
            // 플레이어의 위치가 바뀌었을시
            if (_player.Position.X != _player.PreviousPosition.X || 
                _player.Position.Y != _player.PreviousPosition.Y)
            {
                // 이전 위치 지우기 (그 자리에 있던 타일 다시 출력)
                int oldX = _mapStartX + _player.PreviousPosition.X;
                int oldY = _mapStartY + _player.PreviousPosition.Y;
            
                Console.SetCursorPosition(oldX, oldY);
                // 맵 데이터에서 해당 좌표의 타일을 읽어와 다시 찍어주기
                Tile oldTile = _field[_player.PreviousPosition.Y, _player.PreviousPosition.X];
                Console.Write(oldTile.OnTileObject != null ? oldTile.OnTileObject.Symbol : oldTile.FloorSymbol);

                // 이동 위치에 플레이어 그리기
                _player.Render(_mapStartX, _mapStartY);
            }
        }
        // UI 수치 업데이트 (값이 바뀌었을 때만 덮어쓰기)
        UpdateUIStats();
        
        // 현재 프레임의 인벤토리 상태 저장 (다음 프레임에서 비교용)
        _wasInventoryOpen = !_player.IsActiveControl;
        
        NeedsRedraw = false;
    }
    
    // 인벤토리 영역 선택적으로 복구 (제목 포함 + 맵과 정보창 사이 초기화를 위해 공백 영역 확장)
    private void RestoreInventoryBackground()
    {
        // 1. 인벤토리 정보를 가져옴
        var inv = _player._inventory;
        
        // MenuList의 외곽선 크기를 사용 (동적으로 변하는 값)
        int actualWidth = inv._itemMenu._outline.Width;
        int actualHeight = inv._itemMenu._outline.Height;
        
        // 외곽선이 설정되지 않은 경우 기본값 사용
        if (actualWidth <= 0) actualWidth = 30;
        if (actualHeight <= 0) actualHeight = 10;
        
        // 복구 영역 설정
        int startX = inv.X;
        int startY = inv.Y - 1; // 제목 라인 포함
        int width = Math.Max(actualWidth, 20); // 최소 20칸 (제목 "[ INVENTORY ]" 고려)
        int height = actualHeight + 2; // 제목(1) + 본문 + 가이드(1)
        
        // 안전 여백 추가 (잔상 완전 제거)
        width += 8;  // 좌우 여유
        height += 1; // 하단 여유
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 화면 좌표
                int screenX = startX + x;
                int screenY = startY + y;

                // 화면 좌표를 맵 배열 인덱스로 변환
                int mapX = screenX - _mapStartX;
                int mapY = screenY - _mapStartY;

                Console.SetCursorPosition(screenX, screenY);

                // 맵 범위 안이라면 타일을 그림
                if (mapX >= 0 && mapX < _field.GetLength(1) && 
                    mapY >= 0 && mapY < _field.GetLength(0))
                {
                    Tile tile = _field[mapY, mapX];
                    Console.Write(tile.OnTileObject != null ? tile.OnTileObject.Symbol : tile.FloorSymbol);
                }
                else
                {
                    // 맵 바깥(테두리 등)이라면 공백으로 잔상 제거
                    Console.Write(' ');
                }
            }
        }
        Console.ResetColor();
    }
    
    /*
    // 맵 위쪽 잔상 제거용 메서드
    private void ClearOutSideArea()
    {
        // 게이지가 그려질 수 있는 _startY 위쪽 2줄을 공백으로 덮어씀
        string cleanLine = new string(' ', _field.GetLength(1) + 4);
        for (int i = 1; i <= 2; i++)
        {
            int y = _mapStartY - i;
            if (y >= 0)
            {
                Console.SetCursorPosition(0, y);
                Console.Write(cleanLine);
            }
        }
    }*/

    public override void Exit()
    {
        // 나갈 때 정리
        if (_player.Field != null &&
            _player.Position.Y < _field.GetLength(0) &&
            _player.Position.X < _field.GetLength(1))
        {
            _field[_player.Position.Y, _player.Position.X].OnTileObject = null;
        }
        _player.Field = null;
    }

    private void PrintField()
    {
        _mapOutline.Draw();
        
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            _sb.Clear();
            
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Tile tile = _field[y, x];
                // 타일 위에 오브젝트(벽, 아이템 등)가 있으면 그 기호, 없으면 점(.)
                if (tile.OnTileObject != null)
                {
                    _sb.Append(tile.OnTileObject.Symbol);
                }
                else
                {
                    _sb.Append(' '); // 타일 기본 기호 (바닥)
                }
            }
            
            Console.SetCursorPosition(_mapStartX, _mapStartY + y);
            _sb.ToString().Print();
            
            // 왼쪽 여백 청소 (게이지가 왼쪽으로 빠져나간 경우 대비)
            // Console.SetCursorPosition(0, _mapStartY + y);
            // Console.Write(new string(' ', _mapStartX));
            
            // 절대 좌표로 커서 이동 후 한 글자 출력
            // 오른쪽 여백 청소 (게이지가 오른쪽으로 삐져나간 잔상 제거)
            // Console.SetCursorPosition(_mapStartX + _field.GetLength(1), _mapStartY + y);
            // Console.Write(" ");
        }
    }
    
    // UI 틀 드로우 (처음 한 번만 호출)
    private void DrawInfoPanel()
    {
        // 정보창 외곽선
        _infoOutline.Draw();
        UpdateUIStats(); // 수치는 여기서 호출
    }
    
    // 수치만 빠르게 갱신하는 메서드
    private void UpdateUIStats()
    {
        // 외곽선 안쪽 배치 (가시성을 위해 한칸 띄워서)
        int uiX = _uiStartX + 1;
        int uiY = _uiStartY;
        
        Console.SetCursorPosition(uiX, uiY);
        "[ STATUS ]".Print(ConsoleColor.Yellow);
        
        Console.SetCursorPosition(uiX, uiY + 2);
        $"NAME : {_player.playerName}".Print(ConsoleColor.White);
        
        // Player 데이터 읽어와서 출력
        Console.SetCursorPosition(uiX, uiY + 3);
        $"HP   : {_player._healthGauge} ({_player.Health.Value}/5)".Print(ConsoleColor.Red);

        Console.SetCursorPosition(uiX, uiY + 4);
        $"MP   : {_player._manaGauge} ({_player.Mana.Value}/5)".Print(ConsoleColor.Blue);

        // [ Battle LOG ] 영역
        int logY = uiY + 7;
        Console.SetCursorPosition(uiX, logY);
        "[ Battle LOG ]".Print(ConsoleColor.Yellow);

        //Console.SetCursorPosition(uiX, logY + 2);
        //$"> {배틀로그}".Print(ConsoleColor.Gray);
        Console.ResetColor();
    }
    
    private void DrawTopBar()
    {
        int uiY = _mapStartY - 2;

        Console.SetCursorPosition(_mapStartX, uiY);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[ 태초 마을 ]"); 
        Console.ResetColor();

        string guide = "화살표:이동  I:인벤토리  T:데미지 테스트  Enter:선택";
        int guideX = _mapStartX + _field.GetLength(1) - guide.Length + 10;
    
        if (guideX > _mapStartX) 
        {
            Console.SetCursorPosition(guideX, uiY);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(guide);
            Console.ResetColor();
        }
    }
}