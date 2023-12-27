using System.Runtime.CompilerServices;
using GdUnit4;
using GdUnit4.Asserts;
using Godot;

namespace Tests;

/*[TestSuite]
public class LevelTests {

	private level _level;
	private Player _mockPlayer;

	[Before]
	public void SetUp() {
		_level = new level();
		var mockCamera = new Camera3D();
		var mockGround = new CsgBox3D();
		_mockPlayer = new Player();

		_level.AddChild(mockCamera);
		_level.AddChild(mockGround);
		_level.AddChild(_mockPlayer);
	}

	[After]
	public void TearDown() {
		_level.Free();
	}

	[TestCase]
	public void TestProcessMouseRotation() {
		Godot.Vector2 fakeMousePosition = new Vector2(100, 100);
		_level.ProcessMouseRotation(fakeMousePosition);

		var expectedDirection = new Vector3(1, 0, 0); // Erwartete Richtung basierend auf `fakeMousePosition`
		Assertions.AssertVec3(_mockPlayer.GetLookDirection()).IsEqual(expectedDirection);
	}

	[TestCase]
	public void TestProcessJoystickRotation() {
		// Simulieren die Joystick-Eingaben
		

		_level.ProcessJoystickRotation();

		
		var expectedDirection = new Vector3(0, 0, 1); // Erwartete Richtung basierend auf simulierten Joystick-Eingaben
		Assertions.AssertVec3(_mockPlayer.GetLookDirection()).IsEqual(expectedDirection);
	}
}

	// Weitere Testfäl
    */
