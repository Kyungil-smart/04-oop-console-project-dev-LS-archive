using System;

public class TownScene : Scene
{
    private Tile[,] _field = new Tile[10, 20];
    private PlayerCharacter _player;
    
    public TownScene(PlayerCharacter player) => Init(player);

    public void Init(PlayerCharacter player)
    {
        _player = player;
        
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Vector pos = new Vector(x, y);
                _field[y, x] = new Tile(pos);
            }
        }
    }

    public override void Enter()
    {
        _player.Field = _field;
        _player.Position = new Vector(4, 2);
        _field[_player.Position.Y, _player.Position.X].OnTileObject = _player;

        _field[3, 5].OnTileObject = new Potion() {Name = "Potion1"};
        _field[2, 15].OnTileObject = new Potion() {Name = "Potion2"};
        _field[7, 3].OnTileObject = new Potion() {Name = "Potion3"};
        _field[9, 19].OnTileObject = new Potion() {Name = "Potion4"};
        
        Debug.Log("타운 씬 진입");
    }

    public override void Update()
    {
        _player.Update();
    }

    public override void Render()
    {
        PrintField();
        _player.Render();
    }

    public override void Exit()
    {
        _field[_player.Position.Y, _player.Position.X].OnTileObject = null;
        _player.Field = null;
    }

    private void PrintField()
    {
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                _field[y, x].Print();
            }
            Console.WriteLine();
        }
    }
}