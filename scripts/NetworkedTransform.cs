using Godot;
using System;

public partial class NetworkedTransform : Node
{
    Node3D target;

    Timer updateTimer;

    [Export(PropertyHint.Range,"0,2")]
    float updateInterval = 0.05f;

    Vector3 targetPosition;

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

        Rpc(nameof(RPCUpdatePosition), target.GlobalPosition);

    }

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void RPCUpdatePosition(Vector3 pos){
        targetPosition = pos;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(IsMultiplayerAuthority()){
            return;
        }

		if(target == null){
            return;
        }

        target.GlobalPosition.Lerp(targetPosition, 0.8f);
    }
}
