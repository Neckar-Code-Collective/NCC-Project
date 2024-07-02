using Godot;
using System;

/// <summary>
/// A bullet that can penertrate up to 3 enemies
/// </summary>
public partial class CrossbowBullet: Bullet
{

    /// <summary>
    /// how many entities have been hit already
    /// </summary>
    int _hitcounter = 0;
    /// <summary>
    /// Overriden OnHit function to add penetration functionality
    /// </summary>
    /// <param name="e"></param>
    protected override void OnHit(Entity e)
    {
        _hitcounter++;
        // Emit the onhit signal
        EmitSignal(SignalName.Hit, e);
        if (_locally_owned)
        {
            //we own this bullet, we should deal damage
            e.Rpc(nameof(e.RpcDealDamage), damage);
        
        }
        if(_hitcounter>=3)
        {
            //we dont own this bullet, just remove it
            QueueFree();
        }
    }
}