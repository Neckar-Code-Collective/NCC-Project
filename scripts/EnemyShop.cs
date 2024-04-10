using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class EnemyShop : Interactable
{
    public Dictionary<string, int> _unlockCosts;

    private Label _costLabel;

    private Material originalMaterial;

    private Mage mage;

    private bool hovered;


    public override void _Ready()
    {
        mage = GetTree().Root.GetNode<Node>("Level/Mage") as Mage;
        _unlockCosts = mage.enemyUnlockCosts;
        _costLabel = GetNode<Label>("Control/CostLabel");
        _costLabel.AddThemeFontSizeOverride("font_size", 24);
        _costLabel.Visible = false;
        hovered = false;

        originalMaterial = GetNode<MeshInstance3D>("EnemyShopMesh").MaterialOverride;


        this.MouseEntered += () =>
        {
            ShowUnlockCost();
            StartGlowEffect();
            hovered = true;
        };
        this.MouseExited += () =>
        {
            HideUnlockCost();
            StopGlowEffect();
            hovered = false;
        };

        OnInteractMage += () =>
        {

            foreach(var entry in _unlockCosts)
            {
                if(!mage.HasUnlockedEnemy(entry.Key) && mage.GetCurrentBlood() >= entry.Value)
                {
                    mage.DeductBlood(entry.Value);
                    mage.UnlockEnemy(entry.Key);
                    break;
                }
            }
        };

        
    }

    public override void _Process(double delta)
    {
        UpdateLabelPosition();
    }




    private void ShowUnlockCost()
    {
        var enemy = new KeyValuePair<string, int>("Platzhalter", 4);
        foreach(var entry in _unlockCosts)
        {
            if(!mage.HasUnlockedEnemy(entry.Key))
            {
                enemy = entry;
                break;
            }
        }
        if(enemy.Key != "Platzhalter")
        {
            _costLabel.Text = $"Cost to Unlock {enemy.Key} : {enemy.Value} Blood";
            _costLabel.Visible = true;
        }
        else
        {
            _costLabel.Text = $"All Enemies lie at your feet and serve you!";
            _costLabel.Visible = true;
        }
    }

    private void HideUnlockCost()
    {
        _costLabel.Visible = false;
    }

    private void StartGlowEffect()
    {
        var meshInstance = GetNode<MeshInstance3D>("EnemyShopMesh");
        StandardMaterial3D newMaterial = new StandardMaterial3D
        {
            EmissionEnabled = true,
            Emission = new Color(1, 1, 0)
        };
        meshInstance.MaterialOverride = newMaterial;
    }

    private void StopGlowEffect()
    {
        var meshInstance = GetNode<MeshInstance3D>("EnemyShopMesh");
        meshInstance.MaterialOverride = originalMaterial;
    }

    private  void UpdateLabelPosition()
    {
        var camera = GetViewport().GetCamera3D();
        if (camera == null) return;

        var screenPos = camera.UnprojectPosition(this.GlobalTransform.Origin);

        _costLabel.Position = screenPos - _costLabel.Size / 2 - new Vector2(0,90);
        
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            if(hovered)
            {
                EmitSignal(nameof(OnInteractMage));
                HideUnlockCost();
                ShowUnlockCost();
            }
        }
    }

} 