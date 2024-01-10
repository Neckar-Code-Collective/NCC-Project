using Godot;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;

public partial class Mage : Node
{
    Area3D Collector;
    Area3D Attractor;

    int currentBloodCount = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Collector = GetNode<Area3D>("BloodCollector");
        Attractor = GetNode<Area3D>("BloodAttractor");

        Collector.AreaEntered += OnBloodCollectorCollision;
        Attractor.AreaEntered += OnBloodAttractorCollision;
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

    }

	public void OnBloodCollectorCollision(Node3D other){
        //GD.Print("ICH SPÜRE BLUUUUUUUUUUUUUUUUUT!");

		if(other is Blood b){
            currentBloodCount += b.getAmount();
            b.QueueFree();
            GD.Print("Current blood: " + currentBloodCount);
        }
    }

	public void OnBloodAttractorCollision(Node3D other){
        
		if (other is Blood b){
            b.setLerpTarget(Collector.GlobalPosition);
            b.setLerpSpeed(.2f);
        }
    }


}
