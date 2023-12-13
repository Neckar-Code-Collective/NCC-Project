using Godot;
using System;

public partial class Shooter : Node
{
	//TODO WeaponManager weapons;
	//TODO MoneyManager money;
	bool isLocalPlayer = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



	[Rpc]
	public void RpcSetActiveWeapon(int weaponIndex){

	}

	public virtual void Shoot(){}
}
