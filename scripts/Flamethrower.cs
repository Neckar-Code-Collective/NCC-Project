using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A weapon that deals damage in a block shape infront of it
/// </summary>
public partial class Flamethrower : AbstractWeapon
{

    [Export]
    GpuParticles3D _onParticles;

    [Export]
    SpotLight3D _onLight;

    [Export]
    GpuParticles3D _offParticles;

    [Export]
    SpotLight3D _offLight;

    [Export]
    public int Ammo = 10000;

    [Export]
    Node3D _visual;

    [Export]
    Area3D DamageArea;

    /// <summary>
    /// Whether the fire button is held down
    /// </summary>
    bool _isShooting = false;

    /// <summary>
    /// Used to track whether to stop shooting
    /// </summary>
    float _timeSinceLastShot = 0;

    /// <summary>
    /// A list of entities that are currently being damaged by the fire
    /// </summary>
    List<Enemy> _enemiesInDamageZone = new();

    /// <summary>
    /// The timer that ticks damage
    /// </summary>
    Timer _damagerticker;

    public override void _Ready()
    {
        //setup everything
        DamageArea.BodyEntered += _onDamageZoneEnter;
        DamageArea.BodyExited += _onDamageZoneLeave;

        _offParticles.Visible = true;
        _offLight.Visible = true;

        _onParticles.Emitting = false;
        _onLight.Visible = false;
    }


    public override void ShootInput(Vector3 velocity)
    {
        if(Ammo <= 0){
            return;
        }

		if(!_isShooting){
            //Starting to shoot, enable/disable emitters
            _offParticles.Visible = false;
            _offLight.Visible = false;

            _onParticles.Emitting = true;
            // _onParticles.Restart();
            _onLight.Visible = true;

            _isShooting = true;

            Rpc(nameof(RpcShoot), Vector3.Zero, Vector3.Zero, 1);
        }
        _timeSinceLastShot = 0;

        Ammo -= 1;



    }

    int ticksSinceLastDamage = 0;

    /// <summary>
    /// Apply damage to all enemies in the hitbox
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        if(!_isShooting){
            return;
        }
        _timeSinceLastShot += (float)delta;

        if(_timeSinceLastShot >= 0.1f){
			//we stopped shooting, disable all visuals
            _isShooting = false;

            _onParticles.Emitting = false;
            _onLight.Visible = false;

            _offParticles.Visible = true;
            _offLight.Visible = true;

            Rpc(nameof(RpcShoot), Vector3.Zero, Vector3.Zero, 0);
            return;
        }
        //every 10 ticks, damage entities in fire
		if(ticksSinceLastDamage >= 10){
            ticksSinceLastDamage = 0;

			foreach(var e in _enemiesInDamageZone){
                e.Rpc(nameof(e.RpcDealDamage), 3);
            }
        }
		else{
            ticksSinceLastDamage++;
        }
    }

    

	void _onDamageZoneEnter(Node3D other){
		if(other is Enemy e){
            _enemiesInDamageZone.Add(e);
        }
	}

	void _onDamageZoneLeave(Node3D other){
		if(other is Enemy e){
            _enemiesInDamageZone.Remove(e);
        }
	}

    public override void onDisable()
    {
        _visual.Visible = false;
        
    }
    /// <summary>
    /// Set all visual parts to visible
    /// </summary>
    public override void onEnable()
    {
        _visual.Visible = true;
        _onParticles.Restart();
        _onParticles.Emitting = false;
        _offParticles.Restart();
    }

    /// <summary>
    /// Update the visual representation of the flamethrower
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="vel"></param>
    /// <param name="data"></param>
    public override void RpcShoot(Vector3 pos, Vector3 vel, int data)
    {
        if(data == 0){
            //stop shooting
            _onLight.Visible = false;
            _onParticles.Emitting = false;

            _offLight.Visible = true;
            _offParticles.Emitting = true;
        }
		else{
            _onLight.Visible = true;
            _onParticles.Emitting = true;

            _offLight.Visible = false;
            _offParticles.Visible = false;
        }
    }
}
