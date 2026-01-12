using System;

public struct Tile
{
    // 타일 위에 뭐가 올라와있는지?
    public GameObject OnTileObject { get; set; }
    
    //원래 이 타일의 바닥 모양
    public char FloorSymbol { get; private set; }
    // 타일 위에 올라서면 발생해야 하는 이벤트
    public event Action OnStepPlayer;
    // 자신의 좌표
    public Vector Position { get; set; }
    
    public bool HasGameObject => OnTileObject != null;

    public Tile(Vector position, char floorSymbol = ' ')
    {
        Position = position;
        FloorSymbol = floorSymbol; // 기본값은 점(.)
        OnTileObject = null;
        OnStepPlayer = null;
    }
    
    // 현재 렌더링되어야 할 문자를 반환
    public char GetRenderSymbol()
    {
        if (HasGameObject)
        {
            return OnTileObject.Symbol;
        }
        else
        {
            return FloorSymbol; // 오브젝트 없으면 원래 바닥 모양 리턴
        }
    }
    
    public void SetObject(GameObject obj)
    {
        OnTileObject = obj;
    }
    
    public void ClearObject()
    {
        OnTileObject = null;
    }

    public void Print()
    {
        GetRenderSymbol().Print();
    }
}