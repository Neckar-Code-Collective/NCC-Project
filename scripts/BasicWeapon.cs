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
        if (CanShoot())
        {
            Shoot();
        }
    }

    private bool CanShoot()
    {
        return timeSinceLastShot > ShootDelay && currentClipsSize > 0 && canShoot;
    }

    private void Shoot()
    {
        Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
        if (newBullet == null) return;

        GetTree().Root.AddChild(newBullet);
        newBullet.Setup(GlobalTransform.Origin, GlobalTransform.Basis.X * MuzzleSpeed, DamagePerBullet);
       

        currentClipsSize--;
        timeSinceLastShot = 0;
        canShoot = false;
        rofTimer.Start();
    }

    private void OnRofTimerTimeout()
    {
        canShoot = true;
    }

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

/*using Godot;
using System;

/// <summary>
/// The main class for all basic weapons, e.g. everything projectile based with basic properties
/// </summary>
public partial class BasicWeapon : AbstractWeapon
{
    [Export] public PackedScene Bullet;
	/// <summary>
    /// The delay between every shot, in seconds
    /// </summary>
    [Export(PropertyHint.Range,"0.00001,2")]
    float shootDelay = 0.33333333333334f;

	/// <summary>
    /// The amount of bullets in a single magazine
    /// </summary>
    [Export]
    int clipSize = 30;

	/// <summary>
    /// The damage per bullet (duh)
    /// </summary>
	[Export]
    float damagePerBullet = 10;

	/// <summary>
    /// Node3d of the ejection point, should be a extra node placed at the top of the gun
    /// </summary>
	[Export]
    Node3D ejectionPoint = null;

	/// <summary>
    /// The Bullet prefab, from which bullets get instantiated. Must be of type Bullet
    /// </summary>
    [Export]
    PackedScene bulletPrefab = null;


    /// <summary>
    /// time since the last shot has been emitted, used for shootDelay
    /// </summary>
    float timeSinceLastShot = 0;

	/// <summary>
    /// amount of ammo currently loaded
    /// </summary>
    int currentClipsSize = 0;

    public override void _PhysicsProcess(double delta)
    {
        timeSinceLastShot += (float)delta;
    }

    public override void ShootInput(Vector3 velocity)
    {
        //check if can shoot
		if (CanShoot()){
            //time is sufficient, shoot
            Shoot(velocity);
        }
    }

	/// <summary>
    /// Determines if there is enough delay since the last shot and enough ammo
    /// </summary>
    /// <returns></returns>
	public bool CanShoot(){
        return timeSinceLastShot > shootDelay && currentClipsSize > 0;
    }

	/// <summary>
    /// Gets called when this weapon should eject a projectile.
    /// </summary>
	public void Shoot(Vector3 vel){
        timeSinceLastShot = 0;
        currentClipsSize--;

        var bullet = bulletPrefab.Instantiate() as Bullet;
        GetTree().Root.AddChild(bullet);
        bullet.Setup(ejectionPoint.GlobalPosition, vel, damagePerBullet);

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        currentClipsSize = clipSize;
    }

    public override void onDisable()
    {
		
    }

    public override void onEnable()
    {
        
    }

    public override void Reload()
    {
        currentClipsSize = clipSize;
    }
}
*/
