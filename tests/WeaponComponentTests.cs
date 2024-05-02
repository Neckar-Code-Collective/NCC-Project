using Godot;
using GdUnit4;

[TestSuite]
public class WeaponComponentTests{

    [TestCase]
    public void TestWeaponPickUp(){
        var runner = ISceneRunner.Load("res://level.tscn");

        var shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate<Shooter>();
        runner.Scene().FindChild("Shooters").AddChild(shooter);
        var wc = shooter.FindChild("WeaponComponent") as WeaponComponent;

        Assertions.AssertBool(wc.HasSpace()).IsTrue();

        wc.EquipWeapon("weapon_start_weapon");
        wc.EquipWeapon("weapon_start_weapon");
        wc.EquipWeapon("weapon_start_weapon");

        Assertions.AssertBool(wc.HasSpace()).IsFalse();

        runner.Scene().QueueFree();

    }

    [TestCase]
    public void TestWeaponDrop(){
        var runner = ISceneRunner.Load("res://level.tscn");

        var shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate<Shooter>();
        runner.Scene().FindChild("Shooters").AddChild(shooter);
        var wc = shooter.FindChild("WeaponComponent") as WeaponComponent;

        Assertions.AssertBool(wc.HasSpace()).IsTrue();

        wc.EquipWeapon("weapon_start_weapon");
        wc.EquipWeapon("weapon_start_weapon");
        wc.EquipWeapon("weapon_start_weapon");

        Assertions.AssertBool(wc.HasSpace());

        wc.DropCurrentWeapon();

        Assertions.AssertBool(wc.HasSpace());



    }
}