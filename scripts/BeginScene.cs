using Godot;
using System;

public partial class BeginScene : Control
{
    /// <summary>
    /// reference to the "Start Server" button
    /// </summary>
    Button _serverButton;

    /// <summary>
    /// reference to the "Connect as Client" button
    /// </summary>
    Button _clientButton;

    /// <summary>
    /// The address the user entered to connect to as a client
    /// </summary>
    TextEdit _clientAddress;

    /// <summary>
    /// the port the user entered to connect to as a client
    /// </summary>
    TextEdit _clientPort;

    /// <summary>
    /// the port the user entered to host the server on
    /// </summary>
    TextEdit _serverPort;


    /// <summary>
    /// Sets up the reference and handlers for the buttons in the main menu
    /// </summary>
    public override void _Ready()
    {
        _serverButton = GetNode<Button>("ServerButton");
        _clientButton = GetNode<Button>("ClientButton");

        _clientAddress = GetNode<TextEdit>("TextEdit");
        _clientPort = GetNode<TextEdit>("TextEdit2");
        _serverPort = GetNode<TextEdit>("TextEdit3");

        _serverButton.Pressed += () =>
        {
            //the user pressed the server button
            GD.PushWarning("SERVER");

            //parse input and set global configuration
            int port = int.Parse(_serverPort.Text);
            Global.Is_Shooter = false;
            Global.Is_Mage = true;

            //startup the multiplayer engine
            var mult = new ENetMultiplayerPeer();
            var err = mult.CreateServer(port);
            if (err != Error.Ok)
            {
                GD.PrintErr(err);
                GetTree().Quit();
                return;
            }

            Multiplayer.MultiplayerPeer = mult;

            //switch to the game scene
            err = GetTree().ChangeSceneToFile("res://level.tscn");
            if (err != Error.Ok)
            {
                GD.PrintErr(err);
            }

        };

        _clientButton.Pressed += () =>
        {
            //the user pressed the client button
            GD.PushWarning("CLIENT");

            //parse input for connection
            string address = _clientAddress.Text;
            int port = int.Parse(_clientPort.Text);

            //set global vars
            Global.Is_Shooter = true;


            //start multiplayer engine
            var mult = new ENetMultiplayerPeer();
            var err = mult.CreateClient(address, port);
            if (err != Error.Ok)
            {
                GD.PrintErr(err);
                GetTree().Quit();
                return;
            }

            Multiplayer.MultiplayerPeer = mult;

            //switch to game scene
            err = GetTree().ChangeSceneToFile("res://level.tscn");
            if (err != Error.Ok)
            {
                GD.PrintErr(err);
            }

        };
    }
}
