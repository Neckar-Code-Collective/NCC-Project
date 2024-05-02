using Godot;
using System;

/// <summary>
/// A bullet that gets larger the further it travels
/// </summary>
public partial class PopcornBullet: Bullet
{

    /// <summary>
    /// Applies scale to bullet
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        velocity *= 0.94f;
        GlobalScale(new Vector3(1.05f, 1.05f, 1.05f));
    }

    public override void _Ready()
    {
        base._Ready();
        maxLifeTime = 1f;
    }
}