using Godot;
using System;

/// <summary>
/// BasicEnemy has the overall pathfinding functions targettet at the shooter
/// Missing Functions: Health, Damage, OnCollision, MovementSpeed 
/// </summary>
public partial class Enemy : Entity{

	float damage;
    float movementSpeed = 3.0f;

	 [Export]
    PackedScene MoneyPrefab;

    int NetWorth = 5;




    /// <summary>
    /// nav_agent connected and creation of onready function
    /// aggroRange is the range at which the Enemy starts targeting a Shooter
    /// </summary>
    NavigationAgent3D nav_agent;

	Shooter target;

	public override void _Ready(){
		base._Ready();
		nav_agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		var aggroRange = GetNode<Area3D>("Area3D");
		aggroRange.BodyEntered += (Node3D body)=>{

			if(body is Shooter){
				target = (Shooter)body;
			}

		};

		health.onDeath += () =>{
			QueueFree();
 			//Spawn Money
            for (int i = 0; i < NetWorth; i++)
            {
				var money = MoneyPrefab.Instantiate<Money>();
                money.setMoneyAmount(1);
				GetTree().Root.AddChild(money);
                money.GlobalPosition = this.GlobalPosition;
            }

		};
	}


	

	/// <summary>
	/// This function lets the Enemy move towords the shooter
	/// </summary>
	/// <param name="delta"></param>

	public override void _PhysicsProcess(double delta){ //Update Funktion
		base._PhysicsProcess(delta);
		var current_location = GlobalTransform.Origin;
		var next_location = nav_agent.GetNextPathPosition();
		var new_velocity = (next_location - current_location).Normalized() * movementSpeed;

		Velocity = new_velocity;
		MoveAndSlide();

		if(target != null){
			nav_agent.TargetPosition = target.GlobalPosition;

		}

		Attack();
	}

	public void Attack(){
		var distance = target.GlobalPosition - this.GlobalPosition;
		if(distance.Length() < 1 ){
			target.Rpc(nameof(RpcDealDamage), 2);
			target.RpcDealDamage(2);
		} 

	}

   


	public float getDamage(){
		return this.damage;
	}

	public void setDamge(float _dmg){
		this.damage = _dmg;
	}

    public float getMovementSpeed(){
        return this.movementSpeed;
    }

    public void setMovementSpeed(float _ms){
        this.movementSpeed = _ms;

    }

	public int getNetWorth(){
        return this.NetWorth;
    }

	public void setNetWorth(int _nw){
        this.NetWorth = _nw;
    }

   

 
  


	


	
	
}
