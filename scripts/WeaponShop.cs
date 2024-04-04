using Godot;
using System;

public partial class WeaponShop : Interactable
{
    [Export]
    Node3D _weaponSpawnPos;

    [Export]
    int _price;

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
