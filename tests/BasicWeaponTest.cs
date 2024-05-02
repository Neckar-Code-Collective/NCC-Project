using System.Threading.Tasks;
using GdUnit4;
using Godot;


[TestSuite]
public class BasicWeaponTest{

    [TestCase]
    public async Task TestBasicWeapon(){
        var runner = ISceneRunner.Load("res://weapon.tscn");

        var bw = runner.Scene() as BasicWeapon;

        
        for (int i = 0; i < 10;i++){
            bw.ShootInput(Vector3.Zero);
            await runner.SimulateFrames(1, 105);
        }

        Assertions.AssertInt(runner.Scene().FindChildren("*").Count).Equals(11);

        // var b = runner.Scene().FindChild("DebugBullet*") as Bullet;

        // Assertions.AssertFloat(b.GetDamage()).Equals(10);
        // Assertions.AssertBool(b.IsLocallySimulated()).IsTrue();

    }
}