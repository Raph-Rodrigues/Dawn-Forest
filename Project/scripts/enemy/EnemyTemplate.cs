// ReSharper disable All
using Godot;
using System;

public class EnemyTemplate : KinematicBody2D
{
    // Node References 
    [Export] private Sprite texture;
    [Export] private RayCast2D floorRay;
    [Export] private AnimationPlayer animation;

    // Flags
    private bool canDie = false;
    private bool canHit = false;
    private bool canAttack = false;
    
    // direction var and player reference
    private Vector2 velocity;
    public Player playerRef = null;
    
    // Movement vars
    [Export] private int speed;
    [Export] private int gravitySpeed;
    [Export] private int proximityTreshold;
    [Export] private int raycastDefaultPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        texture = GetNode<Sprite>("Texture");
        floorRay = GetNode<RayCast2D>("FloorRay");
        animation = GetNode<AnimationPlayer>("Animation");
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        Gravity(delta);
        MoveBehavior();
        VerifyPosition();
        //texture.Animate(delta); // TODO: Calls the animations on texture node Script
        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    //Gravity SetUp
    public void Gravity(float delta)
    {
        velocity.y += delta * gravitySpeed;
    }

    // Movement SetUp
    public void MoveBehavior()
    {
        if (playerRef != null)
        {
            Vector2 distance = playerRef.Position - GlobalPosition;
            Vector2 direction = distance.Normalized();
            if (Math.Abs(distance.x) <= proximityTreshold)
            {
                velocity.x = 0;
                canAttack = true;
            } else if (FloorCollision() && !canAttack)
            {
                velocity.x = direction.x * speed;
            }
            else
            {
                velocity.x = 0;
            }

            return;
        }

        velocity.x = 0;
    }

    
    // Detecting the floor with Raycast2D
    private bool FloorCollision()
    {
        if (floorRay.IsColliding())
        {
            return false;
        }
        
        return false;
    }

    // Verifying the x position to flip the sprite setting the raycastDefaultPosiion
    private void VerifyPosition()
    {
        if (playerRef != null)
        {
            float direction = Sign(playerRef.GlobalPosition.x - GlobalPosition.x);

            if (direction > 0)
            {
                texture.FlipH = true;
                floorRay.Position.x = Math.Abs(raycastDefaultPosition);
            } else if (direction < 0)
            {
                texture.FlipH = false;
                floorRay.Position.x = raycastDefaultPosition;
            }
        }
    }
}
