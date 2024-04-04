using Godot;
using System;

public partial class PrintInteractable : Interactable
{
    [Export]
    string msg = "hi";
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        OnInteract += (Shooter s) =>
        {
            GD.Print(msg);
        };
    }

}
