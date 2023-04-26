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

    private readonly string _cnfWithPureLiterals_1_m2_m3 = string.Join('\n',
        "p cnf 3 1",
        "1 -2 -3 0"
    );

    private readonly string _cnfWithPureLiterals_1_m2 = string.Join('\n',
        "p cnf 3 3",
        "1 -2 -3 0",
        "1 -2 3 0",
        "1 -2 -3 0"
    );

    private readonly string _cnfWithPureLiterals_m2 = string.Join('\n',
        "p cnf 3 3",
        "1 -2 -3 0",
        "-1 -2 3 0",
        "1 -2 -3 0"
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
            cnf = cnf.AssignLiteral(notAssigned, notAssigned > 0);
        }

        var expectedStr = string.Join('\n', "p cnf 2 1", "-1 2");
        var expectedCnf = _parser.ParseText(expectedStr);

        Assert.That(_cnfComparer.Equals(cnf, expectedCnf), Is.True);
    }

    [Test]
    public void PureLiteralsTest1()
    {
        var cnf = _parser.ParseText(_cnfWithPureLiterals_1_m2_m3);

        var expected = new List<int> { 1, -2, -3 }.OrderBy(i => i);
        var actual = cnf.PureLiterals.OrderBy(i => i);

        var isEqual = expected.SequenceEqual(actual);
        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void PureLiteralsTest2()
    {
        var cnf = _parser.ParseText(_cnfWithPureLiterals_1_m2);

        var expected = new List<int> { 1, -2, }.OrderBy(i => i);
        var actual = cnf.PureLiterals.OrderBy(i => i);

        var isEqual = expected.SequenceEqual(actual);
        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void PureLiteralsTest3()
    {
        var cnf = _parser.ParseText(_cnfWithPureLiterals_m2);

        var expected = new List<int> { -2, }.OrderBy(i => i);
        var actual = cnf.PureLiterals.OrderBy(i => i);

        var isEqual = expected.SequenceEqual(actual);
        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void GetPureLiteralTest()
    {
        var cnf = _parser.ParseText(_cnfWithPureLiteral);

        var expected = -3;
        var actual = cnf.PureLiteral;

        var isEqual = expected == actual;
        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void PureLiteralsEliminationTest()
    {
        var cnf = _parser.ParseText(_cnfWithPureLiteral);

        while (true)
        {
            var pureLiteral = cnf.PureLiteral;

            if (pureLiteral == 0)
                break;

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

        var x1 = 1;
        cnf = cnf.AssignLiteral(x1, true);

        var expectedStr = string.Join('\n', "p cnf 3 1", "-2 3 0");
        var expectedCnf = _parser.ParseText(expectedStr);

        Assert.That(_cnfComparer.Equals(cnf, expectedCnf), Is.True);
    }
}