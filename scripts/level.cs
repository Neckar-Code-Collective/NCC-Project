using Godot;
using System;
using System.Linq;
using System.Numerics;



public partial class level : Node3D
{
	public Godot.Vector3 rayOrigin = Godot.Vector3.Zero;
	public Godot.Vector3 rayTarget = Godot.Vector3.Zero;
	private const double JoystickSensitivity = 0.2f;

	private bool IsMouseInViewport(Godot.Vector2 mousePosition)    //important to check if mouse is in viewport
	{
		return mousePosition.X >= 0 && mousePosition.Y >= 0 &&
			   mousePosition.X < GetViewport().GetVisibleRect().Size.X &&
			   mousePosition.Y < GetViewport().GetVisibleRect().Size.Y;
	}
	public override void _PhysicsProcess(double delta)
	{

		Godot.Vector2 mousePosition =  GetViewport().GetMousePosition();
		CsgBox3D ground = GetNode<CsgBox3D>("Ground");

		//Code for Player-Rotation with mouse
		if (IsMouseInViewport(mousePosition))
		{
			
			

			Camera3D camera = GetNode<Camera3D>("Camera3D");		//Our Camera3D child Node
			rayOrigin = camera.ProjectRayOrigin(mousePosition);
			

			rayTarget = rayOrigin + camera.ProjectRayNormal(mousePosition) * 2000;  //extended ray

			PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

			var rayQueryParameters = new PhysicsRayQueryParameters3D           //needed to get the intersection
			{
				From = rayOrigin,
				To = rayTarget
			};
			

			var intersection = spaceState.IntersectRay(rayQueryParameters);

			if (intersection != null && intersection.ContainsKey("collider"))  
			{
				GodotObject collider = (GodotObject) intersection["collider"];
				Godot.Vector3 pos = (Godot.Vector3) intersection["position"];
				
				if (collider != null && (CsgBox3D)collider == ground)   // making sure that only collison with ground is recognized
				{
					Player player = GetNode<Player>("Player");
					Godot.Vector3 lookAtMe = new Godot.Vector3(pos.X, player.Position.Y , pos.Z);
					player.LookAt(lookAtMe, Godot.Vector3.Up);
				}
			}
		}

	 	//Code for Player-Rotation with joystick
		float joystickRightX = Input.GetJoyAxis(0, JoyAxis.RightX);
		float joystickRightY = Input.GetJoyAxis(0, JoyAxis.RightY);

		if (Math.Abs(joystickRightX) > JoystickSensitivity || Math.Abs(joystickRightY) > JoystickSensitivity)
		{
			Player player = GetNode<Player>("Player");
			// calculating new View out of right-joystick direction
			Godot.Vector3 targetDirection = new Godot.Vector3(joystickRightX, 0, joystickRightY).Normalized();
			Godot.Vector3 targetPosition = player.GlobalTransform.Origin + targetDirection;
			player.LookAt(targetPosition, Godot.Vector3.Up);
		}
	}
}
