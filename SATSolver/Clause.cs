using System.Text;

namespace SATSolver;

public class Clause : ICloneable
{
    public HashSet<Literal> Literals { get; }

    public Clause(IEnumerable<Literal> literals)
    {
        Literals = new HashSet<Literal>(literals);
    }

    public bool IsEmpty => Literals.Count == 0;

    public bool IsUnitClause => Literals.Count == 1;

    public Literal NotAssigned => Literals.Single();

    public object Clone() => new Clause(Literals);

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
}