using Godot;
using System;
using System.Threading.Tasks.Sources;

public partial class ak47: BasicWeapon
{


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


}