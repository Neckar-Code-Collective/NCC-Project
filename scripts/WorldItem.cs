using Godot;
using System;

/// <summary>
/// Represents a dropped weapon
/// </summary>
public partial class WorldItem : Area3D
{
    /// <summary>
    /// What weapon this is
    /// </summary>
    [Export]
    string _name = "untitled";

    /// <summary>
    /// A reference to the Visuals, used to rotate the weapon in the world
    /// </summary>
    [Export]
    Node3D Visual;

    /// <summary>
    /// helper, stores where the weapon started in the world
    /// </summary>
    float _visualYStartPos;

    public override void _Ready()
    {

        _visualYStartPos = Visual.Position.Y;
    }

    /// <summary>
    /// Applies animation
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        
        if(Visual == null){
            GD.PushError("Visual not set");
            return;
        }

        Visual.RotateY(0.25f * (float)delta);


        Visual.Position = new Vector3(Visual.Position.X,Mathf.Sin(((float)Time.GetTicksMsec())/1000f)/2+_visualYStartPos,Visual.Position.Z);
    }

    public string GetWeaponName(){
        return _name;
    }

    /// <summary>
    /// Called by clients to remove this weapon, i.e. on pickup
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true)]
    public void RpcKill(){
        if(!IsMultiplayerAuthority()){
            return;
        }

        QueueFree();
    }

    /// <summary>
    /// Sets the position of this object on the other peers
    /// </summary>
    /// <param name="pos"></param>
    [Rpc(CallLocal = true)]
    public void RpcSetPosition(Vector3 pos){
        GlobalPosition = pos;
        Position = new Vector3(Position.X, pos.Y, Position.Z);
    }
}
