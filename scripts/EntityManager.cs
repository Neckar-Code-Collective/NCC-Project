using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Keeps references to all entities currently in the scene
/// </summary>
public partial class EntityManager : Node
{

    /// <summary>
    /// All entities overall in the scene
    /// </summary>
    static List<Entity> entities = new List<Entity>();

    /// <summary>
    /// All shooters currently in the scene
    /// </summary>
    static List<Shooter> shooters = new List<Shooter>();

    /// <summary>
    /// Gets called by entities upon creation
    /// </summary>
    /// <param name="e"></param>
    public static void registerEntity(Entity e){
		entities.Add(e);
		if (e is Shooter s){
            shooters.Add(s);
        }
	}

    /// <summary>
    /// Gets called by entities during their destruction
    /// </summary>
    /// <param name="e">The entity to remove</param>
	public static void removeEntity(Entity e){
        entities.Remove(e);

		if (e is Shooter s){
            shooters.Remove(s);
        }


    }
}
