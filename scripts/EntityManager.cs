using Godot;
using System;
using System.Collections.Generic;

public partial class EntityManager : Node
{

    static List<Entity> entities = new List<Entity>();
    static List<Shooter> shooters = new List<Shooter>();

    public static void registerEntity(Entity e){
		entities.Add(e);
		if (e is Shooter s){
            shooters.Add(s);
        }
	}

	public static void removeEntity(Entity e){
        entities.Remove(e);

		if (e is Shooter s){
            shooters.Remove(s);
        }


    }
}
