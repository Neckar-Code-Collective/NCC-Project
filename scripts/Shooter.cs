using Godot;
using System;

/// <summary>
/// The Shooter class represents a player character with abilities to move, jump, and rotate based on user input.
/// </summary>
public partial class Shooter : Entity
{
	WeaponComponent weapons;
	//TODO MoneyManager money;

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

	//private Camera3D camera;

    [Export]
    AnimationPlayer _animPlayer;

	/// <summary>
    /// shows whether we are currently walking. Is used to start and stop the animation.
    /// </summary>
    bool _isWalking = false;


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

		if(!IsMultiplayerAuthority()){
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

		if (direction.Length() > 0.1 && !_isWalking){
            //This means we started walking, we should also start the animation player
            _isWalking = true;
            Rpc(nameof(_RPCPlayWalkAnimation), true);
        }

		else if(direction.Length() < 0.1 && _isWalking){
            //this means we were walking, but the player has stopped pressing the button
            _isWalking = false;
            Rpc(nameof(_RPCPlayWalkAnimation), false);
        }

		Velocity = velocity;
		MoveAndSlide();

		Vector2 mousePosition = GetViewport().GetMousePosition();
		if(IsMouseInViewport(mousePosition))
		{
			ProcessMouseRotation(mousePosition);
		}
		ProcessJoystickRotation();

		if(Input.IsMouseButtonPressed(MouseButton.Left)){
            weapons.ShootAction();
        }

		MoneyLabel.Text = "MONEY IN THE BANK : " + _currentMoneyCount;
		Rpc(nameof(RpcUpdateHealthBar));


        Position = new Vector3(Position.X, 0.229f, Position.Z);

    }
	private Camera3D GetCamera()
	{
		return GetTree().Root.FindChild("Camera3D", true, false) as Camera3D;
	}

	private CsgBox3D GetGround()
	{
		return GetTree().Root.FindChild("Ground", true, false) as CsgBox3D;
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
		catch(OutOfMemoryException ex)
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

        if (!IsMultiplayerAuthority()){
			return;
        }

        Global.LocalShooter = this;
        _health.SetMaxHealth(10);
		_health.SetCurrentHealth(10);
		Area3D moneyCollector = GetNode<Area3D>("MoneyCollector");
		moneyCollector.BodyEntered += OnMoneyCollectorCollision;
        moneyCollector.AreaEntered += OnMoneyCollectorCollision;
        //var deathMethod = new Callable(this, nameof(HandleDeath));
        _health.onDeath += HandleDeath;
		//health.Connect("onDeath",deathMethod);
		_healthBar.Modulate = Colors.Green;
	}

	private void InitializeLabels()
	{
		ColorRect green = GetTree().Root.GetNode<ColorRect>("Level/CanvasLayer/Control/ColorRect");
		if(!Multiplayer.IsServer())
		{
			MoneyLabel.Visible = true;
			green.Visible = true;

		}
		else
		{
			MoneyLabel.Visible = false;
			green.Visible = false;
		}
	}

	public void OnMoneyCollectorCollision(Node3D other){
		if(!IsMultiplayerAuthority()){
            return;
        }

		if(other is Money m){
			_currentMoneyCount += m.GetMoneyAmount();
            m.Rpc(nameof(m.RPCRemove));
            // m.QueueFree();
        }

		if(other is WorldItem wi){
            GD.Print("collision with worldItem");
            //we try to pick up an item
            if(weapons.HasSpace()){
                weapons.EquipWeapon(wi.GetWeaponName());
                wi.RpcId(1,nameof(wi.RpcKill));
            }
		}

	}

	 private void HandleDeath()
	{
        Rpc(nameof(RpcDie));
    }

	/// <summary>
    /// gets called by the owning player to set the animation state on all peers
    /// </summary>
    /// <param name="shouldPlay">whether the animation should start or stop</param>
	[Rpc(CallLocal = true)]
	void _RPCPlayWalkAnimation(bool shouldPlay){
		if(shouldPlay){
            _animPlayer.Play();
        }
		else{
            _animPlayer.Pause();
            _animPlayer.Seek(0.5f);
        }
	}


	
}
