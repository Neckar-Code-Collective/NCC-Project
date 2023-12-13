using Godot;
using System;

public partial class ManaManager : Node
{
    float currentMana = 0;
    float maxMana = 10;
    float manaRegen = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }



	public float getCurrentMana(){
		return currentMana;
	}

	public float getMaxMana(){
		return maxMana;
	}

	public float getManaRegen(){
		return manaRegen;
	}

	public void setMaxMana(float max){
		maxMana = max;
		currentMana = Math.Min(currentMana,maxMana);
	}

	public void setManaRegen(float regen){
		manaRegen = regen;
	}

	public void setCurrentMana(float mana){
		currentMana = mana;
		currentMana = Math.Min(currentMana,maxMana);
	}

	public void removeMana(float amount){
		currentMana -= amount;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		Update(delta);
		
    }

	public void Update(double delta){
		currentMana += manaRegen*(float)delta;

		currentMana = Math.Min(currentMana,maxMana);
	}

}
