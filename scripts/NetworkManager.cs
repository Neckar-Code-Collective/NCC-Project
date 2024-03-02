using Godot;
using System;

public partial class NetworkManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	#region Server Callbacks
	public void PeerConnected(int id){
		if(!Multiplayer.IsServer()){
            return;
        }

        GD.Print("New Client connected: " + id);
    }

	public void PeerDisconnected(int id){
		if(!Multiplayer.IsServer()){
            return;
        }

        GD.Print("Client "+id+" disconnected");
    }
	#endregion

	#region Client Callbacks

	public void ConnectedToServer(){
        GD.Print("Successfully connected");
    }

	public void ConnectionFailed(){
        GD.PushError("Connection to server lost!");
    }

	public void ServerDisconnected(){
        GD.Print("Disconnected from server");
    }

	#endregion
}
