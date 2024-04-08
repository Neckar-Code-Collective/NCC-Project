using Godot;
using System;

/// <summary>
/// The main class for all basic weapons, e.g. everything projectile based with basic properties
/// </summary>
public partial class BasicWeapon : AbstractWeapon
{
    /// <summary>
    /// The blueprint for this weapons projectile
    /// </summary>
    [Export] public PackedScene BulletPrefab;

    /// <summary>
    /// The velocity with which bullets fired from this gun get ejected
    /// </summary>
    [Export] public float MuzzleSpeed = 30;

    /// <summary>
    /// The delay between each shot
    /// </summary>
    [Export] public float ShootDelay = 0.33333333333334f;

    /// <summary>
    /// The amount of bullets this gun holds in general
    /// </summary>
    [Export] public int ClipSize = 30;

    /// <summary>
    /// The damage this bullet inflicts on its target
    /// </summary>
    [Export] public float DamagePerBullet = 10;

    /// <summary>
    /// The location where the bullet gets spawned
    /// </summary>
    [Export] public Marker3D Muzzle;

    /// <summary>
    /// The timer which is used for the shootdelay
    /// </summary>
    protected Timer _rofTimer;

    /// <summary>
    /// Whether this gun can currently shoot
    /// </summary>
    protected bool _canShoot = true;

    /// <summary>
    /// The amount of bullets currently loaded
    /// </summary>
    protected int _currentClipsSize;

    /// <summary>
    /// Sets up the timer and ammunition
    /// </summary>
    public override void _Ready()
    {
        _rofTimer = new Timer();
        AddChild(_rofTimer);
        _rofTimer.WaitTime = ShootDelay;
        _rofTimer.OneShot = true;
        _rofTimer.Timeout += OnRofTimerTimeout;
        _currentClipsSize = ClipSize;
    }

    /// <summary>
    /// If the gun can currently shoot, i.e. if enough time has passed since the last shot
    /// </summary>
    /// <returns></returns>
    private bool CanShoot()
    {
        return _currentClipsSize > 0 && _canShoot;
    }

    /// <summary>
    /// Summons the actual bullet and notifies the other peers, that they should spawn a bullet as well
    /// </summary>
    protected virtual void Shoot()
    {
        //Instantiate a new bullet
        Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
        if (newBullet == null) return;

        //Add the bullet to the scene
        GetTree().Root.AddChild(newBullet);
        //setup its position, velocity and damage
        newBullet.Setup(Muzzle.GlobalPosition, Muzzle.GlobalBasis.Z * MuzzleSpeed, DamagePerBullet, true);
        //sets the bullets collision mask, i.e. that it only collides with mobs and the environment
        newBullet.SetCollisionMaskForPlayerBullet();

        //notify the other peers of our shoot
        Rpc(nameof(RpcShoot), Muzzle.GlobalPosition, Muzzle.GlobalBasis.Z * 30, 0);

        //restart timer, decrement ammo count
        _currentClipsSize--;
        _canShoot = false;
        _rofTimer.Start();
    }

    /// <summary>
    /// RPC, gets called on every peer except the one the shoot. This spawns a dummy bullet, which will only be visual and will not deal damage to anything.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="vel"></param>
    /// <param name="data"></param>
    public override void RpcShoot(Vector3 pos, Vector3 vel, int data)
    {
        //Instantiate bullet
        Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
        newBullet.SetCollisionMaskForPlayerBullet();
        GetTree().Root.AddChild(newBullet);

        //Set it up. Note that we set locally_owned to false, so this bullet wont issue collision responses
        newBullet.Setup(pos, vel, 0, false);
    }

    /// <summary>
    /// see ::ShootInput
    /// </summary>
    /// <param name="velocity"></param>
    public override void ShootInput(Vector3 velocity)
    {
        if (CanShoot())
        {
            Shoot();
        }
    }


    /// <summary>
    /// Callback for the timer, just resets the _canshoot variable
    /// </summary>
    private void OnRofTimerTimeout()
    {
        _canShoot = true;
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

    /// <summary>
    /// Reloads this gun to max ClipSize
    /// </summary>
    public override void Reload()
    {
        _currentClipsSize = ClipSize;

    }


}
