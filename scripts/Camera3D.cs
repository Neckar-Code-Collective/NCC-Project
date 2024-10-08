using Godot;
using System;
using System.Reflection.Metadata;

/// <summary>
/// Helperscript for the camera to only focus the local player
/// </summary>
public partial class Camera3D : Godot.Camera3D
{
    private float _zoom = 7;

    /// <summary>
    /// Updates the cameras position
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
   {

        if (Global.LocalShooter != null)
        {
            GlobalPosition = GlobalPosition.Lerp(Global.LocalShooter.GlobalPosition + new Vector3(0, _zoom, _zoom), 0.5f);
        }
   }

    /// <summary>
    /// Listens to input and applies zoom
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton e)
        {
            if(!Input.IsPhysicalKeyPressed(Key.Ctrl)){
                return;
            }

            if(e.ButtonIndex == MouseButton.WheelUp)
            {
                _zoom -= 0.5f;
            }
            if(e.ButtonIndex == MouseButton.WheelDown)
            {
                _zoom += 0.5f;
            }

        }
        _zoom = Mathf.Clamp(_zoom, 4, 8);
    }

}
