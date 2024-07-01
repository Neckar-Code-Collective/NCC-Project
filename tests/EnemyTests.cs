using Godot;
using GdUnit4;
using System.Threading.Tasks;

namespace Tests;

[TestSuite]
public class EnemyTests
{

    [TestCase]
    public async Task TestTargeting()
    {
        //setup scene
        Global.Is_Mage = true;
        Assertions.AssertBool(true).IsTrue();
        var runner = ISceneRunner.Load("res://level.tscn");
        await runner.AwaitIdleFrame();
        
        //add shooter
        var shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate<Shooter>();
        shooter.Name = "1";
        runner.Scene().AddChild(shooter);
        shooter.GlobalPosition = new Vector3(15,0,10);
        
        await runner.AwaitIdleFrame();
        
        //setup mage and spawn mob
        var mage = runner.Scene().GetNode<Mage>("Mage");
        // var mageG = runner.Scene().GetTree().Root.FindChild("Mage");
        // var mage = mageG as Mage;
        mage.GetManaManager().SetCurrentMana(10);
        mage.SetSelectionState(SelectionState.BASIC_ENEMY);
        mage.SpawnMob(new Vector3(10, 0, 10));
        Assertions.AssertObject(mage).IsNotNull();

        //get enemy refernce and check that current target is null
        var enemy = runner.Scene().GetNode("Shooters").GetChild(0) as Enemy;
        Assertions.AssertObject(enemy).IsNotNull();
        Assertions.AssertObject(enemy.GetTarget()).IsNull();

        //verify that the enemy found our shooter and set it as its target
        await runner.SimulateFrames(5,100);
        Assertions.AssertObject(enemy.GetTarget()).IsNotNull();
        Assertions.AssertObject(enemy.GetTarget()).IsEqual(shooter);

        // runner.Scene().QueueFree();
    }

    [TestCase]
    public async Task TestDamage()
    {
        //setup scene
        Global.Is_Mage = true;
        Assertions.AssertBool(true).IsTrue();
        var runner = ISceneRunner.Load("res://level.tscn");
        await runner.AwaitIdleFrame();
        
        //add a shooter
        var shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate<Shooter>();
        shooter.Name = "1";
        runner.Scene().AddChild(shooter);
        shooter.GlobalPosition = new Vector3(10,0,10);
        
        await runner.AwaitIdleFrame();
        
        //setup mage and spawn mob
        var mage = runner.Scene().GetNode<Mage>("Mage");
        mage.GetManaManager().SetCurrentMana(10);
        mage.SetSelectionState(SelectionState.BASIC_ENEMY);
        mage.SpawnMob(new Vector3(10, 0, 10));
        Assertions.AssertObject(mage).IsNotNull();

        //get enemy refernce and check that current target is null
        var enemy = runner.Scene().GetNode("Shooters").GetChild(0) as Enemy;
        Assertions.AssertObject(enemy).IsNotNull();

        Assertions.AssertFloat(shooter.GetHealth().GetCurrentHealth()).IsEqual(10);

        //verify that the enemy found the shooter and damaged it
        await runner.SimulateFrames(5,100);
        Assertions.AssertObject(enemy.GetTarget()).IsEqual(shooter);
        Assertions.AssertFloat(shooter.GetHealth().GetCurrentHealth()).IsLess(9);



        // runner.Scene().QueueFree();
    }
}
