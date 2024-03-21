using Godot;
using Microsoft.CodeAnalysis;
using System;
using System.Runtime.CompilerServices;


/// <summary>
/// The Revenant is a special enemy that revives itself for a short time after dying and sprints really fast
/// </summary>
public partial class RevenantEnemy : Enemy{

    enum RevenantEnemyState
    { 
        ALIVE, REVENANT
    }
    /// <summary>
    /// The states the enemy can be in
    /// </summary>
    RevenantEnemyState state = RevenantEnemyState.ALIVE;

    //timers
    Timer timer = new Timer();
    Timer ticker = new Timer();





    public override void _Ready()
    {
        base._Ready();
        AddChild(timer);
        AddChild(ticker);

        //setup stats
        _health.SetMaxHealth(100f);
        _health.SetCurrentHealth(_health.GetMaxHealth());
        _health.ResetDeathHandler();
        SetMovementSpeed(1.0f);
        SetNetWorth(15);

        //add new on death handler
        _health.onDeath += () =>
        {
            switch (state)
            {
                case RevenantEnemyState.ALIVE:
                    //when we die and are in the ALIVE state, we stop for a short time and reset our health
                    state = RevenantEnemyState.REVENANT;
                    ticker.OneShot = false;
                    timer.OneShot = true;
                    ticker.Start(2.1f);
                    ticker.Timeout += Ticker;
                    SetMovementSpeed(0);
                    _health.SetCurrentHealth(120f);
                    timer.Timeout += () =>
                    {
                        SetMovementSpeed(2.0f);
                    };
                    timer.Start(2);
                    break;
                case RevenantEnemyState.REVENANT:
                    //when we die in the REVENANT state, we die for real
                    Rpc(nameof(RpcDie));
                    //Spawn Money
                    Global.NetworkManager.SpawnMoney(GlobalPosition, NetWorth);
                    break;
            
                }
        };


    }

    /// <summary>
    /// Callback for the ticker, which reduces the health and increases movespeed until death
    /// </summary>
    public void Ticker (){
        //health.setCurrentHealth(health.getCurrentHealth() - 60);
        RpcDealDamage(20);
        SetMovementSpeed(GetMovementSpeed() * 1.2f);
    }

}
