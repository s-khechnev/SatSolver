namespace SATSolver;

public static class Solver
{
    private static readonly Dictionary<(Literal, bool), List<(Literal, bool)>> Storage = new();
    private static readonly (Literal, bool) DummyLiteral = new(new Literal(true, -1), true);

    public static List<(Literal, bool)>? SolveSat(CNF cnf)
    {
        if (!DPLL(cnf, DummyLiteral)) return null;

        var model = new List<(Literal, bool)>();

        model.AddRange(Storage[DummyLiteral]);
        Storage.Remove(DummyLiteral);

        foreach (var pair in Storage)
        {
            model.Add(pair.Key);
            model.AddRange(pair.Value);
        }

        if (model.Count != cnf.CountVars)
            model = InitModel(model, cnf.CountVars);

        return model.OrderBy(item => item.Item1.Index).ToList();
    }

    private static List<(Literal, bool)> InitModel(List<(Literal, bool)> model, int literalsCount)
    {
        var arr = new (Literal, bool)[literalsCount];
        foreach (var literalValue in model)
        {
            arr[literalValue.Item1.Index - 1] = literalValue;
        }

        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i].Item1 != null)
                continue;

            arr[i] = (new Literal(true, i + 1), true);
        }

        return arr.ToList();
    }

    private static bool DPLL(CNF cnf, (Literal, bool) selectedLiteral)
    {
        Storage[selectedLiteral] = new List<(Literal, bool)>();

        while (cnf.UnitClause is { } unitClause)
        {
            var notAssigned = unitClause.NotAssigned;
            cnf = cnf.PropagateUnit(notAssigned);

            Storage[selectedLiteral].Add((notAssigned, notAssigned.Sign));
        }

        while (cnf.PureLiteral is { } pureLiteral)
        {
            cnf = cnf.EliminatePureLiteral(pureLiteral);

            Storage[selectedLiteral].Add((pureLiteral, pureLiteral.Sign));
        }

        if (cnf.IsEmpty)
            return true;

        if (cnf.HasEmptyClause)
            return false;

        var randomLiteral = cnf.GetLiteral();

        var isTrueBranch = DPLL(cnf.InsertValueToLiteral(randomLiteral, true), (randomLiteral, true));

        var isFalseBranch = false;
        if (!isTrueBranch)
        {
            isFalseBranch = DPLL(cnf.InsertValueToLiteral(randomLiteral, false), (randomLiteral, false));

            Storage.Remove((randomLiteral, true));
        }

        if (!isFalseBranch)
            Storage.Remove((randomLiteral, false));

        return isTrueBranch || isFalseBranch;
    }
}