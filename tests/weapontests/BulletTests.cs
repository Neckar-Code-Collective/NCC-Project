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

    [TestCase]
    public async Task TestCollisionWithWall(){
        var runner = ISceneRunner.Load("res://tests/weapontests/bulletcolls/BulletWallCollTest.tscn");
        var bullet = GD.Load<PackedScene>("res://DebugBullet.tscn").Instantiate<Bullet>();
        runner.Scene().AddChild(bullet);
        bullet.Setup(Vector3.Zero, Vector3.Right, 10);

        await runner.SimulateFrames(25, 100);
        Assertions.AssertBool(GodotObject.IsInstanceValid(bullet)).IsFalse();
    }


    [TestCase]
    public async Task TestCollisionWithEnemy(){
        var runner = ISceneRunner.Load("res://tests/weapontests/EmptyScene.tscn");
        var bullet = GD.Load<PackedScene>("res://DebugBullet.tscn").Instantiate<Bullet>();
        var enemy = GD.Load<PackedScene>("res://BasicEnemy.tscn").Instantiate<BasicEnemy>();

        runner.Scene().AddChild(bullet);
        bullet.Setup(Vector3.Zero, Vector3.Right, 10);

        bool gotCalled = false;
        runner.Scene().AddChild(enemy);
        enemy.GlobalPosition = new Vector3(5, 0, 0);
        enemy.getHealth().onDamage += () =>
        {
            gotCalled = true;
        };



        await runner.SimulateFrames(25, 100);
        Assertions.AssertFloat(enemy.getHealth().getCurrentHealth()).IsEqual(90);
        Assertions.AssertBool(GodotObject.IsInstanceValid(bullet)).IsFalse();
        Assertions.AssertBool(gotCalled).IsTrue();
    }


    [TestCase]
    public async Task TestBulletGetsDeletedAfterTime(){
        var runner = ISceneRunner.Load("res://tests/weapontests/EmptyScene.tscn");
        var bullet = new Bullet();
        runner.Scene().AddChild(bullet);
        bullet.Setup(Vector3.Zero, Vector3.Right, 10);
        await runner.SimulateFrames(11, 1000);
        Assertions.AssertBool(Godot.GodotObject.IsInstanceValid(bullet)).IsFalse();
    }

	
}
