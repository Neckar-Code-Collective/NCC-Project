using Godot;
using System;




public partial class level : Node3D
{
	/*private const double JOYSTICKSENSITIVITY = 0.2f; 
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

	public void ProcessMouseRotation(Godot.Vector2 mousePosition)
	{
			Godot.Vector3 rayOrigin = Godot.Vector3.Zero;
			Godot.Vector3 rayTarget = Godot.Vector3.Zero;
			CsgBox3D ground = GetNode<CsgBox3D>("Ground");
			Camera3D camera = GetNode<Camera3D>("Camera3D");		//Our Camera3D child Node
			rayOrigin = camera.ProjectRayOrigin(mousePosition);
			rayTarget = rayOrigin + camera.ProjectRayNormal(mousePosition) * RAYLENGHT;  //extended ray

			PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
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
				return ;
			}

			var intersection = spaceState.IntersectRay(rayQueryParameters);

			if (intersection != null && intersection.ContainsKey("collider"))    //collision mit spieler beachten
			{
				GodotObject collider = (GodotObject) intersection["collider"];
				Godot.Vector3 pos = (Godot.Vector3) intersection["position"];
				Shooter shooter = GetNode<Shooter>("Shooter");
				
				if (collider != null && collider!= shooter && (CsgBox3D)collider == ground)   // making sure that only collison with ground is recognized
				{	
					try
					{
					Godot.Vector3 lookAtMe = new Godot.Vector3(pos.X, shooter.Position.Y , pos.Z);  //new überprüfung
					shooter.LookAt(lookAtMe, Godot.Vector3.Up);
					}
					catch(OutOfMemoryException ex)
					{
						Console.WriteLine("Memory allocation error" + ex.Message);
						return;
					}
				}
			}
	}

	public void ProcessJoystickRotation()
	{
		//Code for Player-Rotation with joystick
		float joystickRightX = Input.GetJoyAxis(0, JoyAxis.RightX);   //if abfrage sparen mit sensitivity untermethode in der abgefragt wird
		float joystickRightY = Input.GetJoyAxis(0, JoyAxis.RightY);

		if (Math.Abs(joystickRightX) > JOYSTICKSENSITIVITY|| Math.Abs(joystickRightY) > JOYSTICKSENSITIVITY)
		{
			Shooter shooter = GetNode<Shooter>("Shooter");
			// calculating new View out of right-joystick direction
			try
			{
			Godot.Vector3 targetDirection = new Godot.Vector3(joystickRightX, 0, joystickRightY).Normalized();
			Godot.Vector3 targetPosition = shooter.GlobalTransform.Origin + targetDirection;
			shooter.LookAt(targetPosition, Godot.Vector3.Up);
			}
			catch(OutOfMemoryException ex)
			{
				Console.WriteLine("Memory allocation error" + ex.Message);
				return;
			}
		}
	}
	*/
}

