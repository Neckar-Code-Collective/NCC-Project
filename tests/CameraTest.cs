using GdUnit4;
using Godot;
using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Tests;

[TestSuite]

public class CamerTests{


    private Shooter _shooter;

    [TestCase]
    public async Task TestCameraMovement(){
        var runner = ISceneRunner.Load("res://level.tscn");

        _shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate<Shooter>();
        _shooter.Name = "1";
        runner.Scene().AddChild(_shooter);

        Global.LocalShooter = _shooter;
        _shooter.GlobalPosition = new Vector3(10, 1, 1);
        await runner.SimulateFrames(5, 200);
        Assertions.AssertVec3(runner.Scene().GetViewport().GetCamera3D().GlobalPosition).IsEqualApprox(new Vector3(10, 7, 8),new Vector3(0.01f, 0.01f, 0.01f));
    }



}