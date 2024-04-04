using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponReg : Node
{
    static Dictionary<string, PackedScene> _equipedWeapons = new();
    static Dictionary<string, PackedScene> _worldWeapons = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _equipedWeapons.Add("weapon_start_weapon", GD.Load<PackedScene>("res://weapon.tscn"));

        _worldWeapons.Add("weapon_start_weapon", GD.Load<PackedScene>("res://worldweaponprefabs/BasicWeaponWorldItem.tscn"));
    }

	public static PackedScene GetEquipedWeapon(string name){
        return _equipedWeapons[name];
    }

	public static PackedScene GetWorldWeapon(string name){
        return _worldWeapons[name];
    }
}
