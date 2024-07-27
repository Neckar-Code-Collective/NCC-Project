using Godot;
using System;
using System.Threading;

public partial class LobbyScene : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(Global.Is_Mage){
            
			GetNode<Control>("ClientText").Visible = false;
            GetNode<Button>("ServerButton").Pressed += () => {
                Rpc(nameof(RPCStart));
                GetTree().CreateTimer(0.5).Timeout += () =>{
                	RPCStart();

				};
            };

            Multiplayer.PeerConnected += (id) =>
            {
                Global.peers.Add(id);
            };
            Multiplayer.PeerDisconnected += (id) => {
                GD.Print("BEODBEODBEODBEODBEDOEBDOEBDOBEODBEOBDOEBDOBEODBEOBDO DISOCNNNECT");
            };
        }
		else{
            GetNode<Control>("ServerButton").Visible = false;
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	void RPCStart(){
        GetTree().Root.AddChild(GD.Load<PackedScene>("res://level.tscn").Instantiate());
        // GetTree().ChangeSceneToFile("res://level.tscn");
        GetTree().CreateTimer(0.5).Timeout += () => {
            QueueFree();
        };
    }
}
