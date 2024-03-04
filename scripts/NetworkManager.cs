using Godot;
using System;

public partial class NetworkManager : Node
{

    [Export]
    PackedScene ShooterPrefab;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
        Multiplayer.ServerDisconnected += ServerDisconnected;

        GD.PushWarning("Ich bin : " +Multiplayer.GetUniqueId());
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	#region Server Callbacks
	public void PeerConnected(long id){
		if(!Multiplayer.IsServer()){
            return;
        }

        GD.Print("New Client connected: " + id);

        var shooter = ShooterPrefab.Instantiate<Shooter>();
        shooter.Name = id.ToString();
        shooter.SetMultiplayerAuthority((int)id, true);
        GD.Print(shooter.GetMultiplayerAuthority());
        GetNode("../Shooters").AddChild(shooter);
    }

	public void PeerDisconnected(long id){
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
