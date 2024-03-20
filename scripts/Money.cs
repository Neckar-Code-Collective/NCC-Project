using Godot;
using System;

/// <summary>
/// Represents a money object in the world
/// </summary>
public partial class Money: RigidBody3D {

    /// <summary>
    /// The amount this money object adds to the player who picks it up
    /// </summary>
    int _moneyAmount = 0;

    /// <summary>
    /// Adds a random force to the money object so that it flies funny
    /// </summary>
    public override void _Ready()
	{
        Vector3 force = new Vector3();
        force.X = Random.Shared.NextSingle()*10;
        force.Y = 10;
        force.Z = Random.Shared.NextSingle()*10;
        ApplyForce(force);
        Vector3 torque = new Vector3();
        torque.X = Random.Shared.NextSingle();
        torque.Y = Random.Shared.NextSingle();
        torque.Z = Random.Shared.NextSingle();
        ApplyTorque(torque);
    }



    public void SetMoneyAmount(int _moneyAmount){
        this._moneyAmount = _moneyAmount;

    }

    public int GetMoneyAmount(){
        return _moneyAmount;

    }

    public override void _PhysicsProcess(double delta)
    {
        //GlobalPosition = GlobalPosition.Lerp(lerpTarget, lerpSpeed);
    }

    /// <summary>
    /// RPC. Sets the position of this money object on the clients. Only gets called on spawn.
    /// </summary>
    /// <param name="pos"></param>
    [Rpc(CallLocal = true)]
    public void RPCSetPosition(Vector3 pos){
        GlobalPosition = pos;
    }

    /// <summary>
    /// RPC. Sets the amount of money this money object grants to players who pick it up
    /// </summary>
    /// <param name="amount"></param>
    [Rpc(CallLocal = true)]
    public void RPCSetAmount(int amount){
        _moneyAmount = amount;
    }

    /// <summary>
    /// RPC. Gets called by shooters who pick this money object up. Removes it from the simulation
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true)]
    public void RPCRemove(){
        if( !IsMultiplayerAuthority()){
            return;
        }
        QueueFree();
    }











}