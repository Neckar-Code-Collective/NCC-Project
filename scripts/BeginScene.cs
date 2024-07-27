using Godot;
using System;
using System.Net;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

/// <summary>
/// Just handles the starting of the game and the server/client
/// </summary>
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

    RichTextLabel _ipAddressLabel;

    Button _copyipButton;


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

        _ipAddressLabel = GetNode<RichTextLabel>("iplabel");
        _copyipButton = GetNode<Button>("ipbutton");

        string ipAdress = GetLocalIPAddress();
        _ipAddressLabel.Text = $"Your IP: {ipAdress}";


        _copyipButton.Text = "Copy IP Address to Clipboard";   
        _copyipButton.Pressed += OnCopyIPPressed;

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
            err = GetTree().ChangeSceneToFile("res://LobbyScene.tscn");
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
            err = GetTree().ChangeSceneToFile("res://LobbyScene.tscn");
            if (err != Error.Ok)
            {
                GD.PrintErr(err);
            }

        };
    }

    private string GetLocalIPAddress(){
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach(var ip in host.AddressList){
            if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork){
                return ip.ToString();
            }
        }
        throw new Exception("Local IP Adress Not Found!");
    }

    private void OnCopyIPPressed(){
        Clipboard.SetText(_ipAddressLabel.Text.Replace("Your IP: ", ""));
        GD.Print("IP address copied to clipboard");
    }
}
