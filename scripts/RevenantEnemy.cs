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
        health.setMaxHealth(100f);
        health.setCurrentHealth(health.getMaxHealth());
        setMovementSpeed(1.0f);
        setNetWorth(15);

        if(health.getCurrentHealth() <= 5){
            switch(state){
                case RevenantEnemyState.ALIVE:
                    state = RevenantEnemyState.REVENANT;
                    timer.Start(2);
                    health.setCurrentHealth(120f);
                    setMovementSpeed(3.0f);
                    break;
                case RevenantEnemyState.REVENANT:
                    break;
            }
        }

        

    }

   public void Ticker (){
    while(health.getCurrentHealth() > 0){
            break;
        }

   }






}






