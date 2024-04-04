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

	private int _findEmptySlot(){
        for (int i = 0; i < 3;i++)
            if(_weapons[i] == null)
                return i;

        return -1;
    }

    private void EquipWeapon(PackedScene weaponToEquip)
    {
        int index = _findEmptySlot();

		if(index == -1){
            GD.PushError("Error on weapon giving. something is wrong with the pickup, inv is full. please check inv size before adding");
            return;
        }

        var w = weaponToEquip.Instantiate<AbstractWeapon>();
        _weapons[index] = w;
        hand.AddChild(w);
        // SwitchWeaponTo(index);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
		if(!IsMultiplayerAuthority())
            return;
		
        if (@event is InputEventMouseButton mbe)
        {
            if (Input.IsPhysicalKeyPressed(Key.Ctrl))
            {
                return;
            }

            if (mbe.ButtonIndex == MouseButton.WheelUp)
            {
                ScrollUp();
            }

            if (mbe.ButtonIndex == MouseButton.WheelDown)
            {
                ScrollDown();
            }
        }
    }

    public void ScrollUp()
    {
        _currentIndex++;
        if (_currentIndex > 2)
        {
            _currentIndex = 0;
        }

        SwitchWeaponTo(_currentIndex);
    }

    public void ScrollDown()
    {
        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = 2;
        }

        SwitchWeaponTo(_currentIndex);
    }

    public void SwitchWeaponTo(int index)
    {
        _equippedWeapon?.onDisable();

        _currentIndex = index;

        _equippedWeapon = _weapons[index];
        _equippedWeapon?.onEnable();

        Rpc(nameof(RpcEquipWeapon), _currentIndex);
    }

    public bool HasSpace()
    {
        foreach (var w in _weapons)
        {
            if (w == null) return true;
        }
        return false;
    }


    public void ShootAction()
    {
        if (_equippedWeapon != null)
        {
            _equippedWeapon.ShootInput(Vector3.Zero);
        }
    }

	[Rpc(MultiplayerApi.RpcMode.Authority,CallLocal = false)]
	public void RpcEquipWeapon(int index){
        _equippedWeapon?.onDisable();
		_equippedWeapon = _weapons[index];
        _equippedWeapon?.onEnable();
    }

	[Rpc(MultiplayerApi.RpcMode.Authority,CallLocal = false)]
	public void RpcSetRemoteWeaponCache(int index,string weapon_name){
		
	}

}
