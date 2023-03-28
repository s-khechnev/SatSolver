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

            var literals = new List<Literal>();

            foreach (var literal in line.Split(_separator))
            {
                if (literal == EndChar)
                    break;

                bool sign;
                int literalIndex;
                if (literal[0] == '-')
                {
                    sign = false;
                    literalIndex = Convert.ToInt32(literal[1..]);
                }
                else
                {
                    sign = true;
                    literalIndex = Convert.ToInt32(literal);
                }

                if (literalIndex > countVars)
                    throw new ArgumentException("Too large variable index");

                literals.Add(new Literal(sign, literalIndex));
            }

            clauses.Add(new Clause(literals));
            readLines++;
        }

        return new CNF(clauses, countVars);
    }

    public static void WriteModelToConsole(List<(Literal, bool)>? model)
    {
        if (model == null)
        {
            Console.WriteLine(UNSAT);
            return;
        }

        var builder = new StringBuilder(SAT);
        builder.Append('\n');
        builder.Append(AnswerStartChar);
        builder.Append(' ');

        foreach (var literalValue in model)
        {
            builder.Append(literalValue.Item2 ? literalValue.Item1.Index.ToString() : $"-{literalValue.Item1.Index}");
            builder.Append(' ');
        }

        builder.Append(EndChar);
        Console.WriteLine(builder.ToString());
    }
}