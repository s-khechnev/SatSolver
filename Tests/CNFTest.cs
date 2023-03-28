using SATSolver;

namespace Tests;

public class CNFTest
{
    private DIMACSParser _parser;
    private CNF.CNFComparer _cnfComparer;

    private readonly string _cnfWithOneUnitClause = string.Join('\n',
        "c cnf with one unit clause = (3)",
        "p cnf 3 3",
        "3 0",
        "1 -2 3 0",
        "-1 2 -3 0"
    );

    private readonly string _cnfWithPureLiteral = string.Join('\n',
        "c cnf with pure literal = 3",
        "p cnf 3 4",
        "1 -2 -3 0",
        "-1 -2 -3 0",
        "1 2 0",
        "-1 -2 0"
    );

    private readonly string _cnfForInsert = string.Join('\n',
        "p cnf 3 3",
        "1 -2 -3 0",
        "-1 -2 3 0",
        "1 2 0"
    );

    [SetUp]
    public void Setup()
    {
        _parser = new DIMACSParser();
        _cnfComparer = new CNF.CNFComparer(new Clause.ClauseComparer());
    }

    [Test]
    public void UnitPropagateTest()
    {
        var cnf = _parser.ParseText(_cnfWithOneUnitClause);

        while (cnf.UnitClause is { } unitClause)
        {
            var notAssigned = unitClause.NotAssigned;
            cnf = cnf.PropagateUnit(notAssigned);
        }

        var expectedStr = string.Join('\n', "p cnf 2 1", "-1 2");
        var expectedCnf = _parser.ParseText(expectedStr);

        Assert.That(_cnfComparer.Equals(cnf, expectedCnf), Is.True);
    }

    [Test]
    public void GetPureLiteralTest()
    {
        var cnf = _parser.ParseText(_cnfWithPureLiteral);

        var expected = new Literal(false, 3);
        var actual = cnf.PureLiteral;

        var isEqual = expected.Sign == actual.Sign && expected.Index == actual.Index;
        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void PureLiteralsEliminationTest()
    {
        var cnf = _parser.ParseText(_cnfWithPureLiteral);

        while (cnf.PureLiteral is { } pureLiteral)
        {
            cnf = cnf.EliminatePureLiteral(pureLiteral);
        }

        var expectedStr = string.Join('\n', "p cnf 2 2", "1 2 0", "-1 -2 0");
        var expectedCnf = _parser.ParseText(expectedStr);

        Assert.That(_cnfComparer.Equals(cnf, expectedCnf), Is.True);
    }

    [Test]
    public void InsertValueToLiteralTest()
    {
        var cnf = _parser.ParseText(_cnfForInsert);

        var x1 = new Literal(true, 1);
        cnf = cnf.InsertValueToLiteral(x1, true);

        var expectedStr = string.Join('\n', "p cnf 3 1", "-2 3 0");
        var expectedCnf = _parser.ParseText(expectedStr);

        Assert.That(_cnfComparer.Equals(cnf, expectedCnf), Is.True);
    }
}