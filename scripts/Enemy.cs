using Godot;
using System;

/// <summary>
/// BasicEnemy has the overall pathfinding functions targettet at the shooter
/// </summary>
public partial class Enemy : Entity{

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

	/// <summary>
    /// Setup references and death handler
    /// </summary>
	public override void _Ready(){
		base._Ready();
		nav_agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		var aggroRange = GetNode<Area3D>("Area3D");
		aggroRange.BodyEntered += (Node3D body)=>{

			if(body is Shooter){
				target = (Shooter)body;
			}

		};

		//basic logic for dying
		_health.onDeath += () =>{
            Global.NetworkManager.SpawnMoney(GlobalPosition, NetWorth);
            Rpc(nameof(RpcDie));
		};
	}


	

	/// <summary>
	/// This function lets the Enemy move towords the shooter
	/// </summary>
	/// <param name="delta"></param>

	public override void _PhysicsProcess(double delta){ //Update Funktion
		base._PhysicsProcess(delta);

		//if we arent the server, we dont want to update the enemies
		if(!IsMultiplayerAuthority()){
            return;
        }

		//finds the next location to go to and moves there
		var current_location = GlobalTransform.Origin;
		var next_location = nav_agent.GetNextPathPosition();
		var new_velocity = (next_location - current_location).Normalized() * _movementSpeed;

		Velocity = new_velocity;
		MoveAndSlide();

		//if we dont have a target we dont want to move at all
		if(target != null){

			if(!IsInstanceValid(target)){
                target = null;
                return;
            }

			nav_agent.TargetPosition = target.GlobalPosition;

			LookAt(target.GlobalPosition, Vector3.Up);
			Attack();
		}

	}

	/// <summary>
    /// Check if we are in range of the shooter to attack him
    /// </summary>
	public void Attack(){
		var distance = target.GlobalPosition - this.GlobalPosition;
		if(distance.Length() < 1 ){
			target.Rpc(nameof(RpcDealDamage), 2);
			target.RpcDealDamage(2);
		} 

	}

   


	public float GetDamage(){
		return this._damage;
	}

	public void SetDamage(float _dmg){
		this._damage = _dmg;
	}

    public float GetMovementSpeed(){
        return this._movementSpeed;
    }

    public void SetMovementSpeed(float _ms){
        this._movementSpeed = _ms;

    }

	public int GetNetWorth(){
        return this.NetWorth;
    }

	public void SetNetWorth(int _nw){
        this.NetWorth = _nw;
    }
	
}
