using Godot;
using Microsoft.CodeAnalysis;
using System;


/// <summary>
/// An enemytype which stops for a few seconds only to charge up a sprint to the nearest player
/// </summary>
public partial class ChargeEnemy : Enemy{

    /// <summary>
    /// The time the enemy charges up
    /// </summary>
    float _waitTime = 2.0f;

    /// <summary>
    /// The states this enemy can be in
    /// </summary>
    enum ChargeEnemyState
    {
        WALK, CHARGEUP, SPRINT
    }

    ChargeEnemyState _state = ChargeEnemyState.WALK;

    /// <summary>
    /// Timer used for the delay
    /// </summary>
    Timer _timer = new Timer();
    /// <summary>
    /// Callback to the timer, switches state
    /// </summary>
    public void OnTimerFire(){
        switch(_state){
            case ChargeEnemyState.WALK:
                //if we are currently in the WALK state, we shall stop and switch to the CHARGEUP state
                SetMovementSpeed(0f);
                _state = ChargeEnemyState.CHARGEUP;
                _timer.Start(2);
                break;
            case ChargeEnemyState.CHARGEUP:
                //if we are currently in the CHARGEUP state, we dramatically increase our movementspeed and switch into the SPRINT state.
                SetMovementSpeed(10f);
                _state = ChargeEnemyState.SPRINT;
                _timer.Start(0.75);
                break;
            case ChargeEnemyState.SPRINT:
                //if we are currently in the SPRINT state, we reset to the normal WALK state.
                SetMovementSpeed(1.0f);
                _state = ChargeEnemyState.WALK;
                _timer.Start(5);
                break;
        }

    }

    /// <summary>
    /// Sets up the stats for this enemy
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        
        //set max health and move speed
        _health.SetMaxHealth(100f);
        _health.SetCurrentHealth(_health.GetMaxHealth());
        SetMovementSpeed(1.0f);

        //setup timer
        AddChild(_timer);
        _timer.OneShot = false;
        _timer.Start(5);
        _timer.Timeout += OnTimerFire;

        //how much money this enemy drops
        SetNetWorth(10);

    }


    public float GetWaitTime(){
        return this._waitTime;
    }

    public void SetWaitTime(float _wt){
        this._waitTime = _wt;
    }

}