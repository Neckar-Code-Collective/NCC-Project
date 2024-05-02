using Godot;
using Microsoft.CodeAnalysis;
using System;


/// <summary>
/// The most basic enemy the game has
/// </summary>
public partial class BasicEnemy : Enemy{

	/// <summary>
    /// sets up starting data
    /// </summary>
	public override void _Ready()
	{
		base._Ready();
		_health.SetMaxHealth(100f);
		_health.SetCurrentHealth(_health.GetMaxHealth());
		SetMovementSpeed(1.0f);
		
	}

	



}
