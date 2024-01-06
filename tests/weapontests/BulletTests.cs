using Godot;
using System;
using GdUnit4;
using System.Threading.Tasks;

[TestSuite]
public partial class BulletTests
{
    

	[TestCase]
	public async Task TestBulletTravel(){
        var runner = ISceneRunner.Load("res://tests/weapontests/EmptyScene.tscn");
        var bullet = new Bullet();
        runner.Scene().AddChild(bullet);
        bullet.Setup(Vector3.Zero, Vector3.Right, 10);
        await runner.SimulateFrames(5, 200);
        Assertions.AssertVec3(bullet.GlobalPosition).IsGreaterEqual(Vector3.Right);
        
    }

	[TestCase]
	public async Task TestBulletLifeTime(){
        var runner = ISceneRunner.Load("res://tests/weapontests/EmptyScene.tscn");
        var bullet = new Bullet();
        runner.Scene().AddChild(bullet);
        bullet.Setup(Vector3.Zero, Vector3.Right, 10);
        await runner.SimulateFrames(5, 200);
        Assertions.AssertFloat(bullet.GetLifetime()).IsGreaterEqual(1);

    }

	[After]
	public void Clear(){
        
    }
	
}
