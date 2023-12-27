using Godot;
using System;

public partial class Shooter : CharacterBody3D
{
	public const float SPEED = 5.0f;
	public const float JUMPVELOCITY = 4.5f;

	private const float JOYSTICKDEADZONE = 0.1f;   //DeadZone-Value
	private const float RAYLENGHT = 2000f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


	
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

	 private Vector3 GetJoystickInputDirection()
	{
		Vector3 direction = Vector3.Zero;
		float joystickX = -Input.GetActionStrength("ui_left") + Input.GetActionStrength("ui_right");
		float joystickZ = -Input.GetActionStrength("ui_up") + Input.GetActionStrength("ui_down");

		if (Math.Abs(joystickX) > JOYSTICKDEADZONE)
			direction.X += joystickX;
		if (Math.Abs(joystickZ) > JOYSTICKDEADZONE)
			direction.Z += joystickZ;

		return direction;
	}

	private bool IsMouseInViewport(Vector2 mousePosition)
	{
		return mousePosition.X >= 0 && mousePosition.Y >= 0 &&
			   mousePosition.X < GetViewport().GetVisibleRect().Size.X &&
			   mousePosition.Y < GetViewport().GetVisibleRect().Size.Y;
	}

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

	private void ProcessJoystickRotation()
	{
		float joystickRightX = Input.GetJoyAxis(0, JoyAxis.RightX);
		float joystickRightY = Input.GetJoyAxis(0, JoyAxis.RightY);

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
}
