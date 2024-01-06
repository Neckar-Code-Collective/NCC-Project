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
        healthComponent.setMaxHealth(100);

        healthComponent.setCurrentHealth(-5);
        Assertions.AssertBool(healthComponent.isDead()).IsTrue();
        healthComponent.Free();
    }

    [TestCase]
    public void TestCheckIfDeadZeroHealth(){
        HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);

        healthComponent.setCurrentHealth(0);
        Assertions.AssertBool(healthComponent.isDead()).IsTrue();
        healthComponent.Free();
    }

    [TestCase]
    public void TestCheckIfDeadAlive(){
        HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);

        healthComponent.setCurrentHealth(1);
        Assertions.AssertBool(healthComponent.isDead()).IsFalse();
        healthComponent.Free();
    }

//TestCase applyDamage

    [TestCase]
    public void TestApplyDamageNormal(){
        HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);
        float maxH = healthComponent.getMaxHealth(); // unnötig kompliziert aber ich wollte mal irgendwas mit maxHealth machen
        healthComponent.setCurrentHealth(maxH);

        healthComponent.applyDamage(50);
        Assertions.AssertFloat(healthComponent.getCurrentHealth()).IsEqual(50);
        healthComponent.Free();

    }

    
    [TestCase]
    public void TestApplyDamageNegative(){
        HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);
        float maxH = healthComponent.getMaxHealth(); // unnötig kompliziert aber ich wollte mal irgendwas mit maxHealth machen
        healthComponent.setCurrentHealth(maxH);

        healthComponent.applyDamage(-50);
        Assertions.AssertFloat(healthComponent.getCurrentHealth()).IsEqual(100);
        healthComponent.Free();

    }

//TestCase heal

    [TestCase]
    public void TestHealNormal(){

         HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);
        healthComponent.setCurrentHealth(1);

        healthComponent.heal(50);
        Assertions.AssertFloat(healthComponent.getCurrentHealth()).IsEqual(51);
        healthComponent.Free();

    }

    [TestCase]
     public void TestHealNegative(){

         HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);
        healthComponent.setCurrentHealth(1);

        healthComponent.heal(-50);
        Assertions.AssertFloat(healthComponent.getCurrentHealth()).IsEqual(1);
        healthComponent.Free();

    }

     [TestCase]
     public void TestHealToMuch(){

        HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);
        healthComponent.setCurrentHealth(1);

        healthComponent.heal(100);
        Assertions.AssertFloat(healthComponent.getCurrentHealth()).IsEqual(100);
        healthComponent.Free();

    }

//TestCase DIE!!!! and _PhysicsProcess

    [TestCase]

    public void TestDie(){
        
        HealthComponent healthComponent = new HealthComponent();
        healthComponent.setMaxHealth(100);
        healthComponent.setCurrentHealth(-1);

        bool hasFired = false;
        healthComponent.onDeath += () => hasFired = true;
        healthComponent._PhysicsProcess(2);
        Assertions.AssertBool(hasFired).IsTrue();
        healthComponent.Free();


    }


}