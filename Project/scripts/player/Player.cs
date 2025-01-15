using Godot;

// ReSharper disable All

public class Player : KinematicBody2D
{
    private PlayerTexture _playerSprite;
    private RayCast2D _wallRay;
    
    private Vector2 _velocity;

    private int direction = 1;
    private int _jumpCount = 0;
    
    public bool Landing = false;
    public bool Attacking = false;
    public bool Defending = false;
    public bool Crouching = false;
    public bool OnWall = false;

    private bool _canTrackInput = true;
    private bool _notOnWall = true;

    [Export] private int _speed;
    [Export] private int _jumpSpeed;
    [Export] private int _wallJumpSpeed;

    [Export] private int _playerGravity;
    [Export] private int _wallGravity;
    [Export] private int _wallImpulseSpeed;
    
    public override void _Ready()
    {
        _playerSprite = GetNode<PlayerTexture>("Texture");
        _wallRay = GetNode<RayCast2D>("WallRay");
    }

    public override void _PhysicsProcess(float delta)
    {
        Horizontal_Movement_Env();
        Vertical_Movement_Env();
        Actions_Env();

        Gravity(delta);
        _velocity = MoveAndSlide(_velocity, Vector2.Up);
        _playerSprite.Animate(_velocity);
    }

    private void Horizontal_Movement_Env()
    {
        float inputDirection = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        if (!_canTrackInput || Attacking)
        {
            _velocity.x = 0;
            return;
        }
        _velocity.x = inputDirection * _speed;
    }

    private void Vertical_Movement_Env()
    {
        if (IsOnFloor() || IsOnWall())
        {
            _jumpCount = 0;
        }

        bool jumpCondition = _canTrackInput && !Attacking;
        if (Input.IsActionJustPressed("ui_select") && _jumpCount < 2 && jumpCondition)
        {
            _jumpCount += 1;

            if (NextToWall() && !IsOnFloor())
            {
                _velocity.y = _wallJumpSpeed;
                _velocity.x = _wallImpulseSpeed * direction;
            }
            else
            {
                _velocity.y = _jumpSpeed;   
            }
        }
    }

    private void Actions_Env()
    {
        Attack();
        Defense();
        Crouch();
    }

    private void Attack()
    {
        bool attackCondition = !Attacking && !Crouching && !Defending;
        if (Input.IsActionJustPressed("attack") && attackCondition && IsOnFloor())
        {
            Attacking = true;
            _playerSprite.normalAttack = true;
        }
    }

    private void Defense()
    {
        if (Input.IsActionPressed("defend") && IsOnFloor() && !Crouching)
        {
            Defending = true;
            _canTrackInput = false;
        } else if (!Crouching)
        {
            Defending = false;
            _canTrackInput = true;
            _playerSprite.shieldOff = true;
        }
    }

    private void Crouch()
    {
        if (Input.IsActionPressed("crouch") && IsOnFloor() && !Defending)
        {
            Crouching = true;
            _canTrackInput = false;
        } else if (!Defending)
        {
            Crouching = false;
            _canTrackInput = true;
            _playerSprite.crouchOff = true;
        }
    }

    private void Gravity(float delta)
    {
        if (NextToWall())
        {
            _velocity.y += _wallGravity * delta;
            if (_velocity.y >= _wallGravity)
            {
                _velocity.y = _wallGravity;
            }
        }
        else
        {
            _velocity.y += _playerGravity * delta;
            if (_velocity.y >= _playerGravity)
            {
                _velocity.y = _playerGravity;
            }   
        }
    }

    private bool NextToWall()
    {
        if (_wallRay.IsColliding() && !IsOnFloor())
        {
            if (_notOnWall)
            {
                _velocity.y = 0;
                _notOnWall = false;
            }
            
            return true;
        }
        else
        {
            _notOnWall = true;
            return false;
        }
    }
}
