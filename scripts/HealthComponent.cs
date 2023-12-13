using Godot;
using System;

public partial class HealthComponent : Node
{

	float maxHealth;
	float currentHealth;

	[Signal]
	public delegate void onDeathEventHandler();

	[Signal]
	public delegate void onDamageEventHandler();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private bool checkIfDead(){

		if(this.getCurrentHealth() <= 0){
			return true;
		}else{
			return false;
		}

	}

	public void applyDamage(float damage){

		this.currentHealth -= damage;
		EmitSignal(SignalName.onDamage);

	}

	public void heal(float heal){

		this.currentHealth += heal;

	}

	public void die(){

		EmitSignal(SignalName.onDeath);

	}

    public override void _PhysicsProcess(double delta) //Update Funktion
    {
        base._PhysicsProcess(delta);
		if(checkIfDead()){
			die();
		}
		
    }

    

	public float getMaxHealth(){

		return this.maxHealth;

	}

	public void setMaxHealth(float _maxHealth){

		this.maxHealth = _maxHealth;

	}

	public float getCurrentHealth(){

		return this.currentHealth;

	}

	public void setCurrentHealth(float _currentHealth){

		this.currentHealth = _currentHealth;

	}

	
}
