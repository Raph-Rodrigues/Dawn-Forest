// ReSharper disable All
using Godot;
using System;

public class DetectionArea : Area2D
{
    // Declare member variables here
    [Export] private NodePath enemyPath;
    private EnemyTemplate enemy;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        enemy = GetNode<EnemyTemplate>(enemyPath);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void On_Body_Entered(Player body)
    {
        enemy.playerRef = body;
    }

    private void On_Body_Exited(Player _body)
    {
        enemy.playerRef = null;
    }
}
