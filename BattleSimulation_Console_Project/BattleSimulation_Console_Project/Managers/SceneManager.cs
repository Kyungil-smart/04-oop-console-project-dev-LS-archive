using System;
using System.Collections.Generic;

public static class SceneManager
{
    public static Action OnChangeScene;
    public static Scene Current { get; private set; }
    private static Scene _prev;

    private static Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

    public static void AddScene(string key, Scene state)
    {
        if (_scenes.ContainsKey(key)) return;
        
        _scenes.Add(key, state);
    }

    public static void ChangePrevScene()
    {
        Change(_prev);
    }

    // 상태 바꾸는 기능
    public static void Change(string key)
    {
        if (!_scenes.ContainsKey(key)) return;
        
        Change(_scenes[key]);
    }

    public static void Change(Scene scene)
    {
        Scene next = scene;
        
        if (Current == next) return;

        Current?.Exit();
        next.Enter();
        
        _prev = Current;
        Current = next;
        OnChangeScene?.Invoke();
    }

    public static void Update()
    {
        Current?.Update();
    }

    public static void Render()
    {
        Current?.Render();
    }
}