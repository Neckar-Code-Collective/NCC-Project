using Godot;
using System;

/// <summary>
/// The class for all projectile based bullets
/// </summary>
public partial class Bullet : Area3D
{

	/// <summary>
    /// Shows whether this bullet is simulated locally
    /// </summary>
    bool locally_owned = true;

    /// <summary>
    /// After being alive for this many seconds, the bullet kills itself.
    /// </summary>
    [Export]
    float maxLifeTime = 10;

    /// <summary>
    /// The amount of time this bullet has been alive for
    /// </summary>
    float aliveTime = 0;

	/// <summary>
    /// The amount of damage this bullet will inflict on the entity it hits
    /// </summary>
    [Export]
    float damage = 0;

	/// <summary>
    /// The velocity that gets applied to this bullet, in units per second (e.g. vel = (1,0,0) moves the bullet by 1 unit on the x-axis every second)
    /// </summary>
    Vector3 velocity = Vector3.Zero;

    [Signal]
    public delegate void HitEventHandler(Node3D other);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        AreaEntered += onCollision_area;
        BodyEntered += onCollision_body;
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += velocity * (float)delta;

        aliveTime += (float)delta;

		if(aliveTime >= maxLifeTime){
            QueueFree();
        }
    }

	public void Setup(Vector3 pos,Vector3 vel,float damage){
        this.damage = damage;
        GlobalPosition = pos;
        velocity = vel;
    }

	public void SetCollisionMaskForPlayerBullet(){
        SetCollisionMaskValue(1, true);
        SetCollisionMaskValue(2, false);
        SetCollisionMaskValue(3, true);
    }

	public void SetCollisionMaskForEnemyBullet(){
		SetCollisionMaskValue(1, true);
        SetCollisionMaskValue(2, true);
        SetCollisionMaskValue(3, false);
	}

	void onCollision_area(Node3D other){
		if(other is not Entity){
            GD.Print("Weird, i hit something, which is not an entity, i will just die");
            QueueFree();
            return;
        }
        OnHit((Entity)other);
    }

	void onCollision_body(Node3D body){
		if(body is not Entity){
            GD.Print("Weird, i hit something, which is not an entity, i will just die");
            QueueFree();
            return;
        }
        OnHit((Entity)body);
    }

	void OnHit(Entity e){
        GD.Print("HIT");
        if(locally_owned){
            //we own this bullet, we should deal damage
            e.Rpc(nameof(e.RpcDealDamage), damage);
            e.RpcDealDamage(damage);
            QueueFree();
        }
		else{
            //we dont own this bullet, just remove it
            QueueFree();
        }
	}

	public float GetLifetime(){
        return aliveTime;
    }
}
