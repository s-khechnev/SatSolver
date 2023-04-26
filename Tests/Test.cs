using SATSolver;

namespace Tests;

public class Test
{
    private DIMACSParser _parser;

    [SetUp]
    public void Setup()
    {
        _parser = new DIMACSParser();
    }

    private static IEnumerable<string> _unsatCases = new[]
    {
        "TestFiles/unsat_1_2.txt", "TestFiles/unsat_2_3.txt", "TestFiles/unsat_50_100.txt",
        "TestFiles/unsat_100_160.txt", "TestFiles/unsat_100_200.txt"
    };

    [Test]
    [TestCaseSource(nameof(_unsatCases))]
    public void Test_UnsatCase_ReturnsUnsat(string filePath)
    {
        var cnf = _parser.ParseFile(filePath);
        var result = Solver.SolveSat(cnf);
        Assert.That(result, Is.EqualTo(null));
    }

    private static IEnumerable<string> satCases = new[]
    {
        "TestFiles/example.txt", "TestFiles/examplePureLiteral.txt",
        "TestFiles/sat_20_91.txt", "TestFiles/sat_50_80.txt", "TestFiles/sat_50_80_2.txt", "TestFiles/sat_50_170.txt",
        "TestFiles/sat_200_1200.txt"
    };

    [Test]
    [TestCaseSource(nameof(satCases))]
    public void Test_SatCase_ReturnsCorrectSuit(string filePath)
    {
        var cnf = _parser.ParseFile(filePath);
        var someModel = Solver.SolveSat(cnf);

        Assert.That(someModel, Is.Not.EqualTo(null));
    }
}