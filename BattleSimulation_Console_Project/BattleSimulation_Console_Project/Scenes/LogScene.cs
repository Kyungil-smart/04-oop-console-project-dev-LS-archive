using System;

public class LogScene : Scene
{
    public override void Update()
    {
        if (InputManager.GetKey(ConsoleKey.Enter) || InputManager.GetKey(ConsoleKey.Escape))
        {
            SceneManager.ChangePrevScene();
        }
    }

    public override void Render()
    {
        if (!NeedsRedraw) return;
        Debug.Render();
        NeedsRedraw = false;
    }

    public override void Enter()
    {
        base.Enter();
        NeedsRedraw = true;
    }

    public override void Exit()
    {
        Console.Clear();
        Console.ResetColor();
    }
}