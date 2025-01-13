using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class Player : KinematicBody2D
{
    private Sprite _spriteNode;
    private Vector2 _velocity;

    private PlayerTexture _playerSprite;

    [Export] private int _speed;
    
    public override void _Ready()
    {
        _spriteNode = GetNode<Sprite>("Texture");
        _spriteNode = new PlayerTexture();
    }

    public override void _PhysicsProcess(float delta)
    {
        Horizontal_Movement_Env();
        _velocity = MoveAndSlide(_velocity);
        _playerSprite.Animate(_velocity);
    }

    private void Horizontal_Movement_Env()
    {
        float inputDirection = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        _velocity.x = inputDirection * _speed;
    }
}
