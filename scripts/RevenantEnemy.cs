using Godot;
using Microsoft.CodeAnalysis;
using System;
using System.Runtime.CompilerServices;



public partial class RevenantEnemy : Enemy{

    enum RevenantEnemyState
    { 
        ALIVE, REVENANT
    }

    RevenantEnemyState state = RevenantEnemyState.ALIVE;

    Timer timer = new Timer();
    Timer ticker = new Timer();





    public override void _Ready()
    {
        base._Ready();
        AddChild(timer);
        AddChild(ticker);
        health.setMaxHealth(100f);
        health.setCurrentHealth(health.getMaxHealth());
        health.ResetDeathHandler();
        setMovementSpeed(1.0f);
        setNetWorth(15);

        health.onDeath += () =>
        {
            switch (state)
            {
                case RevenantEnemyState.ALIVE:
                    state = RevenantEnemyState.REVENANT;
                    ticker.OneShot = false;
                    timer.OneShot = true;
                    ticker.Start(2.1f);
                    ticker.Timeout += Ticker;
                    setMovementSpeed(0);
                    health.setCurrentHealth(120f);
                    timer.Timeout += () =>
                    {
                        setMovementSpeed(2.0f);
                    };
                    timer.Start(2);
                    break;
                case RevenantEnemyState.REVENANT:

                    Rpc(nameof(RpcDie));
                    //Spawn Money
                    for (int i = 0; i < NetWorth; i++)
                    {
                        var money = MoneyPrefab.Instantiate<Money>();
                        money.setMoneyAmount(1);
                        GetTree().Root.AddChild(money);
                        money.GlobalPosition = this.GlobalPosition;
                    }
                    break;
            
                }
        };





    }

    public void Ticker (){
        //health.setCurrentHealth(health.getCurrentHealth() - 60);
        RpcDealDamage(20);
        setMovementSpeed(getMovementSpeed() * 1.2f);
    }






}






