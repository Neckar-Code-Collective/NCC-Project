using Godot;
using System;

/// <summary>
/// BasicEnemy has the overall pathfinding functions targettet at the shooter
/// </summary>
public partial class Enemy : Entity
{

    /// <summary>
    /// The damage this enemy inflicts
    /// </summary>
    float _damage;

    /// <summary>
    /// The movement speed of this enemy
    /// </summary>
    float _movementSpeed = 3.0f;

    /// <summary>
    /// The amount of money this enemy drops on death
    /// </summary>
    protected int NetWorth = 5;




    /// <summary>
    /// nav_agent connected and creation of onready function
    /// aggroRange is the range at which the Enemy starts targeting a Shooter
    /// </summary>
    NavigationAgent3D nav_agent;

    /// <summary>
    /// The shooter this enemy is currently targeting
    /// </summary>
    Shooter target;

    [Export]
    protected float _attackCooldown = 0.5f;

    bool _canAttack = true;

    /// <summary>
    /// Setup references and death handler
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        _healthBar.Modulate = Colors.Red;
        nav_agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        var aggroRange = GetNode<Area3D>("Area3D");
        aggroRange.BodyEntered += (Node3D body) =>
        {

            if (body is Shooter)
            {
                target = (Shooter)body;
            }

        };

        //basic logic for dying
        _health.onDeath += () =>
        {
            if (IsMultiplayerAuthority())
            {

                Global.NetworkManager.SpawnMoney(GlobalPosition, NetWorth);
                Rpc(nameof(RpcDie));
            }
        };

        //setup attack timer
        var timer = new Timer();
        AddChild(timer);
        timer.Timeout += () => _canAttack = true;
        timer.OneShot = false;
        timer.WaitTime = _attackCooldown;
        timer.Start(_attackCooldown);
    }




    /// <summary>
    /// This function lets the Enemy move towords the shooter
    /// </summary>
    /// <param name="delta"></param>

    public override void _PhysicsProcess(double delta)
    { //Update Funktion
        base._PhysicsProcess(delta);

        //if we arent the server, we dont want to update the enemies
        if (!IsMultiplayerAuthority())
        {
            return;
        }

        //finds the next location to go to and moves there
        var current_location = GlobalTransform.Origin;
        var next_location = nav_agent.GetNextPathPosition();
        var new_velocity = (next_location - current_location).Normalized() * _movementSpeed;

        Velocity = new_velocity;
        // MoveAndSlide();
        GlobalPosition += new_velocity * (float)delta;

        //if we dont have a target we dont want to move at all
        if (target != null)
        {
            //We dont want to target dead players anymore
            if(target.GetShooterState() == Shooter.ShooterState.DEAD){
                target = null;
                return;
            }

            if (!IsInstanceValid(target))
            {
                target = null;
                return;
            }

            nav_agent.TargetPosition = target.GlobalPosition;

            //if we are close enough to a player, deal damage to them
            if(GlobalPosition.DistanceSquaredTo(target.GlobalPosition) > 1){

                LookAt(target.GlobalPosition, Vector3.Up);
            }
            Attack();
            Rpc(nameof(RpcUpdateHealthBar),_health.GetCurrentHealth(),_health.GetMaxHealth());
        }
        else{
            target = ChooseNewTarget();
        }

    }

    /// <summary>
    /// Check if we are in range of the shooter to attack him
    /// </summary>
    public void Attack()
    {
        var distance = target.GlobalPosition - this.GlobalPosition;
        if (distance.Length() < 1 && _canAttack)
        {
            target.Rpc(nameof(RpcDealDamage), 2);
            // target.RpcDealDamage(2);
            _canAttack = false;
        }

    }
    /// <summary>
    /// Queries the EntityManager for the nearest player
    /// </summary>
    /// <returns></returns>
    public Shooter ChooseNewTarget(){
        Shooter nearest = null;
        float distance = 9999999;

        foreach(Shooter s in EntityManager.GetShooters()){
            if(s.GetShooterState() != Shooter.ShooterState.ALIVE){
                continue;
            }

            float d = GlobalPosition.DistanceSquaredTo(s.GlobalPosition);
            if(d < distance){
                nearest = s;
                distance = d;
            }
        }

        return nearest;
    }




    public float GetDamage()
    {
        return this._damage;
    }

    public void SetDamage(float _dmg)
    {
        this._damage = _dmg;
    }

    public float GetMovementSpeed()
    {
        return this._movementSpeed;
    }

    public void SetMovementSpeed(float _ms)
    {
        this._movementSpeed = _ms;

    }

    public int GetNetWorth()
    {
        return this.NetWorth;
    }

    public void SetNetWorth(int _nw)
    {
        this.NetWorth = _nw;
    }

    public Shooter GetTarget(){
        return target;
    }


}
