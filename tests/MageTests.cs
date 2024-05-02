using Godot;
using GdUnit4;
using System.Threading.Tasks;

[TestSuite]
public class MageTests{

    
    [TestCase]
    public async Task TestSpawnBlockedByTerrain(){
        Global.Is_Mage = true;
        var runner = ISceneRunner.Load("res://level.tscn");
        var m = runner.Scene().FindChild("Mage", true);
        Assertions.AssertBool(m is Mage).IsTrue();

        var mage = m as Mage;

        mage.GetManaManager().SetCurrentMana(2);
        await runner.AwaitIdleFrame();

        Assertions.AssertBool(mage.CheckSpawn(new Vector3(0, 0, 0))).IsFalse();

    }

    [TestCase]
    public async Task TestSpawnBlockedByOtherMob(){
        Global.Is_Mage = true;
        var runner = ISceneRunner.Load("res://level.tscn");
        var m = runner.Scene().FindChild("Mage", true);
        Assertions.AssertBool(m is Mage).IsTrue();

        var mage = m as Mage;

        var enemy = GD.Load<PackedScene>("res://BasicEnemy.tscn").Instantiate<Enemy>();
        mage.GetParent().FindChild("Shooters").AddChild(enemy);
        enemy.GlobalPosition = new Vector3(1000, 0, 1000);

        await runner.AwaitIdleFrame();

        mage.GetManaManager().SetCurrentMana(2);

        Assertions.AssertVec3(enemy.GlobalPosition).IsEqual(new Vector3(1000, 0, 1000));

        Assertions.AssertBool(mage.CheckSpawn(new Vector3(1000, 0, 1000))).IsFalse();
    }


    [TestCase]
    public void TestMobSpawn(){
        var runner = ISceneRunner.Load("res://level.tscn");
        var m = runner.Scene().FindChild("Mage", true);
        Assertions.AssertBool(m is Mage).IsTrue();

        var mage = m as Mage;

        Assertions.AssertObject(runner.Scene().FindChild("Shooters").FindChild("BasicEnemy*")).IsNull();

        mage.GetManaManager().SetCurrentMana(10);
        mage.SetSelectionState(SelectionState.BASIC_ENEMY);
        mage.SpawnMob(new Vector3(0, 0, 0));

        Assertions.AssertObject(runner.Scene().FindChild("Shooters").GetChild(0)).IsNotNull();

    }
}