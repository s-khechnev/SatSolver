namespace SATSolver;

public static class Solver
{
    private static readonly Dictionary<(int, bool), List<(int, bool)>> Storage = new();
    private static readonly (int, bool) DummyLiteral = new(0, true);

    public static List<(int, bool)>? SolveSat(CNF cnf)
    {
        var copy = new CNF(cnf.Clauses);
        if (!DPLL(copy, DummyLiteral)) return null;

        var model = new List<(int, bool)>();

        model.AddRange(Storage[DummyLiteral]);
        Storage.Remove(DummyLiteral);

        foreach (var pair in Storage)
        {
            model.Add(pair.Key);
            model.AddRange(pair.Value);
        }

        if (model.Count != cnf.CountVars)
            model = InitModel(model, cnf.CountVars);

        return model.OrderBy(item => Math.Abs(item.Item1)).ToList();
    }

    private static List<(int, bool)> InitModel(List<(int, bool)> model, int literalsCount)
    {
        var arr = new (int, bool)[literalsCount];
        foreach (var literalValue in model)
        {
            arr[Math.Abs(literalValue.Item1) - 1] = literalValue;
        }

        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i].Item1 != 0)
                continue;

            arr[i] = (i + 1, true);
        }

        return arr.ToList();
    }

    private static bool DPLL(CNF cnf, (int, bool) selectedLiteral)
    {
        Storage[selectedLiteral] = new List<(int, bool)>();

        while (cnf.UnitClause is { } unitClause)
        {
            var notAssigned = unitClause.NotAssigned;
            cnf = cnf.AssignLiteral(notAssigned, notAssigned > 0);

            Storage[selectedLiteral].Add((notAssigned, notAssigned > 0));
        }

        while (true)
        {
            var pureLiteral = cnf.PureLiteral;

            if (pureLiteral == 0)
                break;

            cnf = cnf.EliminatePureLiteral(pureLiteral);

            Storage[selectedLiteral].Add((pureLiteral, pureLiteral > 0));
        }

        if (cnf.IsEmpty)
            return true;

        if (cnf.HasEmptyClause)
            return false;

        var randomLiteral = cnf.GetLiteral();
        var copyCnf = cnf.Clone();

        var isTrueBranch = DPLL(copyCnf.AssignLiteral(randomLiteral, true), (randomLiteral, true));

        var isFalseBranch = false;
        if (!isTrueBranch)
        {
            isFalseBranch = DPLL(cnf.AssignLiteral(randomLiteral, false), (randomLiteral, false));

            Storage.Remove((randomLiteral, true));
        }

        if (!isFalseBranch)
            Storage.Remove((randomLiteral, false));

        return isTrueBranch || isFalseBranch;
    }
}