using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Global helper class
/// </summary>
public partial class Global : Node
{

    /// <summary>
    /// Whether this peer is the mage (the server)
    /// </summary>
    public static bool Is_Mage = false;

    /// <summary>
    /// Whether this peer is a shooter (a client)
    /// </summary>
    public static bool Is_Shooter = false;

    /// <summary>
    /// A reference to the local shooter, e.g. used by the camera script
    /// </summary>
    public static Shooter LocalShooter = null;

    /// <summary>
    /// A reference to the NetworkManager for easy access
    /// </summary>
    public static NetworkManager NetworkManager;

    public static List<long> peers = new List<long>();
}
