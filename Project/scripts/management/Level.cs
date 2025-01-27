using Godot;

// ReSharper disable All

public class Level : Node2D
{
    private Player _player;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _player = GetNode<Player>("Player");
        _player.Connect(nameof(PlayerTexture.GameOver), this, nameof(OnGameOver));
    }

    private void OnGameOver()
    {
        GetTree().ReloadCurrentScene();
    }

}
