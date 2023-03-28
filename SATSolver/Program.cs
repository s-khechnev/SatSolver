using SATSolver;

if (args.Length is 0 or > 1)
    throw new ArgumentException("Wrong args");

var filePath = args[0].Replace("\"", "");

if (!File.Exists(filePath))
    throw new ArgumentException("File not exists");

var parser = new DIMACSParser();
var cnf = parser.ParseFile(filePath);
var model = Solver.SolveSat(cnf);

DIMACSParser.WriteModelToConsole(model);