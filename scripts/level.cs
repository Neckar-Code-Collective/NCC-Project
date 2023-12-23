using Godot;
using System;




public partial class level : Node3D
{
	private const double JOYSTICKSENSITIVITY = 0.2f; 
	private const float RAYLENGHT = 2000f; 

	public override void _PhysicsProcess(double delta)
	{

		Godot.Vector2 mousePosition =  GetViewport().GetMousePosition();

		//Code for Player-Rotation with mouse
		if (IsMouseInViewport(mousePosition))   
		{
			ProcessMouseRotation(mousePosition);
		}

		ProcessJoystickRotation();
	}

	private bool IsMouseInViewport(Godot.Vector2 mousePosition)    //important to check if mouse is in viewport
	{
		return mousePosition.X >= 0 && mousePosition.Y >= 0 &&
			   mousePosition.X < GetViewport().GetVisibleRect().Size.X &&
			   mousePosition.Y < GetViewport().GetVisibleRect().Size.Y;   //assertion
	}

	private void ProcessMouseRotation(Godot.Vector2 mousePosition)
	{
			Godot.Vector3 rayOrigin = Godot.Vector3.Zero;
			Godot.Vector3 rayTarget = Godot.Vector3.Zero;
			CsgBox3D ground = GetNode<CsgBox3D>("Ground");
			Camera3D camera = GetNode<Camera3D>("Camera3D");		//Our Camera3D child Node
			rayOrigin = camera.ProjectRayOrigin(mousePosition);
			rayTarget = rayOrigin + camera.ProjectRayNormal(mousePosition) * RAYLENGHT;  //extended ray

			PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

			var rayQueryParameters = new PhysicsRayQueryParameters3D           //needed to get the intersection, null beachten, try-catch
			{
				From = rayOrigin,
				To = rayTarget
			};
			

			var intersection = spaceState.IntersectRay(rayQueryParameters);

			if (intersection != null && intersection.ContainsKey("collider"))    //collision mit spieler beachten
			{
				GodotObject collider = (GodotObject) intersection["collider"];
				Godot.Vector3 pos = (Godot.Vector3) intersection["position"];
				Player player = GetNode<Player>("Player");
				
				if (collider != null && collider!= player && (CsgBox3D)collider == ground)   // making sure that only collison with ground is recognized
				{
					Godot.Vector3 lookAtMe = new Godot.Vector3(pos.X, player.Position.Y , pos.Z);  //new überprüfung
					player.LookAt(lookAtMe, Godot.Vector3.Up);
				}
			}
	}

	private void ProcessJoystickRotation()
	{
		//Code for Player-Rotation with joystick
		float joystickRightX = Input.GetJoyAxis(0, JoyAxis.RightX);   //if abfrage sparen mit sensitivity untermethode in der abgefragt wird
		float joystickRightY = Input.GetJoyAxis(0, JoyAxis.RightY);

		if (Math.Abs(joystickRightX) > JOYSTICKSENSITIVITY|| Math.Abs(joystickRightY) > JOYSTICKSENSITIVITY)
		{
			Player player = GetNode<Player>("Player");
			// calculating new View out of right-joystick direction
			Godot.Vector3 targetDirection = new Godot.Vector3(joystickRightX, 0, joystickRightY).Normalized();
			Godot.Vector3 targetPosition = player.GlobalTransform.Origin + targetDirection;
			player.LookAt(targetPosition, Godot.Vector3.Up);
		}
	}
}
