using Godot;
using System;

public partial class Entity : CharacterBody3D
{
	
	protected HealthComponent health;
	NetworkedTransform netTrans;


	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{


        health = new HealthComponent();
        health.Name = "Health";
        health.SetMultiplayerAuthority(GetMultiplayerAuthority());
        AddChild(health);


        netTrans = new NetworkedTransform();
        netTrans.Name = "NetworkTransform";
        netTrans.SetMultiplayerAuthority(GetMultiplayerAuthority());
        netTrans.SetTarget(this);
        AddChild(netTrans);

		if (int.TryParse(Name, System.Globalization.NumberStyles.Any, null, out int id) )
        {
            // GD.Print("Setting Auhtority "+id);
            SetMultiplayerAuthority(id, true);
			
        }

        GD.Print("NETTRANS AUTH IS " + netTrans.GetMultiplayerAuthority());

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public HealthComponent getHealth(){
        return health;
    }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true)]
	public void RpcDealDamage(float amount){
        if (!IsMultiplayerAuthority()){
            return;
        }
        GD.Print("AUA!");
        health.applyDamage(amount);
    }

	[Rpc]
	public void RpcPlayAnimation(string anim,float speed){}

	[Rpc(MultiplayerApi.RpcMode.Authority,CallLocal = true)]
	public void RpcDie(){
        QueueFree();
    }
}
