using Godot;
using System;

/// <summary>
/// The main class for all basic weapons, e.g. everything projectile based with basic properties
/// </summary>
public partial class BasicWeapon : AbstractWeapon
{
	[Export] public PackedScene BulletPrefab;
	[Export] public float MuzzleSpeed = 30;
	[Export] public float ShootDelay = 0.33333333333334f;
	[Export] public int ClipSize = 30;
	[Export] public float DamagePerBullet = 10;
	[Export] public Marker3D Muzzle;

	private Timer rofTimer;
	private bool canShoot = true;
	private float timeSinceLastShot = 0;
	private int currentClipsSize;

	public override void _Ready()
	{
		rofTimer = new Timer();
		AddChild(rofTimer);
		rofTimer.WaitTime = ShootDelay;
		rofTimer.OneShot = true;
		rofTimer.Timeout+= OnRofTimerTimeout;
		currentClipsSize = ClipSize;
	}

	public override void _PhysicsProcess(double delta)
	{
		timeSinceLastShot += (float)delta;
		
	}

	private bool CanShoot()
	{
		return currentClipsSize > 0 && canShoot;
	}

	private void Shoot()
	{
		Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
		if (newBullet == null) return;

		GetTree().Root.AddChild(newBullet);
		newBullet.Setup(Muzzle.GlobalPosition, Muzzle.GlobalBasis.Z*30, DamagePerBullet,true);
        newBullet.SetCollisionMaskForPlayerBullet();
        Rpc(nameof(RpcShoot),Muzzle.GlobalPosition,Muzzle.GlobalBasis.Z*30,0);


        currentClipsSize--;
		timeSinceLastShot = 0;
		canShoot = false;
		rofTimer.Start();
	}

    public override void RpcShoot(Vector3 pos, Vector3 vel, int data)
    {
        Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
        newBullet.SetCollisionMaskForPlayerBullet();
        GetTree().Root.AddChild(newBullet);

        newBullet.Setup(pos, vel, 0, false);
    }

    public override void ShootInput(Vector3 velocity)
    {
        if(CanShoot()){
            Shoot();
        }
    }

    

	private void OnRofTimerTimeout()
	{
		canShoot = true;
	}

	/// <summary>
	/// Called when the weapon is disabled (e.g., when unequipped).
	/// </summary>
	public override void onDisable()
	{
		Visible = false;
	}

	public override void onEnable()
	{
		Visible = true;
	}


	public override void Reload()
	{
		currentClipsSize = ClipSize;
		
	}

	
}
