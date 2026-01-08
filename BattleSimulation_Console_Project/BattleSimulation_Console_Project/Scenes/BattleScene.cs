// Scenes/BattleScene.cs
public class BattleScene : Scene
{
    private Enemy _currentEnemy;
    private PlayerCharacter _player;

    // 배틀 시작 시 적 정보를 받아옴
    public void Setup(PlayerCharacter player, Enemy enemy)
    {
        _player = player;
        _currentEnemy = enemy;
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        // 1. 화면 그리기 (UI 박스, 적 정보, 내 정보)
        // 2. InputManager로 메뉴 선택 (공격, 도망)
        // 3. 결과 처리 (승리 시 TownScene으로 복귀)
    }

    public override void Render()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}