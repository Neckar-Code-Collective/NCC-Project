using Godot;
using System;
using System.Collections.Generic;

public partial class EntityManager : Node
{

	static Dictionary<int,Entity> entities = new Dictionary<int, Entity>();
	

	public static void registerEntity(int id,Entity e){
		entities.Add(id,e);
	}

	public static Entity GetEntity(int id){
		return entities[id];
	}

	public static void removeEntity(int id, Entity e){
		entities.Remove(id);
	}
}
