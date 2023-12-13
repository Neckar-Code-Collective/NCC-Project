using Godot;
using System;
using System.Linq;
using System.Numerics;



public partial class level : Node3D
{
	public Godot.Vector3 rayOrigin = Godot.Vector3.Zero;
	public Godot.Vector3 rayTarget = Godot.Vector3.Zero;
	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector2 mousePosition =  GetViewport().GetMousePosition();
		//GD.Print("Mouse Position", mouse_position);

		Camera3D camera = GetNode<Camera3D>("Camera3D");		//Our Camera3D child Node
		rayOrigin = camera.ProjectRayOrigin(mousePosition);
		//GD.Print("ray_origin: ", ray_origin);

		rayTarget = rayOrigin + camera.ProjectRayNormal(mousePosition) * 2000;  //extended ray

		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

		var rayQueryParameters = new PhysicsRayQueryParameters3D           //needed to get the intersection
		{
			From = rayOrigin,
			To = rayTarget
		};

		var intersection = spaceState.IntersectRay(rayQueryParameters);

		if (intersection!= null)
		{
			//GD.Print("Not Empty!");
			Godot.Vector3 pos = (Godot.Vector3) intersection["position"];

			Player player = GetNode<Player>("Player");
			Godot.Vector3 lookAtMe = new Godot.Vector3(pos.X, player.Position.Y , pos.Z);
			player.LookAt(lookAtMe, Godot.Vector3.Up);
		}
		else
		{
			GD.Print("No intersection detected");
		}
	}
}
