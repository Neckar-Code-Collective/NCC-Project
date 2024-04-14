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
        _equipedWeapons.Add("weapon_flamethrower", GD.Load<PackedScene>("res://equipableweapons/flamethrower.tscn"));
        _equipedWeapons.Add("weapon_ak47", GD.Load<PackedScene>("res://equipableweapons/ak47.tscn"));
        _equipedWeapons.Add("weapon_sniper", GD.Load<PackedScene>("res://equipableweapons/sniper.tscn"));
        _equipedWeapons.Add("weapon_crossbow", GD.Load<PackedScene>("res://equipableweapons/crossbow.tscn"));
        _equipedWeapons.Add("weapon_popcorngun", GD.Load<PackedScene>("res://equipableweapons/popcorngun.tscn"));
        _equipedWeapons.Add("weapon_sling", GD.Load<PackedScene>("res://equipableweapons/sling.tscn"));

        _worldWeapons.Add("weapon_start_weapon", GD.Load<PackedScene>("res://worldweaponprefabs/BasicWeaponWorldItem.tscn"));
        _worldWeapons.Add("weapon_flamethrower", GD.Load<PackedScene>("res://worldweaponprefabs/FlamethrowerWorldItem.tscn"));
        _worldWeapons.Add("weapon_ak47", GD.Load<PackedScene>("res://worldweaponprefabs/ak47WorldItem.tscn"));
        _worldWeapons.Add("weapon_sniper", GD.Load<PackedScene>("res://worldweaponprefabs/SniperWorldItem.tscn"));
        _worldWeapons.Add("weapon_crossbow", GD.Load<PackedScene>("res://worldweaponprefabs/CrossbowWorldItem.tscn"));
        _worldWeapons.Add("weapon_popcorngun", GD.Load<PackedScene>("res://worldweaponprefabs/PopcorngunWorldItem.tscn"));
        _worldWeapons.Add("weapon_sling", GD.Load<PackedScene>("res://worldweaponprefabs/SlingWorldItem.tscn"));
    }

	public static PackedScene GetEquipedWeapon(string name){
        return _equipedWeapons[name];
    }

	public static PackedScene GetWorldWeapon(string name){
        return _worldWeapons[name];
    }
}
