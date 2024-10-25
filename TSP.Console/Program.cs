using TSP.Console.Files;

string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
string filePath = Path.Combine(projectDirectory, "Input", "berlin52.tsp");

TSPLIBData data = TSPLIBImporter.Import(filePath);

Console.WriteLine(data);