using Godot;
using System;
using System.Threading.Tasks.Sources;

/// <summary>
/// Ak47 class, adds a little bit of randomness to the bullets
/// </summary>
public partial class ak47: BasicWeapon
{

    /// <summary>
    /// Gets called when a projectile is to be emitted, the same as the normal one except this time we add a little bit of spread
    /// </summary>
    protected override void Shoot()
    {
        //Instantiate a new bullet
        Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
        if (newBullet == null) return;

        //Add the bullet to the scene
        GetTree().Root.AddChild(newBullet);
        //setup its position, velocity and damage
        Vector3 muzz = (Muzzle.GlobalBasis.Rotated(Vector3.Up, (Random.Shared.NextSingle() - 0.5f)*0.25f)).Z * MuzzleSpeed;
        newBullet.Setup(Muzzle.GlobalPosition,muzz, DamagePerBullet, true);
        //sets the bullets collision mask, i.e. that it only collides with mobs and the environment
        newBullet.SetCollisionMaskForPlayerBullet();

        //notify the other peers of our shoot
        Rpc(nameof(RpcShoot), Muzzle.GlobalPosition,muzz, 0);

        //restart timer, decrement ammo count
        _currentClipsSize--;
        _canShoot = false; 
        _rofTimer.Start();
    }

    /// <summary>
    /// The RPC version of shoot.
    /// </summary>
    /// <param name="position">where the projectile should be emitted</param>
    /// <param name="velocity">the velocity of the projectile</param>
    /// <param name="data">null</param>
     public override void RpcShoot(Vector3 position, Vector3 velocity, int data)
    {
        //Instantiate a new bullet
        Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
        if (newBullet == null) return;

        //Add the bullet to the scene
        GetTree().Root.AddChild(newBullet);
        //setup its position, velocity and damage
        newBullet.Setup(position, velocity, DamagePerBullet, false);
        //sets the bullets collision mask, i.e. that it only collides with mobs and the environment
        newBullet.SetCollisionMaskForPlayerBullet();

       
    }



}