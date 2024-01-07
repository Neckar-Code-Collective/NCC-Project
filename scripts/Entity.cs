using Godot;
using System;

public partial class Entity : Area3D
{
	
	 HealthComponent health;
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

	[Rpc]
	public void RpcDealDamage(float amount){
        GD.Print("AUA!");
    }

	[Rpc]
	public void RpcPlayAnimation(string anim,float speed){}

	[Rpc]
	public void RpcDie(){}
}
