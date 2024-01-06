using Godot;
using System;

public partial class bullettest : Area3D
{
	[Export] public float speed = 30;
	const double KILLTIME = 0.2;
	double timer = 0;
 	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 forwardDirection = GlobalTransform.Basis.Z.Normalized();
		GlobalTranslate(forwardDirection * speed * (float)delta);
		timer += delta;
		if (timer >= KILLTIME)
			QueueFree();
	}
}
