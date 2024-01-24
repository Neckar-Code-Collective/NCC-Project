using Godot;
using Microsoft.CodeAnalysis;
using System;



public partial class ChargeEnemy : Enemy{

    float WaitTime = 2.0f;
    enum ChargeEnemyState
    {
        WALK, CHARGEUP, SPRINT
    }

    ChargeEnemyState state = ChargeEnemyState.WALK;

    Timer timer = new Timer();
    public void OnTimerFire(){
        switch(state){
            case ChargeEnemyState.WALK:
                setMovementSpeed(0f);
                state = ChargeEnemyState.CHARGEUP;
                timer.Start(2);
                break;
            case ChargeEnemyState.CHARGEUP:
                setMovementSpeed(10f);
                state = ChargeEnemyState.SPRINT;
                timer.Start(0.75);
                break;
            case ChargeEnemyState.SPRINT:
                setMovementSpeed(1.0f);
                state = ChargeEnemyState.WALK;
                timer.Start(5);
                break;
        }

    }

    public override void _Ready()
    {
        base._Ready();
        health.setMaxHealth(100f);
        health.setCurrentHealth(health.getMaxHealth());
        setMovementSpeed(1.0f);

        AddChild(timer);
        timer.OneShot = false;
        timer.Start(5);
        timer.Timeout += OnTimerFire;

        setNetWorth(10);

    }


    public float getWaitTime(){
        return this.WaitTime;
    }

    public void setWaitTime(float _wt){
        this.WaitTime = _wt;
    }



    



}