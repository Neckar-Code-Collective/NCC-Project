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
    public int CurrentBloodCount = 0;

    /// <summary>
    /// Reference to the label that displays the current amount of blood
    /// </summary>
    public Label BloodLabel;

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


        BloodLabel = GetTree().Root.GetNode<Label>("Level/CanvasLayer/Control2/BloodLabel");
        InitializeLabels();
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
        BloodLabel.Text = "BLOOD IN THE BANK: " + CurrentBloodCount;

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
                CheckSpawn();
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
    /// Check whether we can spawn an enemy in the current mouse position
    /// </summary>
    /// <returns></returns>
    void CheckSpawn()
    {
        //get mouse position and instantiate enemy there
        var mousePosInView = GetViewport().GetMousePosition();

        var camera = GetViewport().GetCamera3D();

        var origin = camera.ProjectRayOrigin(mousePosInView);
        var direction = camera.ProjectRayNormal(mousePosInView);

        var distance = -origin.Y / direction.Y;

        var targetPosition = origin + direction * distance;

        //check if area is free


        var query = new PhysicsShapeQueryParameters3D();
        var shape = new SphereShape3D();
        shape.Radius = 2;
        //4 is the collisionmask for enemies 
        query.CollisionMask = 4;
        query.Shape = shape;
        query.Transform = query.Transform.Translated(targetPosition);

        var objs = GetViewport().GetCamera3D().GetWorld3D().DirectSpaceState.IntersectShape(query, 1);
        if (objs.Count >= 1)
        {
            //space is not empty
            GD.Print("Cant spawn, space not empty");
            return;
        }

        SpawnMob(targetPosition);

    }

    /// <summary>
    /// Spawns the mob into the scene
    /// </summary>
    /// <param name="pos">The position the mobs should be spawned in</param>
    void SpawnMob(Vector3 pos)
    {
        pos.Y = -1;
        switch (_selectionState)
        {
            case SelectionState.BASIC_ENEMY:
                var e = _basicEnemyPrefab.Instantiate<BasicEnemy>();
                _entityHolder.AddChild(e, true);
                e.GlobalPosition = pos;
                break;
            case SelectionState.CHARGER_ENEMY:
                var ch = _chargerEnemyPrefab.Instantiate<ChargeEnemy>();
                _entityHolder.AddChild(ch, true);
                ch.GlobalPosition = pos;
                break;
            case SelectionState.REVENANT_ENEMY:
                var r = _revenantEnemyPrefab.Instantiate<RevenantEnemy>();
                _entityHolder.AddChild(r, true);
                r.GlobalPosition = pos;
                break;
        }
    }

    private void _onBasicEnemyPress()
    {
        _selectionState = SelectionState.BASIC_ENEMY;
        GD.Print("Selecting");
    }

    private void _onChargerEnemyPress()
    {
        _selectionState = SelectionState.CHARGER_ENEMY;
        GD.Print("Selecting");
    }

    private void _onRevenantEnemyPress()
    {
        _selectionState = SelectionState.REVENANT_ENEMY;
        GD.Print("Selecting");
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
            CurrentBloodCount += b.GetAmount();
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


}

public enum SelectionState
{
    NONE, BASIC_ENEMY, CHARGER_ENEMY, REVENANT_ENEMY
}