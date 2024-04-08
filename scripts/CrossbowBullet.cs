using Godot;
using System;

public partial class CrossbowBullet: Bullet
{

    int hitcounter = 0;
    protected override void OnHit(Entity e)
    {
        hitcounter++;
        // Emit the onhit signal
        EmitSignal(SignalName.Hit, e);
        if (locally_owned)
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