using Godot;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Net;

/// <summary>
/// The main class of the mage. Controls spawning, mana and camera movement
/// </summary>
public partial class Mage : Node
{
    /// <summary>
    /// The area that collects blood
    /// </summary>
    Area3D _collector;

    /// <summary>
    /// The area that attracts blood
    /// </summary>
    Area3D _attractor;

    /// <summary>
    /// The amount of blood points the mage currently has
    /// </summary>
    public int _currentBloodCount = 0;

    /// <summary>
    /// Reference to the label that displays the current amount of blood
    /// </summary>
    public Label BloodLabel;

    const int SPAWNCOST = 1;

    /// <summary>
    /// Reference to the ManaManager
    /// </summary>
    ManaManager _manaManager;


    /// <summary>
    /// The currently selected mob to spawn
    /// </summary>
    SelectionState _selectionState;

    /// <summary>
    /// Reference to the BasicEnemy Button
    /// </summary>
    Button _basicEnemyButton;

    /// <summary>
    /// Reference to the ChargerEnemy Button
    /// </summary>
    Button _chargerEnemyButton;

    /// <summary>
    /// Reference to the Revenant Button
    /// </summary>
    Button _revenantButton;

    Button _hydraButton;

    /// <summary>
    /// Reference to the node that holds all entities for replication
    /// </summary>
    [Export]
    Node _entityHolder;

    /// <summary>
    /// Prefab of the basic enemy
    /// </summary>
    [Export]
    PackedScene _basicEnemyPrefab;

    /// <summary>
    /// Prefab of the charger enemy
    /// </summary>
    [Export]
    PackedScene _chargerEnemyPrefab;

    /// <summary>
    /// Prefab to the revenant enemy
    /// </summary>
    [Export]
    PackedScene _revenantEnemyPrefab;

    [Export]
    PackedScene _hydraEnemyPrefab;

    TextureRect _chargerLockIcon;

    TextureRect _revenantLockIcon;

    TextureRect _hydraLockIcon;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        //in case we are a shooter, we can just remove this node
        if (!Global.Is_Mage)
        {
            QueueFree();
        }

        //create manamanager
        _manaManager = new ManaManager();
        AddChild(_manaManager);

        //setup references
        _collector = GetNode<Area3D>("BloodCollector");
        _attractor = GetNode<Area3D>("BloodAttractor");

        _collector.AreaEntered += OnBloodCollectorCollision;
        _attractor.AreaEntered += OnBloodAttractorCollision;

        _basicEnemyButton = GetNode<Button>("CanvasLayer/MageUI/Panel/BasicEnemy");
        _basicEnemyButton.Pressed += _onBasicEnemyPress;
        _chargerEnemyButton = GetNode<Button>("CanvasLayer/MageUI/Panel/ChargerEnemy");
        _chargerEnemyButton.Pressed += _onChargerEnemyPress;
        _revenantButton = GetNode<Button>("CanvasLayer/MageUI/Panel/RevenantEnemy");
        _revenantButton.Pressed += _onRevenantEnemyPress;
        _hydraButton = GetNode<Button>("CanvasLayer/MageUI/Panel/HydraEnemy");
        _hydraButton.Pressed += _onHydraEnemyPress;


        BloodLabel = GetTree().Root.GetNode<Label>("Level/CanvasLayer/Control2/BloodLabel");
        InitializeLabels();

        _chargerLockIcon = _chargerEnemyButton.GetNode<TextureRect>("ChargerLock");
        _chargerLockIcon.Visible = true;
        _chargerEnemyButton.Disabled = true;
        _revenantLockIcon = _revenantButton.GetNode<TextureRect>("RevenantLock");
        _revenantLockIcon.Visible = true;
        _revenantButton.Disabled = true;
        _hydraLockIcon = _hydraButton.GetNode<TextureRect>("HydraLock");
        _hydraLockIcon.Visible = true;
        _hydraButton.Disabled = true;

    }

    /// <summary>
    /// Updates the position of the collectors and the text
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        //calculate mouse position in world space
        var mousePosInView = GetViewport().GetMousePosition();
        var camera = GetViewport().GetCamera3D();
        var origin = camera.ProjectRayOrigin(mousePosInView);
        var direction = camera.ProjectRayNormal(mousePosInView);

        //find distance so we hit y = 0
        var distance = -origin.Y / direction.Y;
        var target_position = origin + direction * distance;


        //set the collector position
        _collector.GlobalPosition = target_position;
        _attractor.GlobalPosition = target_position;

        //update ui text
        BloodLabel.Text = "BLOOD IN THE BANK: " + _currentBloodCount;

        UpdateButtonStates();

    }

    /// <summary>
    /// Whether the user holds down the right mouse button with intent to move the camera
    /// </summary>
    bool _isMovingCamera = false;

    /// <summary>
    /// The sensitivity of mouse movements
    /// </summary>
    [Export]
    public float MouseSensitivity = 1;

    //Handles different inputs
    public override void _UnhandledInput(InputEvent @event)
    {
        //check if the player is canceling the spawning via escape. This resets the current selection
        if (@event is InputEventKey ie)
        {
            if (ie.Keycode == Key.Escape)
            {
                _selectionState = SelectionState.NONE;
            }
        }

        //check if the player is canceling the spawing via right click. This resets the current selection
        if (@event is InputEventMouseButton me)
        {
            if (me.ButtonIndex == MouseButton.Right)
            {
                _selectionState = SelectionState.NONE;
            }
        }

        //check for left mouse click. If we have something selected, we want to spawn it into the world
        if (@event is InputEventMouseButton me2)
        {
            if (me2.ButtonIndex == MouseButton.Left && _selectionState != SelectionState.NONE && me2.Pressed)
            {

                //check if we can do the spawn
                var pos = GetSpawnPosition();
                if (CheckSpawn(pos))
                {
                    SpawnMob(pos);
                }
            }
        }

        //check if the user holds down the right mouse button
        if (@event is InputEventMouseButton)
        {
            var e = @event as InputEventMouseButton;
            if (e.ButtonIndex == MouseButton.Right)
            {
                _isMovingCamera = e.Pressed;
            }
        }

        //if the user is holding down the right mouse button and moves the mouse, we want to move the camera
        if (@event is InputEventMouseMotion)
        {
            var e = @event as InputEventMouseMotion;
            if (_isMovingCamera)
            {
                var cam = GetViewport().GetCamera3D();
                cam.GlobalPosition -= new Vector3(e.Relative.X * MouseSensitivity, 0, e.Relative.Y * MouseSensitivity);
            }
        }

    }

    /// <summary>
    /// Gets the location where the mob would be spawned
    /// </summary>
    /// <returns>The location in global space</returns>
    public Vector3 GetSpawnPosition()
    {
        //get mouse position and instantiate enemy there
        var mousePosInView = GetViewport().GetMousePosition();

        var camera = GetViewport().GetCamera3D();

        var origin = camera.ProjectRayOrigin(mousePosInView);
        var direction = camera.ProjectRayNormal(mousePosInView);

        var distance = -origin.Y / direction.Y;

        var targetPosition = origin + direction * distance;

        return targetPosition;
    }

    /// <summary>
    /// Check whether we can spawn an enemy in the current mouse position
    /// </summary>
    /// <returns>Whether we can spawn there</returns>
    public bool CheckSpawn(Vector3 targetPosition)
    {


        //check if area is free


        var query = new PhysicsShapeQueryParameters3D();
        var shape = new SphereShape3D();
        shape.Radius = 1.25f;
        //4 is the collisionmask for enemies 
        query.CollisionMask = 4 + 32;
        query.Shape = shape;
        query.Transform = query.Transform.Translated(targetPosition);

        var objs = GetViewport().GetCamera3D().GetWorld3D().DirectSpaceState.IntersectShape(query, 1);
        if (objs.Count >= 1)
        {
            //space is not empty
            GD.Print("Cant spawn, space not empty");
            return false;
        }

        return true;

    }

    /// <summary>
    /// Spawns the mob into the scene
    /// </summary>
    /// <param name="pos">The position the mobs should be spawned in</param>
    public void SpawnMob(Vector3 pos)
    {
        pos.Y = -1;
        switch (_selectionState)
        {
            case SelectionState.BASIC_ENEMY:
                if (_manaManager.GetCurrentMana() >= SPAWNCOST)
                {
                    var e = _basicEnemyPrefab.Instantiate<BasicEnemy>();
                    _entityHolder.AddChild(e, true);
                    e.GlobalPosition = pos;
                    _manaManager.RemoveMana(SPAWNCOST);
                }
                break;
            case SelectionState.CHARGER_ENEMY:
                if (_manaManager.GetCurrentMana() >= SPAWNCOST * 2 && _chargerLockIcon.Visible == false)
                {
                    var ch = _chargerEnemyPrefab.Instantiate<ChargeEnemy>();
                    _entityHolder.AddChild(ch, true);
                    ch.GlobalPosition = pos;
                    _manaManager.RemoveMana(SPAWNCOST * 2);
                }
                break;
            case SelectionState.REVENANT_ENEMY:
                if (_manaManager.GetCurrentMana() >= SPAWNCOST * 2 && _revenantLockIcon.Visible == false)
                {
                    var r = _revenantEnemyPrefab.Instantiate<RevenantEnemy>();
                    _entityHolder.AddChild(r, true);
                    r.GlobalPosition = pos;
                    _manaManager.RemoveMana(SPAWNCOST * 2);
                }
                break;
            case SelectionState.HYDRA_ENEMY:
                if (_manaManager.GetCurrentMana() >= SPAWNCOST * 8)
                {
                    var h = _hydraEnemyPrefab.Instantiate<HydraEnemy>();
                    _entityHolder.AddChild(h, true);
                    h.GlobalPosition = pos;
                    _manaManager.RemoveMana(SPAWNCOST * 8);
                }
                break;
        }
    }

    private void _onBasicEnemyPress()
    {
        _selectionState = SelectionState.BASIC_ENEMY;
    }

    private void _onChargerEnemyPress()
    {
        _selectionState = SelectionState.CHARGER_ENEMY;
    }

    private void _onRevenantEnemyPress()
    {
        _selectionState = SelectionState.REVENANT_ENEMY;
    }

    private void _onHydraEnemyPress()
    {
        _selectionState = SelectionState.HYDRA_ENEMY;
    }

    /// <summary>
    /// Sets the references to the labels
    /// </summary>
    private void InitializeLabels()
    {
        ColorRect red = GetTree().Root.GetNode<ColorRect>("Level/CanvasLayer/Control2/ColorRect2");
        if (!Multiplayer.IsServer())
        {
            BloodLabel.Visible = false;
            red.Visible = false;
        }
        else
        {
            BloodLabel.Visible = true;
            red.Visible = true;
        }
    }

    /// <summary>
    /// Called when a blood orb intersects with the collector. This removes the orb and adds its value to the mages blood count
    /// </summary>
    /// <param name="other"></param>
    public void OnBloodCollectorCollision(Node3D other)
    {
        //GD.Print("ICH SPÜRE BLUUUUUUUUUUUUUUUUUT!");

        if (other is Blood b)
        {
            _currentBloodCount += b.GetAmount();
            b.QueueFree();
        }
    }

    /// <summary>
    /// Called when a blood orb intersects with the attractor. This sets the orbs lerpTarget to our position
    /// </summary>
    /// <param name="other"></param>
    public void OnBloodAttractorCollision(Node3D other)
    {

        if (other is Blood b)
        {
            b.SetLerpTarget(_collector.GlobalPosition);
            b.SetLerpSpeed(.2f);
        }
    }

    public void UpdateButtonStates()
    {
        float currentMana = _manaManager.GetCurrentMana();

        _chargerLockIcon.TooltipText = !_chargerLockIcon.Visible ? "" : "Can be unlocked at OPEN GRAVE!";
        _revenantLockIcon.TooltipText = !_revenantLockIcon.Visible ? "" : "Can be unlocked at OPEN GRAVE!";
        _hydraLockIcon.TooltipText = !_hydraLockIcon.Visible ? "" : "Can be unlocked at OPEN GRAVE!";


        UpdateButtonState(_basicEnemyButton, currentMana >= SPAWNCOST, SPAWNCOST, false);
        UpdateButtonState(_chargerEnemyButton, currentMana >= SPAWNCOST * 2, SPAWNCOST * 2, _chargerLockIcon.Visible);
        UpdateButtonState(_revenantButton, currentMana >= SPAWNCOST * 2, SPAWNCOST * 2, _revenantLockIcon.Visible);
        UpdateButtonState(_hydraButton, currentMana >= SPAWNCOST * 8, SPAWNCOST * 8, _hydraLockIcon.Visible);

    }

    public void UpdateButtonState(Button button, bool canAfford, float cost, bool lockvisibility)
    {
        if (!lockvisibility)
        {
            button.Disabled = !canAfford;
            button.Modulate = canAfford ? Colors.White : new Color(0.5f, 0.5f, 0.5f, 1);
            button.TooltipText = canAfford ? "" : $"Need {cost} Mana";
        }
        else
        {
            button.Disabled = true;
            button.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }


    public void DeductBlood(int amount)
    {
        _currentBloodCount -= amount;
        //if (IsMultiplayerAuthority())
        //    Rpc(nameof(RpcUpdateBloodOnOtherPeers), _currentBloodCount);
    }

    [Rpc()]
    public void RpcUpdateBloodOnOtherPeers(int amount)
    {
        _currentBloodCount = amount;
    }

    public Dictionary<string, int> enemyUnlockCosts = new Dictionary<string, int>
    {
        {"ChargerEnemy", 10},
        {"Revenant", 20},
        {"Hydra", 40}

    };
    public bool HasUnlockedEnemy(string Enemy)
    {
        if (Enemy == "ChargerEnemy")
        {
            return !_chargerLockIcon.Visible;
        }
        else if (Enemy == "Revenant")
        {
            return !_revenantLockIcon.Visible;
        }
        else if (Enemy == "Hydra")
        {
            return !_hydraLockIcon.Visible;
        }
        else
        {
            GD.Print("ungültiger Enemy");
            return false;
        }
    }

    public void UnlockEnemy(String Enemy)
    {
        if (Enemy == "ChargerEnemy")
        {
            _chargerLockIcon.Visible = false;
        }
        else if (Enemy == "Revenant")
        {
            _revenantLockIcon.Visible = false;
        }
        else if (Enemy == "Hydra")
        {
            _hydraLockIcon.Visible = false;
        }
        else
        {
            GD.Print("ungültiger Enemy");
        }
    }

    public int GetCurrentBlood()
    {
        return _currentBloodCount;
    }


    public ManaManager GetManaManager()
    {
        return _manaManager;
    }


    public void SetSelectionState(SelectionState s){
        _selectionState = s;
    }

}




public enum SelectionState
{
    NONE, BASIC_ENEMY, CHARGER_ENEMY, REVENANT_ENEMY, HYDRA_ENEMY
}

