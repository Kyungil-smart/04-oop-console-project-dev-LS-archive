// GameObjects/Enemy.cs
public class Enemy : GameObject, IInteractable
{
    public string Name { get; set; }
    public int HP { get; set; }
    public int Damage { get; set; }

    public Enemy(string name, int x, int y)
    {
        Symbol = 'E'; // 맵에 E로 표시
        Name = name;
        Position = new Vector(x, y);
        HP = 50;
        Damage = 5;
    }

    // 플레이어와 부딪혔을 때 호출
    public void Interact(PlayerCharacter player)
    {
        // 배틀 씬으로 전환하며 플레이어의 정보를 넘김
    }
}