using Godot;
using Microsoft.CodeAnalysis;
using System;



public partial class BasicEnemy : Enemy{

	public override void _Ready()
	{
		base._Ready();
		health.setMaxHealth(100f);
		health.setCurrentHealth(health.getMaxHealth());
		setMovementSpeed(1.0f);
		
	}

	



}
