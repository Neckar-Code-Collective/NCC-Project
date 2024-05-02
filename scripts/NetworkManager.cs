using Godot;
using System;

/// <summary>
/// The NetworkManager handles player connections, disconnections and spawning of multiplayer objects
/// </summary>
public partial class NetworkManager : Node
{

    /// <summary>
    /// The node which holds all money objects
    /// </summary>
    [Export]
    Node3D _moneySpawnPath;

    /// <summary>
    /// Prefab for money
    /// </summary>
    [Export]
    PackedScene _moneyPrefab;

    [Export]
    PackedScene _bloodPrefab;

    /// <summary>
    /// Prefab for shooter
    /// </summary>
    [Export]
    PackedScene ShooterPrefab;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //set singleton
        Global.NetworkManager = this;

        //setup handlers
        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
        Multiplayer.ServerDisconnected += ServerDisconnected;

        GD.PushWarning("Ich bin : " + Multiplayer.GetUniqueId());

    }

    #region Server Callbacks
    /// <summary>
    /// Called when a new peer connects
    /// </summary>
    /// <param name="id">the id of the new peer</param>
    public void PeerConnected(long id)
    {
        //when we arent the server, we dont handle connections
        if (!Multiplayer.IsServer())
        {
            return;
        }

        GD.Print("New Client connected: " + id);

        //Instantiate player prefab
        var shooter = ShooterPrefab.Instantiate<Shooter>();
        shooter.Name = id.ToString();
        shooter.SetMultiplayerAuthority((int)id, true);
        GD.Print(shooter.GetMultiplayerAuthority());
        GetNode("../Shooters").AddChild(shooter);


        if (true)
        {
            SpawnWeapon("weapon_start_weapon", new Vector3(-5, 0.2f, 10));
            SpawnWeapon("weapon_skull", new Vector3(5, 0.2f, 10));
            first = false;
        }

    }

    bool first = true;

    /// <summary>
    /// Called when a peer disconnects
    /// </summary>
    /// <param name="id">id of the peer that disconnected</param>
    public void PeerDisconnected(long id)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        GD.Print("Client " + id + " disconnected");
    }
    #endregion

    #region Client Callbacks

    public void ConnectedToServer()
    {
        GD.Print("Successfully connected");
    }

    public void ConnectionFailed()
    {
        GD.PushError("Connection to server lost!");
    }

    public void ServerDisconnected()
    {
        GD.Print("Disconnected from server");
    }

    #endregion

    /// <summary>
    /// Called by enemies when they die. This spawns and replicates all money objects
    /// </summary>
    /// <param name="pos">where to spawn money</param>
    /// <param name="amount">how much money to spawn</param>
    public void SpawnMoney(Vector3 pos, int amount)
    {
        //we can only spawn money on the server
        if (!IsMultiplayerAuthority())
        {
            GD.PushError("Tried to spawn money on client, please call this function from server!");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            //Instantiate money node and add it to the simulation
            var money = _moneyPrefab.Instantiate<Money>();
            _moneySpawnPath.AddChild(money, true);
            money.Rpc(nameof(money.RPCSetPosition), pos);
            money.Rpc(nameof(money.RPCSetAmount), 1);
            money.SetMoneyAmount(1);
        }
    }

    public void SpawnWeapon(string name, Vector3 pos)
    {

        if (!IsMultiplayerAuthority())
        {
            GD.PushError("Tried to spawn world item on client, please call this function from server!");
            return;
        }

        var w = WeaponReg.GetWorldWeapon(name).Instantiate<WorldItem>();
        _moneySpawnPath.AddChild(w, true);
        w.Rpc(nameof(w.RpcSetPosition), pos);

    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RpcSpawnWeapon(string name, Vector3 pos)
    {
        if (!IsMultiplayerAuthority())
        {
            return;
        }
        SpawnWeapon(name, pos);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true)]
    public void RpcSpawnBlood(Vector3 pos,float amount){
        if(!IsMultiplayerAuthority()){
            return;
        }


        for (int i = 0; i < amount;i++){
            var b = _bloodPrefab.Instantiate<Blood>();
            GetTree().Root.AddChild(b,true);
            b.GlobalPosition = pos;
            b.SetLerpSpeed(0.1f);
            b.SetLerpTarget(pos + new Vector3(Random.Shared.NextSingle(), 0, Random.Shared.NextSingle()) * 4);
        }
    }

}
