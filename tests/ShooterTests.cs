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
		_shooter = _runner.Scene().GetNode<Shooter>("Shooter");
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
            await _runner.SimulateFrames(5,100);
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

	//JoystickInput
	[TestCase]
    public void TestJoystickRightMovement()
    {
        _shooter.SetSimulatedJoystickInput(1, 0); // Simulate right
        Vector3 direction = _shooter.GetJoystickInputDirection();
        Assertions.AssertVec3(direction).IsEqual(new Vector3(1, 0, 0)); 
    }

    [TestCase]
    public void TestJoystickLeftMovement()
    {
        _shooter.SetSimulatedJoystickInput(-1, 0); // Simulate left
        Vector3 direction = _shooter.GetJoystickInputDirection();
        Assertions.AssertVec3(direction).IsEqual(new Vector3(-1, 0, 0)); 
    }

    [TestCase]
    public void TestJoystickUpMovement()
    {
        _shooter.SetSimulatedJoystickInput(0, -1); // Simulate up
        Vector3 direction = _shooter.GetJoystickInputDirection();
        Assertions.AssertVec3(direction).IsEqual(new Vector3(0, 0, -1)); 
    }

    [TestCase]
    public void TestJoystickDownMovement()
    {
        _shooter.SetSimulatedJoystickInput(0, 1); // Simulate down
        Vector3 direction = _shooter.GetJoystickInputDirection();
        Assertions.AssertVec3(direction).IsEqual(new Vector3(0, 0, 1)); 
    }

	/*
	[TestCase]
	public async Task TestMouseRotation()
	{
		
	}

	[TestCase]
	public async Task TestJoystickRotation()
	{
		
	}

	// Weitere Testmethoden...
	*/
}
