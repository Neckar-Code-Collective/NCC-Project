using Godot;
using System;

/// <summary>
/// An Interactable with which shooters can buy new guns
/// </summary>
public partial class WeaponShop : Interactable
{
    /// <summary>
    /// where to spawn the weapon
    /// </summary>
    [Export]
    Node3D _weaponSpawnPos;

    /// <summary>
    /// the price of the weapon
    /// </summary>
    [Export]
    int _price;

    /// <summary>
    /// what weapon to sell
    /// </summary>
    [Export]
    string _weaponToSell = "weapon_start_weapon";

    public override void _Ready()
	{
        OnInteract += (Shooter s) =>
        {
            if(s.GetMoney() < _price){
                //not enough money
                GD.Print("Not enough money");
                return;
            }

            s.Rpc(nameof(s.RpcDeductMoney), _price);

            Global.NetworkManager.SpawnWeapon(_weaponToSell, _weaponSpawnPos.GlobalPosition);

        };
    }

}
