using Godot;
using System;

/// <summary>
/// Represents a Shape in which a player can stand and interact with
/// </summary>
public partial class Interactable : Area3D
{
    [Signal]
    public delegate void OnInteractEventHandler(Shooter shooter);
    [Signal]
    public delegate void OnInteractMageEventHandler();

    /// <summary>
    /// Called by clients when they want to interact with this
    /// </summary>
    /// <param name="peer"></param>
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
