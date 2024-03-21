using Godot;
using System;


public partial class WeaponComponent : Node
{
	[Export] public PackedScene StartingWeapon;
	private Marker3D hand;
	private Node3D equippedWeapon;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hand = GetParent().FindChild("Hand", true, false) as Marker3D;

		if (StartingWeapon != null)
			EquipWeapon(StartingWeapon);
	}

	private void EquipWeapon(PackedScene weaponToEquip)
	{
		if (equippedWeapon != null)
		{
			GD.Print("Deleting current weapon");
			equippedWeapon.QueueFree();
		}
		else
		{
			GD.Print("No weapon equipped ");
			equippedWeapon = weaponToEquip.Instantiate() as Node3D;
			hand.AddChild(equippedWeapon);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ShootAction(){
		if(equippedWeapon != null){
            (equippedWeapon as BasicWeapon).ShootInput(Vector3.Zero);
        }
	}

}
