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

    List<Enemy> enemiesInDamageZone = new();

    Timer _damagerticker;

    public override void _Ready()
    {
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
			//we stopped shooting
            _isShooting = false;

            _onParticles.Emitting = false;
            _onLight.Visible = false;

            _offParticles.Visible = true;
            _offLight.Visible = true;

            Rpc(nameof(RpcShoot), Vector3.Zero, Vector3.Zero, 0);
            return;
        }

		if(ticksSinceLastDamage >= 10){
            ticksSinceLastDamage = 0;

			foreach(var e in enemiesInDamageZone){
                e.Rpc(nameof(e.RpcDealDamage), 3);
            }
        }
		else{
            ticksSinceLastDamage++;
        }
    }

    

	void _onDamageZoneEnter(Node3D other){
		if(other is Enemy e){
            enemiesInDamageZone.Add(e);
        }
	}

	void _onDamageZoneLeave(Node3D other){
		if(other is Enemy e){
            enemiesInDamageZone.Remove(e);
        }
	}

    public override void onDisable()
    {
        _visual.Visible = false;
        
    }

    public override void onEnable()
    {
        _visual.Visible = true;
        _onParticles.Restart();
        _onParticles.Emitting = false;
        _offParticles.Restart();
    }

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
