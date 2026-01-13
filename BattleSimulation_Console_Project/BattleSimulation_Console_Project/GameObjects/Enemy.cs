public enum EnemyType
{
    Slime,   // 0: 약함
    Goblin,  // 1: 보통
    Orc      // 2: 강함
}

public class Enemy : GameObject, IInteractable
{
    public string Name { get; set; }
    public int HP { get; set; }
    public int Damage { get; set; }
    public int MaxHP { get; set; }
    public ConsoleColor Color { get; set; }

    public Enemy(char symbol, string name, int hp, int damage, int x, int y, ConsoleColor color)
    {
        Symbol = symbol;
        Name = name;
        HP = hp;
        MaxHP = hp; // 생성시 초기 체력을 최대 체력으로
        Damage = damage;
        Position = new Vector(x, y);
        Color = color;
    }

    // 플레이어와 부딪혔을 때 호출
    public void Interact(PlayerCharacter player)
    {
        SceneManager.Change(new BattleScene(player, this));
    }
    
    // 적 생성 메서드
    public static Enemy Create(EnemyType type, int x, int y)
    {
        switch (type)
        {
            case EnemyType.Slime:
                // 심볼, 이름, HP, 데미지
                return new Enemy('S', "슬라임", 2, 1, x, y, ConsoleColor.Cyan);
                
            case EnemyType.Goblin:
                return new Enemy('G', "고블린", 3, 2, x, y, ConsoleColor.Green);
                
            case EnemyType.Orc:
                return new Enemy('O', "오크", 5, 3, x, y, ConsoleColor.Red);
                
            default:
                return new Enemy('E', "Enemy", 1, 1, x, y, ConsoleColor.Yellow);
        }
    }
}