using Godot;
using Microsoft.CodeAnalysis;
using System;



public partial class BasicEnemy : Enemy{

	public override void _Ready()
	{
		base._Ready();
		_health.SetMaxHealth(100f);
		_health.SetCurrentHealth(_health.GetMaxHealth());
		SetMovementSpeed(1.0f);
		
	}

	



}
