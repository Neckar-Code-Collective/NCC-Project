using Godot;
using System;

/// <summary>
/// The Shooter class represents a player character with abilities to move, jump, and rotate based on user input.
/// </summary>
public partial class Shooter : Entity
{
    WeaponComponent weapons;

    int _currentMoneyCount = 0;

    /// <summary>Speed of the shooter's movement.</summary>
    public const float SPEED = 5.0f;

    /// <summary>Deadzone value for joystick input.</summary>
    private const float JOYSTICKDEADZONE = 0.1f;   //DeadZone-Value

    /// <summary>Raylenght value for mouse intersection.</summary>
    private const float RAYLENGHT = 2000f;


    private float _simulatedJoystickX = 0;
    private float _simulatedJoystickZ = 0;
    private Vector2 _simulatedJoystickRotation = Vector2.Zero;
    public Label MoneyLabel;

    private TextureRect _interactionPrompt;

    //private Camera3D camera;

    [Export]
    AnimationPlayer _animPlayer;

    /// <summary>
    /// shows whether we are currently walking. Is used to start and stop the animation.
    /// </summary>
    bool _isWalking = false;

    ShooterState _state = ShooterState.ALIVE;


    public void SetSimulatedJoystickInput(float x, float z)
    {
        _simulatedJoystickX = x;
        _simulatedJoystickZ = z;
    }

    public void SetSimulatedJoystickRotationInput(Vector2 rotationInput)
    {
        _simulatedJoystickRotation = rotationInput;
    }


    /// <summary>
    /// Handles the physics process for the shooter, including movement and rotation based on user input.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame.</param>
    public override void _PhysicsProcess(double delta)
    {

        if (!IsMultiplayerAuthority())
        {
            return;
        }

        MoneyLabel.Text = "MONEY IN THE BANK : " + _currentMoneyCount;
        Rpc(nameof(RpcUpdateHealthBar),_health.GetCurrentHealth(),_health.GetMaxHealth());

        
        if (_state == ShooterState.INJURED)
        {
            
            if(_health.GetCurrentHealth() >= _health.GetMaxHealth()){
                GlobalRotationDegrees = new Vector3();
                Rpc(nameof(RpcSetHardRotation), Vector3.Zero);
                _state = ShooterState.ALIVE;
                return;
            }

			if(_currentlyTouchingShooters > 0){
                _health.Heal(_currentlyTouchingShooters * 5f*(float)delta);
            }
			else{
            	Rpc(nameof(RpcDealDamage), 1 * delta);

			}
            return;
        }

        if (_state == ShooterState.DEAD)
        {
            return;
        }

        Vector3 velocity = Velocity;

        Vector3 direction = GetKeyboardInputDirection() + GetJoystickInputDirection();


        // normalizing vector
        direction = direction.Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * SPEED;
            velocity.Z = direction.Z * SPEED;
        }
        else
        {
            velocity.X = Mathf.Lerp(Velocity.X, 0, SPEED * (float)delta * 4);
            velocity.Z = Mathf.Lerp(Velocity.Z, 0, SPEED * (float)delta * 4);
        }

        if (direction.Length() > 0.1 && !_isWalking)
        {
            //This means we started walking, we should also start the animation player
            _isWalking = true;
            Rpc(nameof(_RPCPlayWalkAnimation), true);
        }

        else if (direction.Length() < 0.1 && _isWalking)
        {
            //this means we were walking, but the player has stopped pressing the button
            _isWalking = false;
            Rpc(nameof(_RPCPlayWalkAnimation), false);
        }

        Velocity = velocity;
        MoveAndSlide();

        Vector2 mousePosition = GetViewport().GetMousePosition();
        if (IsMouseInViewport(mousePosition))
        {
            ProcessMouseRotation(mousePosition);
        }
        ProcessJoystickRotation();

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            weapons.ShootAction();
        }




        Position = new Vector3(Position.X, 0.229f, Position.Z);

    }

    /// <summary>
    /// Calculates and returns the direction of keyboard input.
    /// </summary>
    /// <returns>Direction vector based on WASD keyboard input.</returns>
    private Vector3 GetKeyboardInputDirection()
    {
        Vector3 direction = Vector3.Zero;
        if (Input.IsKeyPressed(Key.W)) // forward
            direction.Z -= 1;
        if (Input.IsKeyPressed(Key.S)) // backward
            direction.Z += 1;
        if (Input.IsKeyPressed(Key.A)) // left
            direction.X -= 1;
        if (Input.IsKeyPressed(Key.D)) // right
            direction.X += 1;
        return direction;
    }

    public override void _UnhandledInput(InputEvent @event)
    {

        if (@event is InputEventKey k && k.Keycode == Key.E && k.Pressed && _currentInteractable != null && IsMultiplayerAuthority())
        {
            _currentInteractable.RpcId(1, nameof(_currentInteractable.RpcInteract), Multiplayer.GetUniqueId());
        }
    }

    /// <summary>
    /// Calculates and returns the direction of joystick input.
    /// </summary>
    /// <returns>Direction vector based on joystick input.</returns>
    public Vector3 GetJoystickInputDirection()
    {
        float joystickX = _simulatedJoystickX != 0 ? _simulatedJoystickX : Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        float joystickZ = _simulatedJoystickZ != 0 ? _simulatedJoystickZ : Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

        try
        {
            Vector3 direction = new Vector3(joystickX, 0, joystickZ);
            if (direction.Length() > JOYSTICKDEADZONE)
                return direction.Normalized();
            return Vector3.Zero;
        }
        catch (OutOfMemoryException ex)
        {
            Console.WriteLine("Memory allocation error" + ex.Message);
            return Vector3.Zero;
        }

    }

    /// <summary>
    /// Checks if the mouse is within the viewport.
    /// </summary>
    /// <param name="mousePosition">Current mouse position.</param>
    /// <returns>True if the mouse is within the viewport; otherwise, false.</returns>
    private bool IsMouseInViewport(Vector2 mousePosition)
    {
        return mousePosition.X >= 0 && mousePosition.Y >= 0 &&
               mousePosition.X < GetViewport().GetVisibleRect().Size.X &&
               mousePosition.Y < GetViewport().GetVisibleRect().Size.Y;
    }

    /// <summary>
    /// Processes the rotation of the shooter based on mouse input.
    /// </summary>
    /// <param name="mousePosition">Current mouse position for calculating the rotation.</param>
    private void ProcessMouseRotation(Vector2 mousePosition)
    {

        var mousePosInView = GetViewport().GetMousePosition();

        var camera = GetViewport().GetCamera3D();

        var origin = camera.ProjectRayOrigin(mousePosInView);
        var direction = camera.ProjectRayNormal(mousePosInView);

        var distance = -origin.Y / direction.Y;

        var target_position = origin + direction * distance;

        target_position.Y = GlobalPosition.Y;
        LookAt(target_position);


    }

    /// <summary>
    /// Processes the rotation of the shooter based on joystick input.
    /// </summary>
    private void ProcessJoystickRotation()
    {
        float joystickRightX = _simulatedJoystickRotation.X != 0 ? _simulatedJoystickRotation.X : Input.GetJoyAxis(0, JoyAxis.RightX);
        float joystickRightY = _simulatedJoystickRotation.Y != 0 ? _simulatedJoystickRotation.Y : Input.GetJoyAxis(0, JoyAxis.RightY);

        if (Math.Abs(joystickRightX) > JOYSTICKDEADZONE || Math.Abs(joystickRightY) > JOYSTICKDEADZONE)
        {
            Vector3 targetDirection = new Vector3(joystickRightX, 0, joystickRightY).Normalized();
            Vector3 targetPosition = GlobalTransform.Origin + targetDirection;
            LookAt(targetPosition, Vector3.Up);
        }
    }

    /// <summary>
    /// Returns the look direction of the shooter
    /// </summary>
    public Vector3 GetLookDirection()
    {

        return -GlobalTransform.Basis.Z.Normalized();
    }

    /// <summary>
    /// override deal damage so that we spawn blood
    /// </summary>
    /// <param name="amount"></param>
    public override void RpcDealDamage(float amount)
    {
        base.RpcDealDamage(amount);


        if(_state == ShooterState.ALIVE && IsMultiplayerAuthority()){
            Global.NetworkManager.Rpc(nameof(Global.NetworkManager.RpcSpawnBlood), GlobalPosition, Mathf.Round(amount));
        }
    }




    // Warum wurde das hier auskommentiert? Brauche ich für das Geldeinsammeln
    public override void _Ready()
    {
        base._Ready();

        weapons = GetNode<WeaponComponent>("WeaponComponent");

        MoneyLabel = GetTree().Root.GetNode<Label>("Level/CanvasLayer/Control/MoneyLabel");
        InitializeLabels();



        _animPlayer.Play("ArmatureAction_001");
        _animPlayer.Pause();
        _animPlayer.Seek(0.5f, true);

        if (!IsMultiplayerAuthority())
        {
            return;
        }

        Global.LocalShooter = this;
        _health.SetMaxHealth(10);
        _health.SetCurrentHealth(10);
        Area3D moneyCollector = GetNode<Area3D>("MoneyCollector");
        moneyCollector.BodyEntered += OnMoneyCollectorCollision;
        moneyCollector.AreaEntered += OnMoneyCollectorCollision;
        moneyCollector.AreaExited += OnMoneyCollectorAreaLeave;
        moneyCollector.BodyExited += OnMoneyCollectorAreaLeave;
        //var deathMethod = new Callable(this, nameof(HandleDeath));
        _health.ResetDeathHandler();
        _health.onDeath += HandleDeath;
        //health.Connect("onDeath",deathMethod);
        _healthBar.Modulate = Colors.Green;


    }

    private void InitializeLabels()
    {
        ColorRect green = GetTree().Root.GetNode<ColorRect>("Level/CanvasLayer/Control/ColorRect");
        if (!Multiplayer.IsServer())
        {
            MoneyLabel.Visible = true;
            green.Visible = true;

        }
        else
        {
            MoneyLabel.Visible = false;
            green.Visible = false;
        }

        _interactionPrompt = GetTree().Root.GetNode<TextureRect>("Level/CanvasLayer/ShooterInteractionPrompt");
    }

    public int GetMoney()
    {
        return _currentMoneyCount;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RpcDeductMoney(int amount)
    {
        _currentMoneyCount -= amount;
        if (IsMultiplayerAuthority())
            Rpc(nameof(RpcUpdateMoneyOnOtherPeers), _currentMoneyCount);
    }

    int _currentlyTouchingShooters = -1;
    public void OnMoneyCollectorCollision(Node3D other)
    {
        if (!IsMultiplayerAuthority())
        {
            return;
        }

        if (other is Money m)
        {
            _currentMoneyCount += m.GetMoneyAmount();
            m.RpcId(1, nameof(m.RPCRemove));
            Rpc(nameof(RpcUpdateMoneyOnOtherPeers), _currentMoneyCount);
            // m.QueueFree();
        }

        if (other is WorldItem wi)
        {
            GD.Print("collision with worldItem");
            //we try to pick up an item
            if (weapons.HasSpace())
            {
                weapons.EquipWeapon(wi.GetWeaponName());
                wi.RpcId(1, nameof(wi.RpcKill));
            }
        }

        if (other is Interactable i)
        {
            _currentInteractable = i;
            _interactionPrompt.Visible = true;
        }

		if(other is Shooter){
            
            _currentlyTouchingShooters++;
        }

    }
    Interactable _currentInteractable;

    private void OnMoneyCollectorAreaLeave(Node3D other)
    {
        if (other is Interactable i)
        {
            _interactionPrompt.Visible = false;
            _currentInteractable = null;
        }

		if(other is Shooter){
            _currentlyTouchingShooters--;
        }

    }

    private void HandleDeath()
    {
        // GD.Print("Calling handler");
        switch (_state)
        {
            case ShooterState.ALIVE:
                GD.Print("SHooter lost all health, to Injured state with him !");
                _state = ShooterState.INJURED;
                GlobalRotationDegrees = new Vector3(90, 0, 0);
                Rpc(nameof(RpcSetHardRotation), new Vector3(90, 0, 0));
                _health.Heal(_health.GetMaxHealth() - 1);
                GD.Print(_health.GetCurrentHealth());
                break;

            case ShooterState.INJURED:
                GD.Print("Player died again in INJURED state, he die now");
                _state = ShooterState.DEAD;
                break;
        }
    }

    /// <summary>
    /// gets called by the owning player to set the animation state on all peers
    /// </summary>
    /// <param name="shouldPlay">whether the animation should start or stop</param>
    [Rpc(CallLocal = true)]
    void _RPCPlayWalkAnimation(bool shouldPlay)
    {
        if (shouldPlay)
        {
            _animPlayer.Play();
        }
        else
        {
            _animPlayer.Pause();
            _animPlayer.Seek(0.5f);
        }
    }

    [Rpc()]
    public void RpcUpdateMoneyOnOtherPeers(int amount)
    {
        _currentMoneyCount = amount;
    }

    public void SetMoney(int a){
        _currentMoneyCount = a;
        Rpc(nameof(RpcUpdateMoneyOnOtherPeers), _currentMoneyCount);
    }

    [Rpc()]
    public void RpcSetHardRotation(Vector3 v){
        GlobalRotationDegrees = v;
    }

    public enum ShooterState
    {
        ALIVE, INJURED, DEAD
    }

    public ShooterState GetShooterState()
    {
        return _state;
    }
}
