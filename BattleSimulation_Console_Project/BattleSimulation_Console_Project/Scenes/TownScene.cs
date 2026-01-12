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
    
    // 인벤토리가 열려있는 동안 기록된 최대 높이 (잔상 제거용)
    private int _maxOpenInvHeight = 0;
    
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
        _player.Position = new Vector(11, 11);
        _field[_player.Position.Y, _player.Position.X].OnTileObject = _player;

        _player._inventory.Init(_mapStartX, _mapStartY, _field.GetLength(1), _field.GetLength(0));
        
        // 아이템 배치
        _field[3, 5].OnTileObject = new Potion(PotionType.HP, 1) { Name = "초급 체력 포션" };
        _field[2, 30].OnTileObject = new Potion(PotionType.HP, 2) { Name = "중급 체력 포션" };
        _field[15, 38].OnTileObject = new Potion(PotionType.HP, 2) { Name = "고급 체력 포션" };
        _field[7, 6].OnTileObject = new Potion(PotionType.MP, 1) { Name = "초급 마나 포션" };
        _field[9, 34].OnTileObject = new Potion(PotionType.MP, 2) { Name = "중급 마나 포션" };
        _field[16, 26].OnTileObject = new Potion(PotionType.MP, 2) { Name = "고급 마나 포션" };
        //함정
        _field[9, 9].OnTileObject = new Trap();
        _field[17, 13].OnTileObject = new Trap();
        _field[11, 27].OnTileObject = new Trap();
        _field[18, 6].OnTileObject = new Trap();
        
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
            
            // 인벤토리가 닫혔으므로 정보창 UI를 다시 그리기
            UpdateUIStats();
            
            // 인벤토리가 닫혔으므로 최대 높이 기록 초기화
            _maxOpenInvHeight = 0;
        }
        else if (!_player.IsActiveControl) // 인벤토리 열려있음
        {
            _player._inventory.Render(); 
            
            // 열려있는 동안 인벤토리의 최대 높이를 계속 추적
            // (아이템을 써서 줄어들더라도, 가장 컸을 때의 값을 기억함)
            int currentHeight = _player._inventory.currentMaxHeight;
            if (currentHeight > _maxOpenInvHeight)
            {
                _maxOpenInvHeight = currentHeight;
            }
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
            UpdateUIStats();
        }
        
        // 현재 프레임의 인벤토리 상태 저장 (다음 프레임에서 비교용)
        _wasInventoryOpen = !_player.IsActiveControl;
        
        NeedsRedraw = false;
    }
    
    // 인벤토리 영역 선택적으로 복구 (제목 포함 + 맵과 정보창 사이 초기화를 위해 공백 영역 확장)
    private void RestoreInventoryBackground()
    {
        // 인벤토리 정보를 가져옴
        var inv = _player._inventory;
        
        // Inventory.cs와 너비 계산 공식 일치화
        // Inventory.cs: int clearWidth = Math.Max(actualWidth, 30) + 4;
        int actualWidth = inv._itemMenu._outline.Width > 0 ? inv._itemMenu._outline.Width : 30;
        int clearWidth = Math.Max(actualWidth, 30) + 4;
        
        // 현재 높이가 아니라, 열려있는 동안 기록된 최대 높이를 사용
        int actualHeight = Math.Max(inv.currentMaxHeight, _maxOpenInvHeight);
        
        // 복구 영역 설정
        // 시작 위치: Inventory.cs가 y-2부터 그렸으므로 똑같이 y-2부터 지움
        int startX = inv.X;
        int startY = inv.Y - 2;
        
        // 지울 높이 설정
        int clearHeight = actualHeight + 2;
        
        /* 하드코딩 제거
        // 안전 여백 추가 (잔상 완전 제거)
        width += 8;  // 좌우 여유
        height += 1; // 하단 여유
        */
        
        // 배경색을 검은색으로 강제 초기화 (이전에 이걸 안해줘서 사간 낭비함)
        Console.BackgroundColor = ConsoleColor.Black; 
        
        for (int y = 0; y < clearHeight; y++)
        {
            for (int x = 0; x < clearWidth; x++)
            {
                // 화면 좌표
                int screenX = startX + x;
                int screenY = startY + y;

                // 화면 좌표를 맵 배열 인덱스로 변환
                int mapX = screenX - _mapStartX;
                int mapY = screenY - _mapStartY;
                
                // 화면 밖 예외 처리 (안전장치)
                if (screenX >= Console.WindowWidth || screenY >= Console.WindowHeight) continue;

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
            Console.SetCursorPosition(_mapStartX, _mapStartY + y);
            _sb.Clear();
            
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Tile tile = _field[y, x];
                // 오브젝트(포션 등)를 만났을 때
                if (tile.OnTileObject != null)
                {
                    // 지금까지 StringBuilder에 쌓인 일반 타일들을 먼저 출력
                    if (_sb.Length > 0)
                    {
                        Console.Write(_sb.ToString());
                        _sb.Clear();
                    }

                    // 포션인 경우 색상 지정해서 출력
                    if (tile.OnTileObject is Potion potion)
                    {
                        potion.Symbol.Print(potion.Color);
                    }
                    else
                    {
                        // 일반 오브젝트(벽 등)는 기본색 출력
                        Console.Write(tile.OnTileObject.Symbol);
                    }
                }
                else
                {
                    // 2. 일반 타일(바닥)은 StringBuilder에 계속 쌓음
                    _sb.Append(tile.FloorSymbol);
                }
            }
            // 3. 줄 끝에 남은 StringBuilder 내용 마저 출력
            if (_sb.Length > 0)
            {
                Console.Write(_sb.ToString());
            }
            
            //Console.SetCursorPosition(_mapStartX, _mapStartY + y);
            //_sb.ToString().Print();
            
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
        "[ Field Info ]".Print(ConsoleColor.Yellow);
        
        Console.SetCursorPosition(uiX, logY + 2);
        "[ 포션 회복량: 초급: 1/중급: 2/고급: 3 ]".Print(ConsoleColor.Gray);
        // 1. 체력 포션 설명
        Console.SetCursorPosition(uiX, logY + 3);
        "> ".Print(ConsoleColor.Gray);
        'P'.Print(ConsoleColor.Red); // Potion의 'P'
        ": 체력 포션".Print(ConsoleColor.Gray);

        // 2. 마나 포션 설명
        Console.SetCursorPosition(uiX, logY + 4);
        "> ".Print(ConsoleColor.Gray);
        'P'.Print(ConsoleColor.Blue);
        ": 마나 포션".Print(ConsoleColor.Gray);
        
        Console.SetCursorPosition(uiX, logY + 5);
        "----------------------------".Print(ConsoleColor.DarkGray);

        // 3. 함정 설명
        Console.SetCursorPosition(uiX, logY + 6);
        "> ".Print(ConsoleColor.Gray);
        '^'.Print(ConsoleColor.DarkGray); // Trap의 '^'
        ": 가시 함정 (HP -1)".Print(ConsoleColor.Gray);

        // 4. 적 설명 (추후 확장용)
        Console.SetCursorPosition(uiX, logY + 7);
        "> ".Print(ConsoleColor.Gray);
        'S'.Print(ConsoleColor.Green);
        ": 적 슬라임 (전투 발생)".Print(ConsoleColor.Gray);
        

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