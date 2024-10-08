using Godot;
using System;

/// <summary>
/// The healthbar above the entities heads
/// </summary>
public partial class HealthBar : ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetHealth(float value, float maxValue)
    {
        MaxValue = maxValue;
        Value = value;
    }
}
