using SATSolver;

namespace Tests;

public class SolverTest
{
    private DIMACSParser _parser;

    private readonly string _cnfSat1 = string.Join('\n',
        "c SAT",
        "p cnf 8 5",
        "4 -5 3 0",
        "-5 -2 -4 0",
        "8 -1 2 0",
        "-3 5 6 0",
        "2 1 8 0"
    );

    [SetUp]
    public void Setup()
    {
        _parser = new();
    }

    [Test]
    public void SatTest()
    {
        var cnfSat = _parser.ParseText(_cnfSat1); 
        var someModel = Solver.SolveSat(cnfSat);
        var isSat = someModel != null;
        Assert.That(isSat, Is.True);
    }
    
    [Test]
    public void CorrectModelTest()
    {
        var cnfSat = _parser.ParseText(_cnfSat1); 
        var someModel = Solver.SolveSat(cnfSat);

        cnfSat = someModel.Aggregate(cnfSat, (current, litValue) => current.InsertValueToLiteral(litValue.Item1, litValue.Item2));
        var isEmpty = cnfSat.IsEmpty;
        
        Assert.That(isEmpty, Is.True);
    }
}