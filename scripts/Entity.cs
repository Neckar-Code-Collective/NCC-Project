using Godot;
using System;

/// <summary>
/// The parent class of all entities (enemies and shooters)
/// </summary>
public partial class Entity : CharacterBody3D
{
	/// <summary>
    /// This entities health component
    /// </summary>
	protected HealthComponent _health;
    /// <summary>
    /// This entities networked transform;
    /// </summary>
	protected NetworkedTransform _netTrans;

    protected HealthBar _healthBar;


	
	// Setup references and create health component and networked transform
	public override void _Ready()
	{
        EntityManager.registerEntity(this);

        _health = new HealthComponent();
        _health.Name = "Health";
        _health.SetMultiplayerAuthority(GetMultiplayerAuthority());
        AddChild(_health);


        _netTrans = new NetworkedTransform();
        _netTrans.Name = "NetworkTransform";
        _netTrans.SetMultiplayerAuthority(GetMultiplayerAuthority());
        _netTrans.SetTarget(this);
        AddChild(_netTrans);

		if (int.TryParse(Name, System.Globalization.NumberStyles.Any, null, out int id) )
        {
            // GD.Print("Setting Auhtority "+id);
            SetMultiplayerAuthority(id, true);
			
        }

        GD.Print("NETTRANS AUTH IS " + _netTrans.GetMultiplayerAuthority());

    }

	public HealthComponent GetHealth(){
        return _health;
    }

    /// <summary>
    /// RPC. Gets called by other peers when they damage the entity this peer owns
    /// </summary>
    /// <param name="amount"></param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true)]
	public void RpcDealDamage(float amount){
        if (!IsMultiplayerAuthority()){
            return;
        }
        GD.Print("Im getting damaged from peer ",Multiplayer.GetRemoteSenderId());
        _health.ApplyDamage(amount);
    }

    /// <summary>
    /// Gets called by the owning peer to destroy an entity. doesnt actually have to be a rpc 😅
    /// </summary>
	[Rpc(MultiplayerApi.RpcMode.Authority,CallLocal = true)]
	public void RpcDie(){
        if (!IsMultiplayerAuthority()){
            return;
        }
        EntityManager.removeEntity(this);
        _health.QueueFree();
        _netTrans.QueueFree();
        QueueFree();
    }
}
