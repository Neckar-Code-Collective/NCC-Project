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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

        if(!Global.Is_Mage){
            QueueFree();
        }

        Collector = GetNode<Area3D>("BloodCollector");
        Attractor = GetNode<Area3D>("BloodAttractor");

        Collector.AreaEntered += OnBloodCollectorCollision;
        Attractor.AreaEntered += OnBloodAttractorCollision;

        BasicEnemyButton = GetNode<Button>("MageUI/Panel/BasicEnemy");
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
    	private void InitializeLabels()
	    {
            ColorRect red = GetTree().Root.GetNode<ColorRect>("Level/CanvasLayer/Control2/ColorRect2");
            if(!Multiplayer.IsServer())
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

	public void OnBloodCollectorCollision(Node3D other){
        //GD.Print("ICH SPÜRE BLUUUUUUUUUUUUUUUUUT!");

		if(other is Blood b){
            currentBloodCount += b.getAmount();
            b.QueueFree();
        }
    }

	public void OnBloodAttractorCollision(Node3D other){
        
		if (other is Blood b){
            b.setLerpTarget(Collector.GlobalPosition);
            b.setLerpSpeed(.2f);
        }
    }


}

    public enum SelectionState {
        NONE,BASIC_ENEMY
    }