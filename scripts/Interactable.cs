using Godot;
using System;

public partial class Interactable : Area3D
{
    [Signal]
    public delegate void OnInteractEventHandler(Shooter shooter);

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void RpcInteract(int peer){
		if(!IsMultiplayerAuthority()){
            return;
        }
        
        var s = EntityManager.GetShooterForPeerID(peer);

		if(s == null){
            GD.PushError("Shooter on interactable " + Name + " is null. This is bad!");
            return;
        }
        

        EmitSignal(SignalName.OnInteract,s);
		
    }

}
