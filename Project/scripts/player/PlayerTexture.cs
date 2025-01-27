using Godot;

// ReSharper disable All


public class PlayerTexture : Sprite
{
    [Signal]
    public delegate void GameOver();
    
    [Export] private NodePath _animationPath;
    private AnimationPlayer _animation;

    [Export] private NodePath _playerPath;
    private Player _player;

    [Export] private NodePath collisionPath;
    private CollisionShape2D collisionAttack;

    public bool normalAttack = false;
    private string suffix = "Right";

    public bool shieldOff = false;
    public bool crouchOff = false;


    public override void _Ready()
    {
        _animation = GetNode<AnimationPlayer>(_animationPath);
        _player = GetNode<Player>(_playerPath);
        collisionAttack = GetNode<CollisionShape2D>(collisionPath);
    }

    public void Animate(Vector2 direction)
    {
        VerifyPosition(direction);
        if (_player.onHit || _player.dead)
        {
            HitBehavior();
        }
        else if (_player.Attacking || _player.Defending || _player.Crouching || _player.Attacking || _player.NextToWall())
        {
            ActionBehavior();
        }
        else if (direction.y != 0)
        {
            VerticalBehavior(direction);
        } else if (_player.Landing == true)
        {
            _animation.Play("Landing");
            _player.SetPhysicsProcess(false);
        }
        else
        {
            HorizontalBehavior(direction);
        }
    }

    private void VerifyPosition(Vector2 direction)
    {
        if (direction.x > 0)
        {
            FlipH = false;
            suffix = "Right";
            _player.direction = -1;
            Position = Vector2.Zero;
            _player._wallRay.CastTo = new Vector2(5.5f, 0);
        }
        else if (direction.x < 0)
        {
            FlipH = true;
            suffix = "Left";
            _player.direction = 1;
            Position = new Vector2(-2, 0);
            _player._wallRay.CastTo = new Vector2(-7.5f, 0);
        }
    }

    private void HitBehavior()
    {
        _player.SetPhysicsProcess(false);
        collisionAttack.SetDeferred("disabled", true);
        if (_player.dead)
        {
            _animation.Play("Dead");
        }
        else if (_player.onHit)
        {
            _animation.Play("Hit");
        }
    }
    
    private void ActionBehavior()
    {
        if (_player.NextToWall())
        {
            _animation.Play("WallSlide");
        }
        else if (_player.Attacking && normalAttack)
        {
            _animation.Play("Attack" + suffix);
        } else if (_player.Defending && shieldOff)
        {
            _animation.Play("Shield");
            shieldOff = false;
        } else if (_player.Crouching && crouchOff)
        {
            _animation.Play("Crouch");
            crouchOff = false;
        }
    }

    private void VerticalBehavior(Vector2 direction)
    {
        if (direction.y > 0)
        {
            _player.Landing = true;
            _animation.Play("Fall");
        } else if (direction.y < 0)
        {
            _animation.Play("Jump");
        }
    }
    
    private void HorizontalBehavior(Vector2 direction)
    {
        _animation.Play(direction.x != 0 ? "Run" : "Idle");
    }

    private void On_Animation_Finished(string animationName)
    {
        switch (animationName)
        {
            case "Landing":
                _player.Landing = false;
                _player.SetPhysicsProcess(true);
                break;
            
            case "AttackLeft":
                normalAttack = false;
                _player.Attacking = false;
                break;
            
            case "AttackRight":
                _player.Attacking = false;
                break;
            case "Hit":
                _player.onHit = false;
                _player.SetPhysicsProcess(true);
                if (_player.Defending)
                {
                    _animation.Play("Shield");
                }

                if (_player.Crouching)
                {
                    _animation.Play("Crouch");
                }
                break;
            case "Dead":
                EmitSignal(nameof(GameOver));
                break;
        }
    }
}