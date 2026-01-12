public abstract class Scene
{
    // 다시 그려줄 필요가 있는지 체크용 변수
    protected bool NeedsRedraw { get; set; } = true;
    
    public virtual void Enter()
    {
        Console.Clear();
        NeedsRedraw = true;
        if (GetType().Name == "LogScene") return; // 로그씬 디버깅 방지
        Debug.Log($"[Scene] {this.GetType().Name}, 생성 및 렌더 클리어");
    }
    public abstract void Update();
    public abstract void Render();
    public abstract void Exit();
}