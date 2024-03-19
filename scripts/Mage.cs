using Godot;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;

public partial class Mage : Node
{
    Area3D Collector;
    Area3D Attractor;

    public int currentBloodCount = 0;
    public Label BloodLabel;



    SelectionState SelectionState;

    Button BasicEnemyButton;
    Button ChargerEnemyButton;
    Button RevenantButton;

    [Export]
    Node _entityHolder;

    [Export]
    PackedScene _basicEnemyPrefab;
    [Export]
    PackedScene _chargerEnemyPrefab;
    [Export]
    PackedScene _revenantEnemyPrefab;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        if (!Global.Is_Mage)
        {
            QueueFree();
        }

        Collector = GetNode<Area3D>("BloodCollector");
        Attractor = GetNode<Area3D>("BloodAttractor");

        Collector.AreaEntered += OnBloodCollectorCollision;
        Attractor.AreaEntered += OnBloodAttractorCollision;

        BasicEnemyButton = GetNode<Button>("CanvasLayer/MageUI/Panel/BasicEnemy");
        BasicEnemyButton.Pressed += _onBasicEnemyPress;
        ChargerEnemyButton = GetNode<Button>("CanvasLayer/MageUI/Panel/ChargerEnemy");
        ChargerEnemyButton.Pressed += _onChargerEnemyPress;
        RevenantButton = GetNode<Button>("CanvasLayer/MageUI/Panel/RevenantEnemy");
        RevenantButton.Pressed += _onRevenantEnemyPress;


        BloodLabel = GetTree().Root.GetNode<Label>("Level/CanvasLayer/Control2/BloodLabel");
        InitializeLabels();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var mousePosInView = GetViewport().GetMousePosition();

        var camera = GetViewport().GetCamera3D();

        var origin = camera.ProjectRayOrigin(mousePosInView);
        var direction = camera.ProjectRayNormal(mousePosInView);

        var distance = -origin.Y / direction.Y;

        var target_position = origin + direction * distance;

        //GD.Print(target_position);

        Collector.GlobalPosition = target_position;
        Attractor.GlobalPosition = target_position;
        BloodLabel.Text = "BLOOD IN THE BANK: " + currentBloodCount;

    }



    
    public override void _UnhandledInput(InputEvent @event)
    {
        //check if the player is canceling the spawning via escape
        if (@event is InputEventKey ie)
        {
            if (ie.Keycode == Key.Escape)
            {
                SelectionState = SelectionState.NONE;
            }
        }

        //check if the player is canceling the spawing via right click
        if (@event is InputEventMouseButton me)
        {
            if (me.ButtonIndex == MouseButton.Right)
            {
                SelectionState = SelectionState.NONE;
            }
        }

        //spawning of enemies
        if (@event is InputEventMouseButton me2)
        {
            if (me2.ButtonIndex == MouseButton.Left && SelectionState != SelectionState.NONE && me2.Pressed)
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
                query.CollisionMask = 4;
                query.Shape = shape;
                query.Transform = query.Transform.Translated(targetPosition);

                var objs = GetViewport().GetCamera3D().GetWorld3D().DirectSpaceState.IntersectShape(query,1);
                if(objs.Count >= 1){
                    //space is not empty
                    GD.Print("Cant spawn, space not empty");
                    return;
                }
                
                _spawnMob(targetPosition);
            }
        }
        
    }

    void _spawnMob(Vector3 pos)
    {
        pos.Y = -1;
        switch(SelectionState){
            case SelectionState.BASIC_ENEMY:
                var e = _basicEnemyPrefab.Instantiate<BasicEnemy>();
                _entityHolder.AddChild(e,true);
                e.GlobalPosition = pos;
                break;
            case SelectionState.CHARGER_ENEMY:
                var ch = _chargerEnemyPrefab.Instantiate<ChargeEnemy>();
                _entityHolder.AddChild(ch,true);
                ch.GlobalPosition = pos;
                break;
            case SelectionState.REVENANT_ENEMY:
                var r = _revenantEnemyPrefab.Instantiate<RevenantEnemy>();
                _entityHolder.AddChild(r,true);
                r.GlobalPosition = pos;
                break;
        }
    }

    private void _onBasicEnemyPress()
    {
        SelectionState = SelectionState.BASIC_ENEMY;
        GD.Print("Selecting");
    }

    private void _onChargerEnemyPress()
    {
        SelectionState = SelectionState.CHARGER_ENEMY;
        GD.Print("Selecting");
    }

    private void _onRevenantEnemyPress()
    {
        SelectionState = SelectionState.REVENANT_ENEMY;
        GD.Print("Selecting");
    }

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

    public void OnBloodCollectorCollision(Node3D other)
    {
        //GD.Print("ICH SPÜRE BLUUUUUUUUUUUUUUUUUT!");

        if (other is Blood b)
        {
            currentBloodCount += b.getAmount();
            b.QueueFree();
        }
    }

    public void OnBloodAttractorCollision(Node3D other)
    {

        if (other is Blood b)
        {
            b.setLerpTarget(Collector.GlobalPosition);
            b.setLerpSpeed(.2f);
        }
    }


}

public enum SelectionState
{
    NONE, BASIC_ENEMY, CHARGER_ENEMY, REVENANT_ENEMY
}