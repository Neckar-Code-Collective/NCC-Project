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

    /// <summary>
    /// The health bar
    /// </summary>
    protected HealthBar _healthBar;

    /// <summary>
    /// The active camera
    /// </summary>
    protected Camera3D _camera;



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

        if (int.TryParse(Name, System.Globalization.NumberStyles.Any, null, out int id))
        {
            // GD.Print("Setting Auhtority "+id);
            SetMultiplayerAuthority(id, true);

        }

        GD.Print("NETTRANS AUTH IS " + _netTrans.GetMultiplayerAuthority());

        _camera = GetViewport().GetCamera3D() as Camera3D;



        var healthBarScene = (PackedScene)GD.Load("res://HealthBar.tscn");
        _healthBar = healthBarScene.Instantiate() as HealthBar;
        GetTree().Root.GetNode<CanvasLayer>("Level/CanvasLayer2").AddChild(_healthBar);
        _healthBar.SetHealth(_health.GetCurrentHealth(), _health.GetMaxHealth());

    }

    public HealthComponent GetHealth()
    {
        return _health;
    }

    /// <summary>
    /// RPC. Gets called by other peers when they damage the entity this peer owns
    /// </summary>
    /// <param name="amount"></param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public virtual void RpcDealDamage(float amount)
    {
        GD.Print("Im getting damaged from peer ", Multiplayer.GetRemoteSenderId());
        if(this is Shooter shooter && shooter.GetShooterState() == Shooter.ShooterState.INJURED && Multiplayer.GetRemoteSenderId() != GetMultiplayerAuthority()){
            return;
        }
        _health.ApplyDamage(amount);
        // Rpc(nameof(RpcUpdateHealthBar));

    }

    /// <summary>
    /// Gets called by the owning peer to destroy an entity. 
    /// </summary>
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void RpcDie()
    {
        EntityManager.removeEntity(this);
        _healthBar.QueueFree();
        if (IsMultiplayerAuthority())
        {
            _health.QueueFree();
            _netTrans.QueueFree();
            QueueFree();

        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void RpcUpdateHealthBar(float h,float maxh)
    {
        _healthBar.SetHealth(h, maxh);
        var screenPosition = _camera.UnprojectPosition(GlobalTransform.Origin);
        _healthBar.Position = screenPosition + new Vector2(-_healthBar.Size.X / 2, -100);

    }
}
