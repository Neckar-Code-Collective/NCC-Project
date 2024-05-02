using Godot;
using System.Collections.Generic;
using System;


/// <summary>
/// An enemy that spits fire, works the same as the flamethrower
/// </summary>
public partial class HydraEnemy : BasicEnemy{

	float _movementSpeed = 1.5f;
	[Export]
    GpuParticles3D _onParticles;

    [Export]
    SpotLight3D _onLight;

    [Export]
    GpuParticles3D _offParticles;

    [Export]
    SpotLight3D _offLight;

    [Export]
    Node3D _visual;

    [Export]
    Area3D DamageArea;

	public float ATTACKRANGE = 20.0f;

	List<Shooter> shootersInDamageZone = new();

    Timer _damagerticker;

	private Shooter _shooter;

	float _timeSinceLastShot;




	public override void _Ready()
	{
		base._Ready();
		SetMovementSpeed(_movementSpeed);
		DamageArea.BodyEntered += _onDamageZoneEnter;
        DamageArea.BodyExited += _onDamageZoneLeave;

        _offParticles.Visible = true;
        _offLight.Visible = true;

        _onParticles.Emitting = false;
        _onLight.Visible = false;


		
	}

	int ticksSinceLastDamage;
    public override void _PhysicsProcess(double delta)
    {
		base._PhysicsProcess(delta);
		_timeSinceLastShot += (float)delta;

		
		if(_timeSinceLastShot >= 2.0f && _timeSinceLastShot < 6.0f)
		{
			_onParticles.Emitting = true;
			_onLight.Visible = true;

			if(ticksSinceLastDamage >= 25)
			{
				ticksSinceLastDamage = 0;

				foreach(var s in shootersInDamageZone){
					s.Rpc(nameof(s.RpcDealDamage), 1);
				}
			}
			else{
				ticksSinceLastDamage++;
			}

		}
		else if(_timeSinceLastShot >= 6.0f)
		{
			_timeSinceLastShot = 0;
		}
		else
		{
			_onParticles.Emitting = false;
			_onLight.Visible = false;
		}
		
    }
	

	void _onDamageZoneEnter(Node3D other){
		if(other is Shooter s)
		{
            shootersInDamageZone.Add(s);
        }
	}

	void _onDamageZoneLeave(Node3D other){
		if(other is Shooter s)
		{
            shootersInDamageZone.Remove(s);
        }
	}

    public void onDisable()
    {
        _visual.Visible = false;
        
    }

    public void onEnable()
    {
        _visual.Visible = true;
        _onParticles.Restart();
        _onParticles.Emitting = false;
        _offParticles.Restart();
    }




}

	



