using Godot;
using System;

/// <summary>
/// WeaponComponent handles the weapon selection for the shooter
/// </summary>
public partial class WeaponComponent : Node
{
	/// <summary>
    /// Prefab of the starter weapon
    /// </summary>
	[Export] public PackedScene StartingWeapon;

	/// <summary>
    /// Hand position. Basically just the position where the gun model gets spawned
    /// </summary>
	private Marker3D hand;

	/// <summary>
    /// The currently equipped weapon
    /// </summary>
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
