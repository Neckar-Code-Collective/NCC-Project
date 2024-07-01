using System;
using Godot;
using GdUnit4;
using GdUnit4.Asserts;
using Tests;
using System.Threading.Tasks;

namespace Tests;

[TestSuite]
public class EnemyShopTests
{
    private EnemyShop _enemyShop;
    private Mage _mage;

    private ISceneRunner runner;

    [Before]
    public async Task Setup()
    {
        Global.Is_Mage = true;
        runner = ISceneRunner.Load("res://level.tscn");
        await runner.AwaitIdleFrame();
        _enemyShop = runner.Scene().GetNode<EnemyShop>("EnemyShop");
        _mage = runner.Scene().GetNode<Node>("Mage") as Mage;

    }

    [TestCase]
    public void TestInitialCostLabel()
    {
        Assertions.AssertBool(_enemyShop.GetNode<Label>("Control/CostLabel").Visible).IsFalse();
    }

    [TestCase]
    public void TestShowUnlockCost()
    {
        var costLabel = _enemyShop.GetNode<Label>("Control/CostLabel");
        _enemyShop.ShowUnlockCost();

        Assertions.AssertBool(costLabel.Visible).IsTrue();
        Assertions.AssertString(costLabel.Text).IsNotEmpty();
    }

    [TestCase]
    public void TestInteraction()
    {
        _mage.SetCurrentBlood(300);
        var initialBlood = _mage.GetCurrentBlood();
        var unlockCosts = _enemyShop._unlockCosts;

        // Simulate interaction
        foreach (var entry in unlockCosts)
        {
            _enemyShop.EmitSignal(nameof(EnemyShop.OnInteractMage));
        }

        // Check if Mage blood was deducted correctly
        var remainingBlood = _mage.GetCurrentBlood();
        Assertions.AssertFloat(remainingBlood).IsLess(initialBlood);

        // Check if enemy was unlocked
        foreach (var entry in unlockCosts)
        {
            Assertions.AssertBool(_mage.HasUnlockedEnemy(entry.Key)).IsTrue();
        }
    }

    [TestCase]
    public void TestGlowEffect()
    {
        var meshInstance = _enemyShop.GetNode<MeshInstance3D>("EnemyShopMesh");

        _enemyShop.StartGlowEffect();
        var emissionMaterial = meshInstance.MaterialOverride as StandardMaterial3D;

        Assertions.AssertBool(emissionMaterial.EmissionEnabled).IsTrue();
        Assertions.AssertBool(emissionMaterial.Emission == new Color(1, 1, 0));

        _enemyShop.StopGlowEffect();
        Assertions.AssertBool(meshInstance.MaterialOverride == _enemyShop.originalMaterial);
    }
}
