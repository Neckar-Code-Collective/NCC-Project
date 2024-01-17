using Godot;
using System;

public partial class Money: Area3D {

    int moneyAmount = 0;

    Vector3 lerpTarget = Vector3.Zero;

    float lerpSpeed = 0.5f;

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

    public void setMoneyAmount(int _moneyAmount){
        moneyAmount = _moneyAmount;

    }

    public int getMoneyAmount(){
        return moneyAmount;

    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = GlobalPosition.Lerp(lerpTarget, lerpSpeed);
    }














}