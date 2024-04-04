using Godot;
using System;


/// <summary>
/// The ManaManager keeps track of the current mana state and calculating regenration
/// </summary>

public partial class ManaManager : Node
{
	/// <summary>
    /// The amount of mana the mage currently has
    /// </summary>
    float _currentMana = 0;

	/// <summary>
    /// The maximum amount of mana the mage can hold
    /// </summary>
    float _maxMana = 10;

	/// <summary>
    /// The rate at which mana regenrates
    /// </summary>
    float _manaRegen = 1;

	ProgressBar _manaBar;

	Label _manaLabel;

    public override void _Ready()
    {
		_manaBar = GetTree().Root.GetNode<ProgressBar>("Level/Mage/CanvasLayer/MageUI/ManaBar");
		_manaBar.MaxValue = _maxMana;
		_manaBar.Value = _currentMana;

		_manaLabel = GetTree().Root.GetNode<Label>("Level/Mage/CanvasLayer/MageUI/ManaBar/ManaLabel");
    }




    public float GetCurrentMana(){
		return _currentMana;
	}

	public float GetMaxMana(){
		return _maxMana;
	}

	public float GetManaRegen(){
		return _manaRegen;
	}

	public void SetMaxMana(float max){
		_maxMana = max;
		_currentMana = Math.Min(_currentMana,_maxMana);
	}

	public void SetManaRegen(float regen){
		_manaRegen = regen;
	}

	public void SetCurrentMana(float mana){
		_currentMana = mana;
		_currentMana = Math.Min(_currentMana,_maxMana);
	}


	/// <summary>
	/// Removes a amount of mana from currentMana
	/// </summary>
	/// <param name="amount">The amount to remove</param>
	public void RemoveMana(float amount){
		_currentMana -= amount;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		Update(delta);
		
    }

	/// <summary>
	/// Applies manaregen and checks for bounds
	/// </summary>
	/// <param name="delta">the amount of time that has passed</param>
	public void Update(double delta){
		_currentMana += _manaRegen*(float)delta;

		_currentMana = Math.Min(_currentMana,_maxMana);
		_manaBar.Value = _currentMana;

		_manaLabel.Text = $"{(int)_currentMana}";
		
	}

}
