using Godot;
using System;

public partial class BloodSpawner : Node3D
{
    [Export]
    float emitInterval = 1;

    [Export]
    PackedScene BloodPrefab;

    [Export]
    float emitRange = 5;

    float spawnCounter = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Multiplayer.IsServer())
        {
            spawnCounter += (float)delta;
            if (spawnCounter >= emitInterval)
            {
                spawnCounter = 0;

                //Spawn Blood
                var blood = BloodPrefab.Instantiate<Blood>();
                blood.SetLerpSpeed(0.01f);
                blood.SetLerpTarget(getRandomPosInRange(emitRange));
                blood.SetAmount(Random.Shared.Next(0, 11));
                GetTree().Root.AddChild(blood);

            }
        }
    }

	private Vector3 getRandomPosInRange(float range){
        var r = Random.Shared;
        var x = GlobalPosition.X + ((r.Next(0, 2) < 1 ? -1 : 1) * r.NextSingle() * range);
		var z = GlobalPosition.Z + ((r.Next(0, 2) < 1 ? -1 : 1) * r.NextSingle() * range);
        return new Vector3(x, GlobalPosition.Y, z);
    }
}
