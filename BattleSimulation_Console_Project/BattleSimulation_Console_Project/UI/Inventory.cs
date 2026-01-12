using System.Collections.Generic;

public class Inventory
{
    private PlayerCharacter _owner;
    private List<Item> _items = new List<Item>();
    
    public bool IsActive { get; set; }
    public MenuList _itemMenu = new MenuList();
    
    private int _x, _y;
    // 외부에서 인벤토리 위치와 크기 접근
    public int X => _x;
    public int Y => _y;

    public int invWidth;
    public int invHeight;
    
    // 이전 프레임의 인벤토리 크기 추적
    private int _lastRenderedHeight = 0;
    
    public Inventory(PlayerCharacter owner)
    {
        _owner = owner;
    }
    
    // TownScene에서 중앙 좌표를 계산하여 전달
    // 인벤토리가 맵 밖으로 벗어나지 않게 중앙 계산 추가.
    public void Init(int mapStartX, int mapStartY, int mapWidth, int mapHeight)
    {
        invWidth = _itemMenu._outline.Width > 0 ? _itemMenu._outline.Width : 30;
        invHeight = _itemMenu._outline.Height > 0 ? _itemMenu._outline.Height : 10;
        
        _x = mapStartX + (mapWidth - invWidth) / 2;
        _y = Math.Max(1, mapStartY + (mapHeight - invHeight) / 2); // 최소 1 이상 유지
    }

    public void Add(Item item)
    {
        if (_items.Count >= 10) return;
        
        _items.Add(item);
        _itemMenu.Add(item.Name, item.Use);
        item.Inventory = this;
        item.Owner = _owner;
    }

    public void Remove(Item item)
    {
        _items.Remove(item);
        _itemMenu.Remove();
    }

    public void Render()
    {
        if (!IsActive) return;
        
        /*// 인벤토리 영역 배경 지우기 (잔상 방지)
        // MenuList의 외곽선 크기를 참조
        string empty = new string(' ', _itemMenu._outline.Width);
        for (int i = 0; i < _itemMenu._outline.Height; i++)
        {
            Console.SetCursorPosition(_x, _y + i);
            Console.Write(empty);
        }*/
        // 현재 렌더링할 실제 크기 계산
        int actualHeight = _itemMenu._outline.Height > 0 ? _itemMenu._outline.Height : invHeight;
        int actualWidth = _itemMenu._outline.Width > 0 ? _itemMenu._outline.Width : invWidth;
        
        // 이전 프레임과 현재 프레임 중 더 큰 영역을 지움 (잔상 제거)
        int clearHeight = Math.Max(_lastRenderedHeight, actualHeight + 2); // +2는 제목과 가이드
        int clearWidth = Math.Max(actualWidth, 30) + 4; // 여유 공간
        
        string clearLine = new string(' ', clearWidth);
        for (int i = -1; i < clearHeight; i++) // 제목(-1)부터 가이드+여유까지
        {
            Console.SetCursorPosition(_x, _y + i);
            clearLine.PrintBackColor(ConsoleColor.DarkGray);
        }
        
        // 제목 출력 (외곽선 상단)
        Console.SetCursorPosition(_x + 1, _y - 1);
        "[ INVENTORY ]".Print(ConsoleColor.Yellow);

        // MenuList를 이용해 외곽선과 아이템 목록 출력
        if (_items.Count == 0)
        {
            // 아이템이 없을 때의 기본 외곽선 및 메시지
            _itemMenu._outline.X = _x;
            _itemMenu._outline.Y = _y;
            _itemMenu._outline.Draw();
            Console.SetCursorPosition(_x + 2, _y + 1);
            //"비어 있음".Print(ConsoleColor.DarkGray);
        }
        else
        {
            _itemMenu.Render(_x, _y);
        }
        
        // 하단 가이드 - 실제 외곽선 높이 기준으로 동적 배치
        Console.SetCursorPosition(_x, _y + actualHeight);
        "I:닫기 Enter:사용".Print(ConsoleColor.DarkGray);
        
        // 현재 프레임의 높이를 저장 (다음 프레임에서 비교용)
        _lastRenderedHeight = actualHeight + 2; // 가이드 포함
    }

    public void Select()
    {
        if(!IsActive) return;
        _itemMenu.Select();
    }

    public void SelectUp()
    {
        if(!IsActive) return;
        _itemMenu.SelectUp();
    }

    public void SelectDown()
    {
        if(!IsActive) return;
        _itemMenu.SelectDown();
    }
}