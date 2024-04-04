using Godot;
using System;

public partial class WorldItem : Area3D
{
    [Export]
    string _name = "untitled";

    [Export]
    Node3D Visual;

    float _visualYStartPos;

    public override void _Ready()
    {

        _visualYStartPos = Visual.Position.Y;
    }

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

    [Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true)]
    public void RpcKill(){
        if(!IsMultiplayerAuthority()){
            return;
        }

        QueueFree();
    }

    [Rpc(CallLocal = true)]
    public void RpcSetPosition(Vector3 pos){
        GlobalPosition = pos;
        Position = new Vector3(Position.X, pos.Y, Position.Z);
    }
}
