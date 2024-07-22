using Godot;
using GdUnit4;
using System.Threading.Tasks;

namespace Tests;

[TestSuite]
public class NetworkedTransformTest {
    
    [TestCase]
    public async Task TestPositionCorrection(){
        var runner = ISceneRunner.Load("res://level.tscn");

        var shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate<Shooter>();

        shooter.Name = "1234";
        runner.Scene().AddChild(shooter);
        shooter.GlobalPosition = new Vector3(10000,0,10000);

        var nt = shooter.GetNode<NetworkedTransform>("NetworkTransform");
        nt.RPCUpdatePosition(new Vector3(10005, 0, 10000),new Vector3());

        Assertions.AssertVec3(shooter.GlobalPosition).IsEqual(new Vector3(10000, 0, 10000));

        await runner.SimulateFrames(10, 100);

        Assertions.AssertVec3(shooter.GlobalPosition).IsEqualApprox(new Vector3(10005, 0, 10005),new Vector3(0.1f,0.1f,0.1f));

    }
}