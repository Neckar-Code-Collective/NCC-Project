using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	private const float JoyStickDeadZone = 0.1f;   //DeadZone-Value
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;


		// Direct query for WASD-Movement (Keyboard)
		Vector3 direction = Vector3.Zero;
		if (Input.IsKeyPressed(Key.W)) // forward
			direction.Z -= 1;
		if (Input.IsKeyPressed(Key.S)) // backward
			direction.Z += 1;
		if (Input.IsKeyPressed(Key.A)) // left
			direction.X -= 1;
		if (Input.IsKeyPressed(Key.D)) // rechts
			direction.X += 1;

		
		
		// Joystick-Eingaben 
		float joystickX = -Input.GetActionStrength("ui_left") + Input.GetActionStrength("ui_right");
		float joystickZ = -Input.GetActionStrength("ui_up") + Input.GetActionStrength("ui_down");

		if (Math.Abs(joystickX) > JoyStickDeadZone)
			direction.X += joystickX;
		if (Math.Abs(joystickZ) > JoyStickDeadZone)
			direction.Z += joystickZ;
		

		// normalizing vector
		direction = direction.Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.Lerp(Velocity.X, 0, Speed * (float)delta);
			velocity.Z = Mathf.Lerp(Velocity.Z, 0, Speed * (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
		
	}
}
