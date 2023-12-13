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

	public float removeMana(){
		
	}

}
