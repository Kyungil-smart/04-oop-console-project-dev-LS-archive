public class Trap : GameObject, IInteractable
{
    public int Damage { get; set; } = 1;
    public Trap() => Init();

    public void Init()
    {
        Symbol = '^';
    }
    public void Interact(PlayerCharacter player)
    {
        player.Health.Value -= Damage;
        Debug.Log($"함정 데미지: {Damage}");
    }
}