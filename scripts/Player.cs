using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Direkte Tastenabfrage für WASD-Bewegung (Tastatur)
		Vector3 direction = Vector3.Zero;
		if (Input.IsKeyPressed(Key.W)) // Vorwärts
			direction.Z -= 1;
		if (Input.IsKeyPressed(Key.S)) // Rückwärts
			direction.Z += 1;
		if (Input.IsKeyPressed(Key.A)) // Links
			direction.X -= 1;
		if (Input.IsKeyPressed(Key.D)) // Rechts
			direction.X += 1;

		//Joystickeingaben vermutlich noch nicht richtig
		/*
		direction += new Vector3(
			-Input.GetActionStrength("move_left") + Input.GetActionStrength("move_right"),
			0,
			-Input.GetActionStrength("move_forward") + Input.GetActionStrength("move_back")
		);
		*/

		// Normalisieren der Richtung und Anwenden der Geschwindigkeit
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
