using System.Text;

namespace SATSolver;

public class DIMACSParser
{
    private const char CommentChar = 'c';
    private const string PCnf = "p cnf";
    private const string EndChar = "0";
    private const char DefaultSeparator = ' ';

    private readonly char _separator;

    private const string SAT = "s SATISFIABLE";
    private const string UNSAT = "s NOT SATISFIABLE";
    private const char AnswerStartChar = 'v';

    public DIMACSParser(char separator)
    {
        _separator = separator;
    }

    public DIMACSParser() : this(DefaultSeparator)
    {
    }

    public CNF ParseFile(string filePath)
    {
        var lines = File.ReadLines(filePath);
        return ParseLines(lines);
    }

    public CNF ParseText(string text)
    {
        var lines = text.Split('\n');
        return ParseLines(lines);
    }

    private CNF ParseLines(IEnumerable<string> lines)
    {
        var clauses = new List<Clause>();
        var countClauses = -1;
        var countVars = -1;
        var readLines = 0;
        foreach (var line in lines)
        {
            if (readLines == countClauses)
                break;

            if (line[0] == CommentChar)
                continue;

            if (line.StartsWith(PCnf))
            {
                var splitLine = line.Split();
                countVars = Convert.ToInt32(splitLine[2]);
                countClauses = Convert.ToInt32(splitLine[3]);
                continue;
            }

            var literals = new List<int>();

            foreach (var literalStr in line.Split(_separator))
            {
                if (literalStr == EndChar)
                    break;

                var literal = Convert.ToInt32(literalStr);

                if (literal > countVars)
                    throw new ArgumentException("Too large variable index");

                literals.Add(literal);
            }

            clauses.Add(new Clause(literals));
            readLines++;
        }

        return new CNF(clauses);
    }

    public static void WriteModel(Action<string> writer, List<(int, bool)>? model)
    {
        if (model == null)
        {
            writer.Invoke(UNSAT);
            return;
        }

        var builder = new StringBuilder(SAT);
        builder.Append('\n');
        builder.Append(AnswerStartChar);
        builder.Append(' ');

        foreach (var literalValue in model)
        {
            var abs = Math.Abs(literalValue.Item1);
            builder.Append(literalValue.Item2 ? abs : -abs);
            builder.Append(' ');
        }

        builder.Append(EndChar);
        writer.Invoke(builder.ToString());
    }
}