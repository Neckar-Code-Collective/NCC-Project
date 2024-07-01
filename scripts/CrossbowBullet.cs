using Godot;
using System;

/// <summary>
/// A bullet that can penertrate up to 3 enemies
/// </summary>
public partial class CrossbowBullet: Bullet
{

    int hitcounter = 0;
    /// <summary>
    /// Overriden OnHit function to add penetration functionality
    /// </summary>
    /// <param name="e"></param>
    protected override void OnHit(Entity e)
    {
        hitcounter++;
        // Emit the onhit signal
        EmitSignal(SignalName.Hit, e);
        if (_locally_owned)
        {
            //we own this bullet, we should deal damage
            e.Rpc(nameof(e.RpcDealDamage), damage);
        
        }
        if(hitcounter>=3)
        {
            //we dont own this bullet, just remove it
            QueueFree();
        }
    }
}