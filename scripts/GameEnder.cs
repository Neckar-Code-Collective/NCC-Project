using Godot;
using System;

/// <summary>
/// Represents the circle where the skulls need to be placed
/// </summary>
public partial class GameEnder : Area3D
{

	/// <summary>
    /// how many skulls are needed to finish the game
    /// </summary>
    [Export]
    int _skullsNeeded = 2;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if(!IsMultiplayerAuthority())
            return;

        AreaEntered += OnEnter;
        AreaExited += OnLeave;
    }

	/// <summary>
    /// how many skulls are touching us
    /// </summary>
    int _currentlyTouching = 0;

    /// <summary>
    /// checks whether there are enough skulls in the circle
    /// </summary>
    /// <returns></returns>
	public bool IsFinished(){
        return _currentlyTouching >= _skullsNeeded;
    }

    void OnEnter(Node3D other){
		if(other is WorldItem i){
			if(i.GetWeaponName() == "weapon_skull"){
                _currentlyTouching++;
            }
		}

		if(IsFinished()){
            Global.NetworkManager.Rpc(nameof(Global.NetworkManager.RPCEndGameShootersWin));
        }
	}

	void OnLeave(Node3D other){
		if(other is WorldItem i){
			if(i.GetWeaponName() == "weapon_skull"){
                _currentlyTouching--;
            }
		}
	}
}
