using Godot;
using System;

/// <summary>
/// Networked Transform replicates the position of a gameobject to all other peers
/// </summary>
public partial class NetworkedTransform : Node
{
    /// <summary>
    /// The node this NT replicates
    /// </summary>
    Node3D _target;

    /// <summary>
    /// The timer which calls the replication function
    /// </summary>
    Timer _updateTimer;

    /// <summary>
    /// The interval in which this NT replicates the position
    /// </summary>
    [Export(PropertyHint.Range,"0,2")]
    float _updateInterval = 0.035f;

    /// <summary>
    /// Used on the client. Is the current target position that we are lerping to.
    /// </summary>
    Vector3 _targetPosition;

    /// <summary>
    /// Used on the client. Is the current target rotation that we are lerping to.
    /// </summary>
    Vector3 _targetRotation;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //Setup timer
        _updateTimer = new Timer
        {
            OneShot = false
        };
        AddChild(_updateTimer);
        _updateTimer.Timeout += OnTimer;
        _updateTimer.Start(_updateInterval);

        _targetPosition = _target.GlobalPosition;

    }

	public void SetTarget(Node3D t){
        _target = t;
    }

    //Called when the timer fires. Calls the Replication RPC on all other clients
	public void OnTimer(){
        //in case we are not the authority, we dont want to call the rpc
		if(!IsMultiplayerAuthority()){
            return;
        }
        //in case no target is set, we cancel so we dont create errors
		if(_target == null){
            GD.PushWarning("target is null");
            return;
        }

        //replicates _targets position and rotation to all other peers
        Rpc(nameof(RPCUpdatePosition), _target.GlobalPosition,_target.GlobalRotationDegrees);

    }

    /// <summary>
    /// RPC. Gets called by the owner on all other peers to broadcast the new position and rotation
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotY"></param>
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void RPCUpdatePosition(Vector3 pos,Vector3 rot){
        _targetPosition = pos;
        _targetRotation = rot;
    }

	// In case we arent the authority, we lerp to the current target position and rotation
	public override void _PhysicsProcess(double delta)
	{

        
		if(IsMultiplayerAuthority()){
            // GD.Print(Multiplayer.GetUniqueId());
            return;
        }

		if(_target == null){
            return;
        }

        _target.GlobalPosition = _target.GlobalPosition.Lerp(_targetPosition, 0.6f);
        _target.GlobalRotationDegrees = _target.GlobalRotationDegrees.Lerp(_targetRotation,0.6f);
    }
}
