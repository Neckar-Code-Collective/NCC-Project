using Godot;
using System;

///<summary>
///	The component, which manages the health of a entity
///</summary>

public partial class HealthComponent : Node
{

	/// <summary>
    /// 
    /// </summary>
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
		//if (isDead()){
		//	die();
		//}
	}

	private bool checkIfDead(){

		if(this.getCurrentHealth() <= 0){
			return true;
		}else{
			return false;
		}

	}

	public bool isDead(){
		return checkIfDead();
	}

	public void applyDamage(float damage){

		if(damage < 0){
			GD.Print("Trying to do negative damage!");
		}else{
		this.currentHealth -= damage;
		EmitSignal(SignalName.onDamage);
		}

	}

	public void heal(float heal){

		if(heal < 0){
			GD.Print("Trying to heal negative damage");
		}else{
		this.currentHealth += heal;
			if(this.getCurrentHealth() > this.getMaxHealth()){
				this.currentHealth = maxHealth;
			}
		}
		

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

	public void ResetDeathHandler(){
        backing_onDeath = null;

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
