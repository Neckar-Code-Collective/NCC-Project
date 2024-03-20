using Godot;
using System;

public partial class Blood : Area3D
{
    /// <summary>
    /// The amount of blood point this blood unit gives to the mage
    /// </summary>
    int _amount = 0;

    /// <summary>
    /// The location this blood orb is traveling
    /// </summary>
    Vector3 _lerpTarget = Vector3.Zero;

    /// <summary>
    /// The speed with which this blood orb is traveling to its location
    /// </summary>
    float _lerpSpeed = 0.2f;

    /// <summary>
    /// Sets the location this orb is traveling to
    /// </summary>
    /// <param name="target">the target position</param>
	public void SetLerpTarget(Vector3 target)
    {
        _lerpTarget = target;
    }

    public void SetLerpSpeed(float s)
    {
        _lerpSpeed = s;
    }

    public void SetAmount(int a)
    {
        _amount = a;
    }

    public int GetAmount()
    {
        return _amount;
    }

    /// <summary>
    /// Moves this orb to its _lerpTarget location
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = GlobalPosition.Lerp(_lerpTarget, _lerpSpeed);
    }
}
