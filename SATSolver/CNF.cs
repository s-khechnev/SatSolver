using System.Text;

namespace SATSolver;

public class CNF
{
    public int CountVars { get; }

    private readonly HashSet<Clause> _clauses;
    private readonly HashSet<Literal> _literals;

    public CNF(IEnumerable<Clause> clauses)
    {
        _clauses = new HashSet<Clause>(clauses);
        _literals = _clauses.SelectMany(clause => clause.Literals).ToHashSet();
        CountVars = _literals.Count;
    }

    public CNF(IEnumerable<Clause> clauses, int countVars) : this(clauses)
    {
        CountVars = countVars;
    }

    public Clause? UnitClause => _clauses.FirstOrDefault(clause => clause.IsUnitClause);

    public Literal? PureLiteral => GetPureLiteral();

    public bool IsEmpty => _clauses.Count == 0;

    public bool HasEmptyClause => _clauses.Any(clause => clause.IsEmpty);

    public CNF PropagateUnit(Literal notAssigned)
    {
        var simplifiedClauses = new HashSet<Clause>(_clauses);

        foreach (var clause in _clauses)
        {
            if (clause.Literals.TryGetValue(notAssigned, out var sameLiteral))
            {
                if (notAssigned.Sign == sameLiteral.Sign)
                    simplifiedClauses.Remove(clause);
                else
                    clause.Literals.Remove(sameLiteral);
            }
        }

        return new(simplifiedClauses);
    }

    private Literal? GetPureLiteral()
    {
        foreach (var uniqueLiteral in _literals)
        {
            var isPure = true;
            foreach (var clause in _clauses)
            {
                if (clause.Literals.TryGetValue(uniqueLiteral, out var sameLiteral)
                    && sameLiteral.Sign != uniqueLiteral.Sign)
                {
                    isPure = false;
                    break;
                }
            }

            if (isPure)
                return uniqueLiteral;
        }

        return null;
    }

    public CNF EliminatePureLiteral(Literal pureLiteral)
    {
        _clauses.RemoveWhere(clause => clause.Literals.Contains(pureLiteral));
        return new(_clauses);
    }

    public Literal GetLiteral() => _literals.First();

    public CNF InsertValueToLiteral(Literal literal, bool value)
    {
        var clausesDeepClone = _clauses.Select(clause => (Clause)clause.Clone()).ToList();

        var simplifiedClauses = new HashSet<Clause>(clausesDeepClone);

        foreach (var clause in clausesDeepClone)
        {
            if (clause.Literals.TryGetValue(literal, out var sameLiteral))
            {
                if (sameLiteral.Sign == value)
                    simplifiedClauses.Remove(clause);
                else
                    clause.Literals.Remove(sameLiteral);
            }
        }

        return new(simplifiedClauses);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        const string separator = " /\\ ";
        foreach (var clause in _clauses)
        {
            builder.Append(clause);
            builder.Append(separator);
        }

        builder.Length -= separator.Length;
        return builder.ToString();
    }
}