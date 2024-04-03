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
	private AbstractWeapon _equippedWeapon;

    private AbstractWeapon[] _weapons;
    int _currentIndex = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _weapons = new AbstractWeapon[3];
        hand = GetParent().FindChild("Hand", true, false) as Marker3D;

		if (StartingWeapon != null)
			EquipWeapon(StartingWeapon);
	}

	private void EquipWeapon(PackedScene weaponToEquip)
	{
		if (_equippedWeapon != null)
		{
			GD.Print("Deleting current weapon");
			_equippedWeapon.QueueFree();
		}
		else
		{
			GD.Print("No weapon equipped ");
			_equippedWeapon = weaponToEquip.Instantiate<AbstractWeapon>();
			hand.AddChild(_equippedWeapon);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseButton mbe){
			if(Input.IsPhysicalKeyPressed(Key.Ctrl)){
                return;
            }

			if(mbe.ButtonIndex == MouseButton.WheelUp){

			}

			if(mbe.ButtonIndex == MouseButton.WheelDown){

			}
		}
    }

	public void ScrollUp(){
        _currentIndex++;
		if(_currentIndex >= 2){
            _currentIndex = 0;
        }

        SwitchWeaponTo(_currentIndex);
    }

	public void ScrollDown(){
        _currentIndex--;
		if(_currentIndex < 0){
            _currentIndex = 2;
        }

        SwitchWeaponTo(_currentIndex);
    }

	public void SwitchWeaponTo(int index){
		
	}
    

	public void ShootAction(){
		if(_equippedWeapon != null){
            _equippedWeapon.ShootInput(Vector3.Zero);
        }
	}

}
