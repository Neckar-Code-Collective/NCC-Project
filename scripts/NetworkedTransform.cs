using Godot;
using System;

public partial class NetworkedTransform : Node
{
    Node3D target;

    Timer updateTimer;

    [Export(PropertyHint.Range,"0,2")]
    float updateInterval = 0.035f;

    Vector3 targetPosition;
    float targetRotation;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        updateTimer = new Timer
        {
            OneShot = false
        };
        AddChild(updateTimer);
        updateTimer.Timeout += OnTimer;
        updateTimer.Start(updateInterval);

        targetPosition = target.GlobalPosition;

    }

	public void SetTarget(Node3D t){
        target = t;
    }

	public void OnTimer(){
		if(!IsMultiplayerAuthority()){
            return;
        }

		if(target == null){
            GD.PushWarning("target is null");
            return;
        }

        // GD.Print(Multiplayer.GetUniqueId());
        Rpc(nameof(RPCUpdatePosition), target.GlobalPosition,target.GlobalRotationDegrees.Y);

    }

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void RPCUpdatePosition(Vector3 pos,float rotY){
        targetPosition = pos;
        targetRotation = rotY;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        if(target is Shooter){
            // GD.Print(GetMultiplayerAuthority());
        }
        
		if(IsMultiplayerAuthority()){
            // GD.Print(Multiplayer.GetUniqueId());
            return;
        }

		if(target == null){
            return;
        }

        target.GlobalPosition = target.GlobalPosition.Lerp(targetPosition, 0.6f);
        target.GlobalRotationDegrees = target.GlobalRotationDegrees.Lerp(new Vector3(0, targetRotation, 0),0.8f);
    }
}
