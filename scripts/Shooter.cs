using Godot;
using System;

/// <summary>
/// The Shooter class represents a player character with abilities to move, jump, and rotate based on user input.
/// </summary>
public partial class Shooter : CharacterBody3D
{
	//TODO WeaponManager weapons;
	//TODO MoneyManager money;
	bool isLocalPlayer = false;
	/// <summary>Speed of the character's movement.</summary>
	public const float SPEED = 5.0f;

	/// <summary>Velocity of the character's jump.</summary>
	public const float JUMPVELOCITY = 4.5f;

	/// <summary>Deadzone value for joystick input.</summary>
	private const float JOYSTICKDEADZONE = 0.1f;   //DeadZone-Value

	/// <summary>Raylenght value for mouse intersection.</summary>
	private const float RAYLENGHT = 2000f;

	/// <summary>Gravity value affecting the character.</summary>
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private float _simulatedJoystickX = 0;
	private float _simulatedJoystickZ = 0;
	private Vector2 _simulatedJoystickRotation = Vector2.Zero;


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
	/// Handles the physics process for the character, including movement and rotation based on user input.
	/// </summary>
	/// <param name="delta">Time elapsed since the last frame.</param>
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JUMPVELOCITY;
		
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
			velocity.X = Mathf.Lerp(Velocity.X, 0, SPEED * (float)delta);
			velocity.Z = Mathf.Lerp(Velocity.Z, 0, SPEED * (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();

		Vector2 mousePosition = GetViewport().GetMousePosition();
		if(IsMouseInViewport(mousePosition))
		{
			ProcessMouseRotation(mousePosition);
		}
		ProcessJoystickRotation();
		
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
	/// Processes the rotation of the character based on mouse input.
	/// </summary>
	/// <param name="mousePosition">Current mouse position for calculating the rotation.</param>
	private void ProcessMouseRotation(Vector2 mousePosition)
	{
		Camera3D camera = GetCamera();
		CsgBox3D ground = GetGround();

		if (camera == null || ground == null) return;

		Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
		Vector3 rayTarget = rayOrigin + camera.ProjectRayNormal(mousePosition) * RAYLENGHT;


		var spaceState = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D rayQueryParameters = null;

		try
		{
			rayQueryParameters = new PhysicsRayQueryParameters3D           //needed to get the intersection
			{
				From = rayOrigin,
				To = rayTarget
			};
		}

		catch(OutOfMemoryException ex)
		{
			Console.WriteLine("Memory allocation error" + ex.Message);
			return;
		}

		var intersection = spaceState.IntersectRay(rayQueryParameters);
		if (intersection != null && intersection.ContainsKey("collider"))
		{
			var collider = intersection["collider"] as Object;
			Vector3 pos = (Vector3)intersection["position"];

			if (collider != null && collider != this)
			{
				try
				{
					Vector3 lookAtMe = new Vector3(pos.X, Position.Y, pos.Z);
					LookAt(lookAtMe, Vector3.Up);
				}
				catch(OutOfMemoryException ex)
				{
					Console.WriteLine("Memory allocation error" + ex.Message);
					return;
				}
			}
		}
	}

	/// <summary>
	/// Processes the rotation of the character based on joystick input.
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

	 public Vector3 GetLookDirection()
	{
		
		return -GlobalTransform.Basis.Z.Normalized();
	}

	[Rpc]
	public void RpcSetActiveWeapon(int weaponIndex)
	{

	}

	public virtual void Shoot()
	{

	}

}
