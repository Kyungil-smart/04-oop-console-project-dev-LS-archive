using System;

public class PlayerCharacter : GameObject
{
    public ObservableProperty<int> Health = new ObservableProperty<int>(5);
    public ObservableProperty<int> Mana = new ObservableProperty<int>(5);
    private string _healthGauge;
    private string _manaGauge;
    
    public Tile[,] Field { get; set; }
    private Inventory _inventory;
    public bool IsActiveControl { get; private set; }

    public PlayerCharacter() => Init();

    public void Init()
    {
        Symbol = 'P';
        IsActiveControl = true;
        Health.AddListener(SetHealthGauge);
        Mana.AddListener(SetManaGauge);
        _healthGauge = "■■■■■";
        _manaGauge = "■■■■■";
        _inventory = new Inventory(this);
    }

    public void Update()
    {
        if (InputManager.GetKey(ConsoleKey.I))
        {
            HandleControl();
            // 인벤토리를 열거나 닫을 때도 화면을 다시 그려야 함
            // (Scene에서 NeedsRedraw를 true로 바꾸게 유도)
            //return true;
        }
        
        // 조작권이 플레이어에게 있을 때만 이동
        if (IsActiveControl)
        {
            if (InputManager.GetKey(ConsoleKey.UpArrow)) Move(Vector.Up);
            if (InputManager.GetKey(ConsoleKey.DownArrow)) Move(Vector.Down);
            if (InputManager.GetKey(ConsoleKey.LeftArrow)) Move(Vector.Left);
            if (InputManager.GetKey(ConsoleKey.RightArrow)) Move(Vector.Right);
        }
        
        // 인벤토리 조작 중일 때만 메뉴 선택
        else if (_inventory.IsActive)
        {
            if (InputManager.GetKey(ConsoleKey.UpArrow)) _inventory.SelectUp();
            if (InputManager.GetKey(ConsoleKey.DownArrow)) _inventory.SelectDown();
            if (InputManager.GetKey(ConsoleKey.Enter)) _inventory.Select();
        }
        
        // 테스트 체력 감소
        if (InputManager.GetKey(ConsoleKey.T))
        {
            Health.Value = Math.Max(0, Health.Value - 1); // 0 이하로 떨어지지 않게 방어
        }
    }

    public void HandleControl()
    {
        _inventory.IsActive = !_inventory.IsActive;
        IsActiveControl = !_inventory.IsActive;
        Debug.LogWarning($"{_inventory._itemMenu.CurrentIndex}");
    }

    private void Move(Vector direction)
    {
        if (Field == null || !IsActiveControl) return;
        
        Vector current = Position;
        Vector nextPos = Position + direction;
        
        // 1. 맵 바깥은 아닌지?
        if (nextPos.X < 0 || nextPos.Y < 0 || nextPos.X >= Field.GetLength(1) || nextPos.Y >= Field.GetLength(0))
        {
            return;
        }
        // 2. 벽인지?

        GameObject nextTileObject = Field[nextPos.Y, nextPos.X].OnTileObject;

        if (nextTileObject != null)
        {
            if (nextTileObject is IInteractable)
            {
                (nextTileObject as IInteractable).Interact(this);
                // 아이템을 획득했다면 필드에서 제거 (잔상 및 중복 획득 방지)
                if (nextTileObject is Item) 
                {
                    Field[nextPos.Y, nextPos.X].OnTileObject = null;
                }
            }
            else 
            {
                // 상호작용 불가능한 '벽' 같은 오브젝트라면 이동 불가
                return; 
            }
        }

        // 위치 이동 처리
        Field[Position.Y, Position.X].OnTileObject = null;
        Field[nextPos.Y, nextPos.X].OnTileObject = this;
        Position = nextPos;
    }

    public void Render(int offsetX, int offsetY)
    {
        DrawHealthGauge(offsetX, offsetY);
        DrawManaGauge(offsetX, offsetY);
        _inventory.Render();
    }

    public void AddItem(Item item)
    {
        _inventory.Add(item);
    }

    public void DrawManaGauge(int offsetX, int offsetY)
    {
        Console.SetCursorPosition(offsetX + Position.X - 2, offsetY + Position.Y - 1);
        _manaGauge.Print(ConsoleColor.Blue);
    }

    public void DrawHealthGauge(int offsetX, int offsetY)
    {
        Console.SetCursorPosition(offsetX + Position.X - 2, offsetY + Position.Y - 2);
        _healthGauge.Print(ConsoleColor.Red);
    }
    
    private string GetGaugeString(int current, int max)
    {
        // 현재 체력만큼 ■, 나머지는 □ (최대 5칸 기준)
        return new string('■', Math.Clamp(current, 0, max)) + 
               new string('□', Math.Clamp(max - current, 0, max));
    }

    public void SetHealthGauge(int health)
    {
        _healthGauge = GetGaugeString(health, 5);
    }

    public void SetManaGauge(int mana)
    {
        _manaGauge = GetGaugeString(mana, 5);
    }

    public void Heal(int value)
    {
        Health.Value += value;
    }
}