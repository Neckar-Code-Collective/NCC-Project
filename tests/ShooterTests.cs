using GdUnit4;
using Godot;
using System.Threading.Tasks;
using System.Threading;

[TestSuite]
public class ShooterTests
{
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private ISceneRunner _runner;
    private Shooter _shooter;

    [Before]
    public async Task SetUp()
    {
        _runner = ISceneRunner.Load("res://level.tscn");
        await _runner.AwaitIdleFrame();

        _shooter = GD.Load<PackedScene>("res://Shooter.tscn").Instantiate<Shooter>();
        _shooter.Name = "1";
        _runner.Scene().AddChild(_shooter);
        _shooter.GlobalPosition = new Vector3(10000, 0, 10000);

        await _runner.AwaitIdleFrame();
    }

    [TestCase]
    public async Task TestWMovement()
    {
        // Test for W
        await TestMovement(Key.W, Vector3.Forward);

    }

    [TestCase]
    public async Task TestAMovement()
    {
        // Test for A
        await TestMovement(Key.A, Vector3.Left);
    }

    [TestCase]
    public async Task TestSMovement()
    {
        // Test for S
        await TestMovement(Key.S, Vector3.Back);

    }

    [TestCase]
    public async Task TestDMovement()
    {
        // Test for D
        await TestMovement(Key.D, Vector3.Right);

    }


    private async Task TestMovement(Key key, Vector3 expectedDirection)   //KeyboardInput
    {
        await _semaphore.WaitAsync(); // wait for semaphore

        try
        {
            Vector3 initialPosition = _shooter.GlobalTransform.Origin;

            _runner.SimulateKeyPress(key);
            await _runner.SimulateFrames(5, 100);
            _runner.SimulateKeyRelease(key);
            await _runner.AwaitIdleFrame();

            Vector3 newPosition = _shooter.GlobalTransform.Origin;
            Vector3 movementDirection = (newPosition - initialPosition).Normalized();

            Assertions.AssertVec3(movementDirection).IsEqual(expectedDirection);
        }
        finally
        {
            _semaphore.Release(); // release the semaphore when task is done
        }
    }




    [TestCase]
    public async Task TestMouseRotation()
    {

        await _semaphore.WaitAsync();


        //test values
        Vector2[] mousePositions = { new(100, 100), new(-220, 100), new(100, -200) };
        Vector3[] targetPositions = { new(0f,0,1f), new(0f,0f,-1f), new(0f,0,-1f) };
        try
        {
            _shooter.GlobalPosition = new Vector3();
            for (int i = 0; i < mousePositions.Length; i++)
            {

                var mp = mousePositions[i];
                var tp = targetPositions[i];

                // set mouse on start position and simulate rotation
                _runner.SetMousePos(new Vector2(0,0));
                await _runner.SimulateFrames(40, 20);
                _runner.SimulateMouseMove(mp);
                await _runner.SimulateFrames(40, 20);




                var newRotation = _shooter.GetLookDirection();
                Assertions.AssertVec3(newRotation).IsEqualApprox(tp,new Vector3(0.1f,0.1f,0.1f));
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
