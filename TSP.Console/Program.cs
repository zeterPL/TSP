using TSP.Console.Common.Extensions;
using TSP.Console.Files;
using TSP.Console.Interfaces;
using TSP.Console.TSPSolver;

string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
string filePath = Path.Combine(projectDirectory, "Input", "berlin52.tsp");

TSPLIBData data = TSPLIBImporter.Import(filePath);

Console.WriteLine(data);

ITSPSolver solver = new TSPSolver();

double[,] distanceMatrix = solver.CalculateDistanceMatrix(data.Nodes);

distanceMatrix.PrintMatrix();