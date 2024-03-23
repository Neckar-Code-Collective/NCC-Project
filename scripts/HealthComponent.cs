using Godot;
using System;

///<summary>
///	The component which manages the health of a entity
///</summary>

public partial class HealthComponent : Node
{

    /// <summary>
    /// The maximum amount of health this entitiy can have
    /// </summary>
    float _maxHealth;

    /// <summary>
    /// The amount of health this entity currently has
    /// </summary>
    float _currentHealth;

    /// <summary>
    /// A signal that gets emitted when this entity falls below zero health
    /// </summary>
    [Signal]
    public delegate void onDeathEventHandler();

    /// <summary>
    /// A signal that gets emitted when this entity gets damaged
    /// </summary>
    [Signal]
    public delegate void onDamageEventHandler();

    /// <summary>
    /// Checks if the entity is dead, i.e. _currentHealth <= 0
    /// </summary>
    /// <returns></returns>
    public bool IsDead()
    {

        if (this.GetCurrentHealth() <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

	/// <summary>
    /// Removes health from this entity
    /// </summary>
    /// <param name="damage">the amount of damage to deal</param>
    public void ApplyDamage(float damage)
    {

        if (damage < 0)
        {
            GD.Print("Trying to do negative damage!");
        }
        else
        {
            this._currentHealth -= damage;
            EmitSignal(SignalName.onDamage);

        }

    }

	/// <summary>
    /// Adds health to this entity
    /// </summary>
    /// <param name="heal">The amount of health to add</param>
    public void Heal(float heal)
    {

        if (heal < 0)
        {
            GD.Print("Trying to heal negative damage");
        }
        else
        {
            this._currentHealth += heal;
            if (this.GetCurrentHealth() > this.GetMaxHealth())
            {
                this._currentHealth = _maxHealth;
            }
        }


    }

	/// <summary>
    /// Kills this entity 
    /// </summary>
    public void Die()
    {

        EmitSignal(SignalName.onDeath);

    }

	/// <summary>
    /// Checks if the entity is dead and emits corresponding signals
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta) //Update Funktion
    {
        base._PhysicsProcess(delta);
        if (IsDead())
        {
            Die();
        }
        

    }

	/// <summary>
    /// Removes all currently set death handles
    /// </summary>
    public void ResetDeathHandler()
    {
        backing_onDeath = null;

    }



    public float GetMaxHealth()
    {

        return this._maxHealth;

    }

    public void SetMaxHealth(float _maxHealth)
    {

        this._maxHealth = _maxHealth;

    }

    public float GetCurrentHealth()
    {

        return this._currentHealth;

    }

    public void SetCurrentHealth(float _currentHealth)
    {

        this._currentHealth = _currentHealth;

    }


}
