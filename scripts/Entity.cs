using Godot;
using System;

public partial class Entity : CharacterBody3D
{
	
	protected HealthComponent health;
	//TODO NetworkedTransform netTrans;


	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        health = new HealthComponent();
        AddChild(health);


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public HealthComponent getHealth(){
        return health;
    }

	[Rpc]
	public void RpcDealDamage(float amount){
        GD.Print("AUA!");
        health.applyDamage(amount);
    }

	[Rpc]
	public void RpcPlayAnimation(string anim,float speed){}

	[Rpc]
	public void RpcDie(){}
}
