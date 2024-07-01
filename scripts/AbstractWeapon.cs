using Godot;
using System;

/// <summary>
/// Parentclass for all weapons, manages basic operations like weapon
/// </summary>
public abstract partial class AbstractWeapon : Node3D
{
    [Export]
    string _name = "untitled";

    /// <summary>
    /// Gets called on other clients (not the player shooting) and spawns a dummy projectile with the specified properties
    /// </summary>
    /// <param name="pos">the position of the spawned bullet</param>
    /// <param name="vel">the velocity of the spawned bullet</param>
    /// <param name="data">additional data (e.g. different bullet type)</param>
    [Rpc]
    public virtual void RpcShoot(Vector3 pos, Vector3 vel, int data)
    {

    }

    public virtual void Reload()
    {

    }

    /// <summary>
    /// Gets called everytime the player has the shoot button pressed down (so also when shooting is not possible, handler shoot delay needs to be implemented by the respective weapon)
    /// </summary>
    /// <param name="velocity">the velocity the bullet should have</param>
    public virtual void ShootInput(Vector3 velocity)
    {

    }

    /// <summary>
    /// Gets called when the weapon gets unselected. In this function, all weapon parts should be disabled (e.g. the visuals). IMPORTANT!!! The function should not break when called multiple times
    /// </summary>
    public abstract void onDisable();

    /// <summary>
    /// Gets called when the weapon gets selected. In this function, all weapon parts should be enabled (e.g. the visuals).
    /// </summary>
    public abstract void onEnable();

    public string GetWeaponName(){
        return _name;
    }
}
