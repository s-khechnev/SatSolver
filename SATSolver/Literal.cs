using System.Text;

namespace SATSolver;

public class Literal : ICloneable
{
    public bool Sign { get; }
    public int Index { get; }

    public Literal(bool sign, int index)
    {
        Sign = sign;
        Index = index;
    }

    public object Clone() => new Literal(Sign, Index);

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (!Sign)
            builder.Append('~');

        builder.Append($"x{Index}");
        return builder.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;

        var other = (Literal)obj;
        return Index == other.Index;
    }

    public override int GetHashCode() => Index.GetHashCode();
}