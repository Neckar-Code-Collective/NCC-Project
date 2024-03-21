using Godot;
using System;

public partial class Blood : Area3D
{
    int amount = 0;
    Vector3 lerpTarget = Vector3.Zero;
    float lerpSpeed = 0.2f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void setLerpTarget(Vector3 target){
        lerpTarget = target;
    }

	public void setLerpSpeed(float s){
        lerpSpeed = s;
    }

	public void setAmount(int a){
        amount = a;
    }

	public int getAmount(){
        return amount;
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = GlobalPosition.Lerp(lerpTarget, lerpSpeed);
    }
}
