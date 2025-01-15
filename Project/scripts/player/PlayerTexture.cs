using Godot;

// ReSharper disable All


public class PlayerTexture : Sprite
{
    [Export] private NodePath _animationPath;
    private AnimationPlayer _animation;

    [Export] private NodePath _playerPath;
    private Player _player;

    public bool normalAttack = false;
    private string suffix = "Right";

    public bool shieldOff = false;
    public bool crouchOff = false;


    public override void _Ready()
    {
        _animation = GetNode<AnimationPlayer>(_animationPath);
        _player = GetNode<Player>(_playerPath);
    }

    public void Animate(Vector2 direction)
    {
        VerifyPosition(direction);
        if (_player.Attacking || _player.Defending || _player.Crouching)
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
        }
        else if (direction.x < 0)
        {
            FlipH = true;
            suffix = "Left";
        }
    }
    
    private void ActionBehavior()
    {
        if (_player.Attacking && normalAttack)
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
        }
    }
}