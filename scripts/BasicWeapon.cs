using Godot;
using System;

/// <summary>
/// The main class for all basic weapons, e.g. everything projectile based with basic properties
/// </summary>
public partial class BasicWeapon : AbstractWeapon
{
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
