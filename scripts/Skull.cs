using Godot;
using System;
using System.Threading.Tasks.Sources;

public partial class Skull : AbstractWeapon
{
    [Export]
    Node3D _visual;
    public override void onDisable()
    {
        _visual.Visible = false;
    }

    public override void onEnable()
    {
        _visual.Visible = true;
    }
}