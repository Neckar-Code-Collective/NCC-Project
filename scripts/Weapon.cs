using Godot;
using System;

public partial class Weapon : Node3D
{
	[Export] public PackedScene Bullet;
	private Timer rofTimer;   //rate of fire timer
	private bool canShoot = true;
	[Export] float MUZZLESPEED = 30;
	[Export] int MILLIESBETWEENSHOTS = 150;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rofTimer = GetNode<Timer>("Timer");
		rofTimer.WaitTime = MILLIESBETWEENSHOTS / 1000.0f;

		var timeoutMethod = new Callable(this, nameof(OnRofTimerTimeOut));
		rofTimer.Connect("timeout", timeoutMethod);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Shoot();
	}

	private void Shoot()
	{
		if (canShoot)
		{
			Area3D newBullet = Bullet.Instantiate() as Area3D;
			//Area3D newBulletObject = newBullet.GetChild<Area3D>("Bullet");
			if (newBullet == null) return;


			newBullet.GlobalTransform = GetNode<Marker3D>("Muzzle").GlobalTransform;
			//newBulletObject.speed = MUZZLESPEED;
			Node3D sceneRoot = GetTree().Root.GetChildren()[1] as Node3D;
			sceneRoot.AddChild(newBullet);
			GD.Print("pew!");
			canShoot = false;
			rofTimer.Start();
		}
	}

	private void OnRofTimerTimeOut()
	{
		GD.Print("you can shoot");
		canShoot = true;
	}
}
