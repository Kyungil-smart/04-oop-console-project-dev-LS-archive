public enum PotionType
{
    HP, // 체력
    MP  // 마나
}

public class Potion : Item, IInteractable
{
    public PotionType Type { get; set; }
    public int Value { get; set; } // 회복량
    public ConsoleColor Color { get; set; } // 맵에 그릴 때 사용할 색상
    
    public Potion(PotionType type, int value)
    {
        Type = type;
        Value = value;
        Init();
    }
    
    private void Init()
    {
        Symbol = 'I';

        // 타입에 따라 이름과 색상을 자동 설정
        if (Type == PotionType.HP)
        {
            Name = "체력 포션";
            Color = ConsoleColor.Red;
        }
        else if (Type == PotionType.MP)
        {
            Name = "마나 포션";
            Color = ConsoleColor.Blue;
        }
    }

    public override void Use()
    {
        if (Owner == null) return;

        // 타입에 따라 다른 효과 적용
        switch (Type)
        {
            case PotionType.HP:
                Owner.Heal(Value);
                Debug.Log($"{Name}으로 HP {Value} 회복");
                break;

            case PotionType.MP:
                Owner.RestoreMana(Value);
                Debug.Log($"{Name}으로 HP {Value} 회복");
                break;
        }
        
        // 사용 후 인벤토리에서 제거
        Inventory.Remove(this);
        Inventory = null;
        Owner = null;
    }

    public void Interact(PlayerCharacter player)
    {
        player.AddItem(this);
    }
}