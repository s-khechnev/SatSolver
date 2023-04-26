using System.Text;

namespace SATSolver;

public class AbsComparer : IEqualityComparer<int>
{
    public bool Equals(int x, int y) => Math.Abs(x) == Math.Abs(y);

    public int GetHashCode(int obj) => Math.Abs(obj);
}

public class CNF
{
    public readonly HashSet<Clause> Clauses;

    public readonly HashSet<int> PureLiterals;
    private readonly HashSet<int> _vars;

    public CNF(IEnumerable<Clause> clauses)
    {
        Clauses = new HashSet<Clause>();
        _vars = new HashSet<int>(new AbsComparer());
        PureLiterals = new HashSet<int>();

        foreach (var clause in clauses)
        {
            Clauses.Add(clause);
            foreach (var literal in clause.Literals)
            {
                if (PureLiterals.Contains(-literal))
                {
                    PureLiterals.Remove(-literal);
                }
                else if (!_vars.Contains(literal))
                {
                    PureLiterals.Add(literal);
                }

                _vars.Add(literal);
            }
        }
    }

    public int CountVars => _vars.Count;

    public Clause? UnitClause => Clauses.FirstOrDefault(clause => clause.IsUnitClause);

    public int PureLiteral => PureLiterals.FirstOrDefault();

    public bool IsEmpty => Clauses.Count == 0;

    public bool HasEmptyClause => Clauses.Any(clause => clause.IsEmpty);

    public CNF EliminatePureLiteral(int literal)
    {
        Clauses.RemoveWhere(clause => clause.Literals.Contains(literal));
        return new(Clauses);
    }

    public int GetLiteral() => _vars.First();

    public CNF AssignLiteral(int literal, bool value)
    {
        if (literal > 0 == value)
        {
            Clauses.RemoveWhere(clause => clause.Literals.Contains(literal));
            foreach (var clause in Clauses)
            {
                clause.Literals.Remove(-literal);
            }
        }
        else
        {
            Clauses.RemoveWhere(clause => clause.Literals.Contains(-literal));
            foreach (var clause in Clauses)
            {
                clause.Literals.Remove(literal);
            }
        }

        return new(Clauses);
    }

    public CNF Clone()
    {
        return new CNF(Clauses.Select(clause => new Clause(clause.Literals)));
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        const string separator = " /\\ ";
        foreach (var clause in Clauses)
        {
            builder.Append(clause);
            builder.Append(separator);
        }

        builder.Length -= separator.Length;
        return builder.ToString();
    }

    public class CNFComparer : IEqualityComparer<CNF>
    {
        private Clause.ClauseComparer _clauseComparer;

        public CNFComparer(Clause.ClauseComparer clauseComparer)
        {
            _clauseComparer = clauseComparer;
        }

        public bool Equals(CNF? x, CNF? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            if (x._vars.Count != y._vars.Count || x.Clauses.Count != y.Clauses.Count)
                return false;

            var curLitArr = x._vars.OrderBy(lit => lit);
            var otherLitArr = y._vars.OrderBy(lit => lit).ToArray();

            if (curLitArr.Where(
                    (literal, i) => literal != otherLitArr[i]).Any())
                return false;

            var shared = x.Clauses.Intersect(y.Clauses, _clauseComparer);

            return x.Clauses.Count == y.Clauses.Count && shared.Count() == x.Clauses.Count;
        }

        public int GetHashCode(CNF obj) => HashCode.Combine(obj._vars, obj.Clauses);
    }
}