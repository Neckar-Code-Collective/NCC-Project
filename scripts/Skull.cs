using Godot;
using System;
using System.Threading.Tasks.Sources;

/// <summary>
/// Represents a weapon that doesnt do anything
/// </summary>
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