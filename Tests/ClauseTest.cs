using SATSolver;

namespace Tests;

public class ClauseTest
{
    [Test]
    public void IsEmptyTrueTest()
    {
        var empty = new Clause(new List<Literal>());

        var actual = empty.IsEmpty;
        var expected = true;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void IsEmptyFalseTest()
    {
        var notEmpty = new Clause(new List<Literal> { new(true, 1) });

        var actual = notEmpty.IsEmpty;
        var expected = false;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void IsUnitClauseTrueTest()
    {
        var unit = new Clause(new List<Literal> { new(true, 1) });

        var actual = unit.IsUnitClause;
        var expected = true;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void IsUnitClauseFalseTest()
    {
        var notUnit = new Clause(new List<Literal> { new(true, 1), new(false, 2) });

        var actual = notUnit.IsUnitClause;
        var expected = false;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetNotAssignedTrueTest()
    {
        var unit = new Clause(new List<Literal> { new(true, 1) });

        var notAssigned = unit.NotAssigned;
        var expected = new Literal(true, 1);

        Assert.That(notAssigned, Is.EqualTo(expected));
    }

    [Test]
    public void GetNotAssignedExceptionTest()
    {
        var notUnit = new Clause(new List<Literal> { new(true, 1), new(true, 2) });

        Assert.Throws<InvalidOperationException>(() =>
        {
            var notAssigned = notUnit.NotAssigned;
        });
    }
}