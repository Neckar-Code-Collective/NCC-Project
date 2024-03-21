using System.Runtime.CompilerServices;
using GdUnit4;
using GdUnit4.Asserts;
using Godot;

namespace Tests;

[TestSuite]
public class HealthComponentTests {

	[TestCase]
	public void TestDieTests(){
		Assertions.AssertBool(true).IsEqual(true);
	}

//TestCase CheckIfDead()

	[TestCase]
	public void TestCheckIfDeadNegative(){
		HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);

		healthComponent.SetCurrentHealth(-5);
		Assertions.AssertBool(healthComponent.IsDead()).IsTrue();
		healthComponent.Free();
	}

	[TestCase]
	public void TestCheckIfDeadZeroHealth(){
		HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);

		healthComponent.SetCurrentHealth(0);
		Assertions.AssertBool(healthComponent.IsDead()).IsTrue();
		healthComponent.Free();
	}

	[TestCase]
	public void TestCheckIfDeadAlive(){
		HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);

		healthComponent.SetCurrentHealth(1);
		Assertions.AssertBool(healthComponent.IsDead()).IsFalse();
		healthComponent.Free();
	}

//TestCase applyDamage

	[TestCase]
	public void TestApplyDamageNormal(){
		HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);
		float maxH = healthComponent.GetMaxHealth(); // unnötig kompliziert aber ich wollte mal irgendwas mit maxHealth machen
		healthComponent.SetCurrentHealth(maxH);

		healthComponent.ApplyDamage(50);
		Assertions.AssertFloat(healthComponent.GetCurrentHealth()).IsEqual(50);
		healthComponent.Free();

	}

	
	[TestCase]
	public void TestApplyDamageNegative(){
		HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);
		float maxH = healthComponent.GetMaxHealth(); // unnötig kompliziert aber ich wollte mal irgendwas mit maxHealth machen
		healthComponent.SetCurrentHealth(maxH);

		healthComponent.ApplyDamage(-50);
		Assertions.AssertFloat(healthComponent.GetCurrentHealth()).IsEqual(100);
		healthComponent.Free();

	}

//TestCase heal

	[TestCase]
	public void TestHealNormal(){

		 HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);
		healthComponent.SetCurrentHealth(1);

		healthComponent.Heal(50);
		Assertions.AssertFloat(healthComponent.GetCurrentHealth()).IsEqual(51);
		healthComponent.Free();

	}

	[TestCase]
	 public void TestHealNegative(){

		 HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);
		healthComponent.SetCurrentHealth(1);

		healthComponent.Heal(-50);
		Assertions.AssertFloat(healthComponent.GetCurrentHealth()).IsEqual(1);
		healthComponent.Free();

	}

	 [TestCase]
	 public void TestHealToMuch(){

		HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);
		healthComponent.SetCurrentHealth(1);

		healthComponent.Heal(100);
		Assertions.AssertFloat(healthComponent.GetCurrentHealth()).IsEqual(100);
		healthComponent.Free();

	}

//TestCase DIE!!!! and _PhysicsProcess

	[TestCase]

	public void TestDie(){
		
		HealthComponent healthComponent = new HealthComponent();
		healthComponent.SetMaxHealth(100);
		healthComponent.SetCurrentHealth(-1);

		bool hasFired = false;
		healthComponent.onDeath += () => hasFired = true;
		healthComponent._PhysicsProcess(2);
		Assertions.AssertBool(hasFired).IsTrue();
		healthComponent.Free();


	}


}
