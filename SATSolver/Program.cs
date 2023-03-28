using SATSolver;

var filePath = Console.ReadLine();

if (string.IsNullOrEmpty(filePath))
    throw new ArgumentException("Empty file path");

filePath = filePath.Replace("\"", "");

if (!File.Exists(filePath))
    throw new ArgumentException("File not exists");

var parser = new DIMACSParser();
var cnf = parser.ParseFile(filePath);
var model = Solver.SolveSat(cnf);

DIMACSParser.WriteModelToConsole(model);