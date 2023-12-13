using GdUnit4;
using GdUnit4.Asserts;

namespace Tests;

[TestSuite]
public class ManaManagerTests{

    [TestCase]
    public void TestManaManagerSetter(){
        var manager = new ManaManager();
        manager.setCurrentMana(7);
        Assertions.AssertFloat(manager.getCurrentMana()).IsEqual(7);
        Assertions.AssertFloat(manager.getCurrentMana()).IsNotEqual(9);
        manager.Free();
    }

    [TestCase]
    public void TestManaRegenFromZero(){
        var manager = new ManaManager();
        manager.setCurrentMana(0);
        manager.Update(10); //simulate 10 seconds passing
        Assertions.AssertFloat(manager.getCurrentMana()).IsEqual(10);
        manager.Free();
    }

    [TestCase]
    public void TestManaUntilMaxRegen(){
        var manager = new ManaManager();
        manager.setCurrentMana(7);
        manager.Update(100);
        Assertions.AssertFloat(manager.getCurrentMana()).IsEqual(10);
        manager.Free();
    }

    
}