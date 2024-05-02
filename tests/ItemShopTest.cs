using System.Threading.Tasks;
using GdUnit4;
using Godot;

[TestSuite]
public class ItemShopTest{

    [TestCase]
    public async Task TestItemShopNotEnoughMoney(){
        var runner = ISceneRunner.Load("res://level.tscn");

        var shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate() as Shooter;
        shooter.Name = "1";

        runner.Scene().FindChild("Shooters").AddChild(shooter);

        await runner.AwaitIdleFrame();

        var ws = runner.Scene().FindChild("WeaponShop") as WeaponShop;

        EntityManager.registerEntity(shooter);

        ws.RpcInteract(1);

        Assertions.AssertInt(runner.Scene().FindChild("MoneyHolder").GetChildCount()).Equals(0);

        
    }

    [TestCase]
    public async Task TestItemShopEnoughMoney(){

        var runner = ISceneRunner.Load("res://level.tscn");

        var shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate() as Shooter;
        shooter.Name = "1";

        runner.Scene().FindChild("Shooters").AddChild(shooter);

        await runner.AwaitIdleFrame();

        var ws = runner.Scene().FindChild("WeaponShop") as WeaponShop;

        EntityManager.registerEntity(shooter);

        shooter.SetMoney(100);

        ws.RpcInteract(1);

        Assertions.AssertInt(runner.Scene().FindChild("MoneyHolder").GetChildCount()).Equals(1);
    }
}