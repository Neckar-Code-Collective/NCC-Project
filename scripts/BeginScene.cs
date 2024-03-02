using Godot;
using System;

public partial class BeginScene : Control
{
    Button ServerButton;
    Button ClientButton;

    TextEdit ClientAddress;
    TextEdit ClientPort;
    TextEdit ServerPort;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ServerButton = GetNode<Button>("ServerButton");
        ClientButton = GetNode<Button>("ClientButton");

        ClientAddress = GetNode<TextEdit>("TextEdit");
        ClientPort = GetNode<TextEdit>("TextEdit2");
        ServerPort = GetNode<TextEdit>("TextEdit3");

        ServerButton.Pressed += () =>
        {
            int port = int.Parse(ServerPort.Text);
            Global.Is_Shooter = false;
            Global.Is_Mage = true;

            var mult = new ENetMultiplayerPeer();
            var err = mult.CreateServer(port);
			if (err != Error.Ok){
                GD.PrintErr(err);
                GetTree().Quit();
                return;
            }

            Multiplayer.MultiplayerPeer = mult;

			err = GetTree().ChangeSceneToFile("res://level.tscn");
			if (err != Error.Ok){
                GD.PrintErr(err);
            }

        };

        ClientButton.Pressed += () =>
        {
            string address = ClientAddress.Text;
            int port = int.Parse(ClientPort.Text);

            Global.Is_Shooter = true;

            var mult = new ENetMultiplayerPeer();
            var err = mult.CreateClient(address, port);
			if(err != Error.Ok){
                GD.PrintErr(err);
                GetTree().Quit();
                return;
            }

            Multiplayer.MultiplayerPeer = mult;

            err = GetTree().ChangeSceneToFile("res://level.tscn");
			if (err != Error.Ok){
                GD.PrintErr(err);
            }

        };
    }
}
