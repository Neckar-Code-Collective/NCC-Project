using Godot;
using System;


/// <summary>
/// A shotgun that fires multiple bullets
/// </summary>
public partial class popcorngun: BasicWeapon
{
    private double ConvertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }
    /// <summary>
    /// Changed shot logic to shoot mulitple bullets in a cone
    /// </summary>
    protected override void Shoot()
    {
        //Instantiate a new bullet
        for (int i = 1; i < 6; i++)
        {
            float angle = (float)ConvertToRadians(-50 +20 * i);
            Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
            if (newBullet == null) return;

            newBullet.CollisionLayer = 0;
            //Add the bullet to the scene
            GetTree().Root.AddChild(newBullet);
            //setup its position, velocity and damage
            Vector3 muzz = (Muzzle.GlobalBasis.Rotated(Vector3.Up, angle )).Z * MuzzleSpeed;
            newBullet.Setup(Muzzle.GlobalPosition,muzz, DamagePerBullet, true);

            //sets the bullets collision mask, i.e. that it only collides with mobs and the environment
            newBullet.SetCollisionMaskForPlayerBullet();
            
        }

        //notify the other peers of our shoot
        Rpc(nameof(RpcShoot), Muzzle.GlobalPosition, Muzzle.GlobalBasis.Z * 30, 0);

        //restart timer, decrement ammo count
        _currentClipsSize--;
        _canShoot = false;
        _rofTimer.Start();
    }
    public override void RpcShoot(Vector3 position, Vector3 velocity, int data)
    {
        //Instantiate a new bullet
        for (int i = 1; i < 6; i++)
        {
            float angle = (float)ConvertToRadians(-50 +20 * i);
            Bullet newBullet = BulletPrefab.Instantiate<Bullet>();
            if (newBullet == null) return;

            newBullet.CollisionLayer = 0;
            //Add the bullet to the scene
            GetTree().Root.AddChild(newBullet);
            //setup its position, velocity and damage
            Godot.Vector3 muzz = (Muzzle.GlobalBasis.Rotated(Godot.Vector3.Up, angle )).Z * MuzzleSpeed;
            newBullet.Setup(position, muzz, DamagePerBullet, false);

            //sets the bullets collision mask, i.e. that it only collides with mobs and the environment
            newBullet.SetCollisionMaskForPlayerBullet();
            
        }
    }
}
