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
    protected bool _locally_owned = true;

    /// <summary>
    /// After being alive for this many seconds, the bullet kills itself.
    /// </summary>
    [Export]
    protected float maxLifeTime = 10;

    /// <summary>
    /// The amount of time this bullet has been alive for
    /// </summary>
    float _aliveTime = 0;

    /// <summary>
    /// The amount of damage this bullet will inflict on the entity it hits
    /// </summary>
    [Export]
    protected float damage = 0;

    /// <summary>
    /// The velocity that gets applied to this bullet, in units per second (e.g. vel = (1,0,0) moves the bullet by 1 unit on the x-axis every second)
    /// </summary>
    protected Vector3 _velocity = Vector3.Zero;

    /// <summary>
    /// A callback for when an entity collides with this bullet
    /// </summary>
    /// <param name="other">the entity we collided with</param>
    [Signal]
    public delegate void HitEventHandler(Node3D other);

    /// <summary>
    /// Sets up the triggers
    /// </summary>
    public override void _Ready()
    {
        AreaEntered += onCollision_area;
        BodyEntered += onCollision_body;
    }

    /// <summary>
    /// Update the position of this bullet
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        //add the velocity to this bullets position
        GlobalPosition += _velocity * (float)delta;

        _aliveTime += (float)delta;

        //if we exceed the allowed alivetime, remove this bullet. This is to prevent bullets traveling indefinitly
        if (_aliveTime >= maxLifeTime)
        {
            QueueFree();
        }
    }

    /// <summary>
    /// Helper method to more easily setup a bullet
    /// </summary>
    /// <param name="pos">The position this bullet gets ejected from</param>
    /// <param name="vel">The velocity this bullet has</param>
    /// <param name="damage">The damage this bullet inflicts on its target</param>
    /// <param name="locally_owned">Whether this bullet is owned by the local peer. If not, we will ignore all collision</param>
	public void Setup(Vector3 pos, Vector3 vel, float damage, bool locally_owned)
    {
        this.damage = damage;
        GlobalPosition = pos;
        _velocity = vel;
        this._locally_owned = locally_owned;
        LookAt(Position + vel);

    }

    /// <summary>
    /// Sets up the bullet so that it collides with mobs and the terrain, but not players (to prevent friendly fire).
    /// </summary>
	public void SetCollisionMaskForPlayerBullet()
    {
        SetCollisionMaskValue(1, true);
        SetCollisionMaskValue(2, false);
        SetCollisionMaskValue(3, true);
    }

    /// <summary>
    /// Sets up the bullet so that it collides with players and the terrain, but not mobs (to prevent friendly fire).
    /// </summary>
	public void SetCollisionMaskForEnemyBullet()
    {
        SetCollisionMaskValue(1, true);
        SetCollisionMaskValue(2, true);
        SetCollisionMaskValue(3, false);
    }

    /// <summary>
    /// Callnback for when we collide with an area
    /// </summary>
    /// <param name="other">the area we collided with</param>
	void onCollision_area(Node3D other)
    {

        if (other is not Entity)
        {
            QueueFree();
            return;
        }
        OnHit((Entity)other);
    }

    /// <summary>
    /// Callback for when we collide with a body
    /// </summary>
    /// <param name="body"></param>
    void onCollision_body(Node3D body)
    {
        if (body is not Entity)
        {
            QueueFree();
            return;
        }
        OnHit((Entity)body);

    }

    /// <summary>
    /// Called when we hit an entity
    /// </summary>
    /// <param name="e">The entity we hit</param>
    protected virtual void OnHit(Entity e)
    {
        // Emit the onhit signal
        EmitSignal(SignalName.Hit, e);
        if (_locally_owned)
        {
            //we own this bullet, we should deal damage
            e.Rpc(nameof(e.RpcDealDamage), damage);
            QueueFree();
        }
        else
        {
            //we dont own this bullet, just remove it
            QueueFree();
        }
    }

    public float GetLifetime()
    {
        return _aliveTime;
    }

    public float GetDamage(){
        return damage;
    }

    public bool IsLocallySimulated(){
        return _locally_owned;
    }

}
