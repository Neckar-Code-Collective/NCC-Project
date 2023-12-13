using GdUnit4;
using GdUnit4.Asserts;

namespace Tests;

[TestSuite]
public class TestsTest {

    [TestCase]
    public void TestDieTests(){
        Assertions.AssertBool(true).IsEqual(true);
    }
}