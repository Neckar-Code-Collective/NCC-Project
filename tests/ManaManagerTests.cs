using GdUnit4;
using GdUnit4.Asserts;

namespace Tests;

/// <summary>
/// All Tests related to the ManaManager
/// </summary>
[TestSuite]
public class ManaManagerTests{

	/// <summary>
	/// Tests the setters of ManaManager
	/// </summary>

	[TestCase]
	public void TestManaManagerSetter(){
		var manager = new ManaManager();
		manager.setCurrentMana(7);
		Assertions.AssertFloat(manager.getCurrentMana()).IsEqual(7);
		Assertions.AssertFloat(manager.getCurrentMana()).IsNotEqual(9);
		manager.Free();
	}


	/// <summary>
	/// Tests the mana regeneration logic
	/// </summary>
	[TestCase]
	public void TestManaRegenFromZero(){
		var manager = new ManaManager();
		manager.setCurrentMana(0);
		manager.Update(10); //simulate 10 seconds passing
		Assertions.AssertFloat(manager.getCurrentMana()).IsEqual(10);
		manager.Free();
	}

	/// <summary>
	/// Tests that the mana regenration caps of at maxMana
	/// </summary>
	[TestCase]
	public void TestManaUntilMaxRegen(){
		var manager = new ManaManager();
		manager.setCurrentMana(7);
		manager.Update(100);
		Assertions.AssertFloat(manager.getCurrentMana()).IsEqual(10);
		manager.Free();
	}

	
}
