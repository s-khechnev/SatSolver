﻿using System.Text;

namespace SATSolver;

public class Clause
{
    public HashSet<int> Literals { get; }

    public Clause(IEnumerable<int> literals)
    {
        Literals = new HashSet<int>(literals);
    }

    public bool IsEmpty => Literals.Count == 0;

    public bool IsUnitClause => Literals.Count == 1;

    public int NotAssigned => Literals.Single();

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append('(');

        const string separator = " \\/ ";
        foreach (var literal in Literals)
        {
            builder.Append(literal);
            builder.Append(separator);
        }

        builder.Length -= separator.Length;
        builder.Append(')');
        return builder.ToString();
    }

    public class ClauseComparer : IEqualityComparer<Clause>
    {
        public bool Equals(Clause? x, Clause? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            if (x.Literals.Count != y.Literals.Count)
                return false;

            var curLitArr = x.Literals.OrderBy(lit => lit).ToArray();
            var otherLitArr = y.Literals.OrderBy(lit => lit).ToArray();

            return !curLitArr.Where((t, i) => t != otherLitArr[i]).Any();
        }

        public int GetHashCode(Clause obj) => 1;
    }
}