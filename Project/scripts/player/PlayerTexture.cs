using Godot;
using System;
using System.Diagnostics;

public class PlayerTexture : Sprite
{
    [Export] private NodePath animationPath;
    private AnimationPlayer animation;


    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>(animationPath);
    }

    public void Animate(Vector2 direction)
    {
        VerifyPosition(direction);
        HorizontalBehavior(direction);
    }

    public void VerifyPosition(Vector2 direction)
    {
        if (direction.x > 0)
        {
            FlipH = false;
        }
        else if (direction.x < 0)
        {
            FlipH = true;
        }
    }

    public void HorizontalBehavior(Vector2 direction)
    {
        animation.Play(direction.x != 0 ? "Run" : "Idle");
    }

}
