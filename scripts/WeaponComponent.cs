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

        // if (StartingWeapon != null)
            // EquipWeapon(StartingWeapon);
    }

    /// <summary>
    /// Returns the next empty slot
    /// </summary>
    /// <returns>the index of the slot or -1 if there is no space</returns>
	private int _findEmptySlot(){
        for (int i = 0; i < 3;i++)
            if(_weapons[i] == null)
                return i;

        return -1;
    }

    /// <summary>
    /// Adds the weapon to the inventory
    /// </summary>
    /// <param name="name">The weapon to add</param>
    public void EquipWeapon(string name)
    {
        if(!IsMultiplayerAuthority()){
            GD.PushWarning("Tried to equip weapon on not local player");
            return;
        }

        int index = _findEmptySlot();

		if(index == -1){
            GD.PushError("Error on weapon giving. something is wrong with the pickup, inv is full. please check inv size before adding");
            return;
        }

        var w = WeaponReg.GetEquipedWeapon(name).Instantiate<AbstractWeapon>();
        _weapons[index] = w;
        hand.AddChild(w,true);
        Rpc(nameof(RpcSetRemoteWeaponCache), index, name,w.Name);

        SwitchWeaponTo(index);
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
        if(@event is InputEventKey key){
            if(key.Keycode == Key.Key1){
                ScrollDown();
            }
            if(key.Keycode == Key.Key2){
                ScrollUp();
            }
        }

        if(@event is InputEventKey ik && ik.Pressed){
            if(ik.Keycode == Key.G){
                DropCurrentWeapon();
            }
        }
    }

    /// <summary>
    /// Drop the current weapon onto the ground and remove it from the inventory
    /// </summary>
    public void DropCurrentWeapon()
    {
        if(_equippedWeapon == null){
            return;
        }

        var name = _equippedWeapon.GetWeaponName();
        _equippedWeapon.QueueFree();
        _equippedWeapon = null;
        _weapons[_currentIndex] = null;

        Rpc(nameof(RpcSetRemoteWeaponCache), _currentIndex, "null","irrelevant");
        Rpc(nameof(RpcEquipWeapon), _currentIndex);

        var s = GetParent<Node3D>();
        Global.NetworkManager.Rpc(nameof(Global.NetworkManager.RpcSpawnWeapon),name,s.GlobalPosition+-s.GlobalBasis.Z*2.5f);

    }

    /// <summary>
    /// Scroll the weapon selection up
    /// </summary>
    public void ScrollUp()
    {
        _currentIndex++;
        if (_currentIndex > 2)
        {
            _currentIndex = 0;
        }

        SwitchWeaponTo(_currentIndex);
    }

    /// <summary>
    /// Scroll the weapon selection down
    /// </summary>
    public void ScrollDown()
    {
        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = 2;
        }

        SwitchWeaponTo(_currentIndex);
    }

    /// <summary>
    /// Switches to the weapon in the slot index
    /// </summary>
    /// <param name="index">the index to equip</param>
    public void SwitchWeaponTo(int index)
    {
        _equippedWeapon?.onDisable();

        _currentIndex = index;

        _equippedWeapon = _weapons[index];
        _equippedWeapon?.onEnable();

        Rpc(nameof(RpcEquipWeapon), _currentIndex);
    }

    /// <summary>
    /// Checks if there is space in this inventory
    /// </summary>
    /// <returns>returns true if space is in available, false otherwise</returns>
    public bool HasSpace()
    {
        foreach (var w in _weapons)
        {
            if (w == null) return true;
        }
        return false;
    }


    /// <summary>
    /// Gets called by the shooter class when input is received
    /// </summary>
    public void ShootAction()
    {
        if (_equippedWeapon != null)
        {
            _equippedWeapon.ShootInput(Vector3.Zero);
        }
    }

    /// <summary>
    /// Gets called on the other clients to select which weapon to show
    /// </summary>
    /// <param name="index"></param>
	[Rpc(MultiplayerApi.RpcMode.Authority,CallLocal = false)]
	public void RpcEquipWeapon(int index){
        if(_equippedWeapon != null && IsInstanceValid(_equippedWeapon)){
            _equippedWeapon.onDisable();
        }
		_equippedWeapon = _weapons[index];

        if(_equippedWeapon != null && IsInstanceValid(_equippedWeapon)){
            _equippedWeapon.onEnable();
        }
        
    }

    /// <summary>
    /// Sets the weapons on other peers
    /// </summary>
    /// <param name="index">the index to set</param>
    /// <param name="weapon_name">the weapon to equip, or null to unequip the weapon</param>
    /// <param name="nodeName">the name to give to the new node</param>
	[Rpc(MultiplayerApi.RpcMode.Authority,CallLocal = false)]
	public void RpcSetRemoteWeaponCache(int index,string weapon_name,string nodeName){
        if(weapon_name == "null"){
            _weapons[index]?.QueueFree();
            _weapons[index] = null;
            return;
        }

        _weapons[index] = WeaponReg.GetEquipedWeapon(weapon_name).Instantiate<AbstractWeapon>();
        hand.AddChild(_weapons[index]);
        _weapons[index].Name = nodeName;
        _weapons[index].SetMultiplayerAuthority(GetMultiplayerAuthority());
    }

}
