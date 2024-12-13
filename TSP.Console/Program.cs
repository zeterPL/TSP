using TSP.Console.Common.Enums;
using TSP.Console.Common.Extensions;
using TSP.Console.Files;
using TSP.Console.Solver;
using TSP.Console.TSPSolver;

string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
string filePath = Path.Combine(projectDirectory, "Input", "berlin52.tsp");

TSPLIBData data = TSPLIBImporter.Import(filePath);

//Console.WriteLine(data);

double[,] distanceMatrix = Helpers.CalculateDistanceMatrix(data.Nodes);

//distanceMatrix.PrintMatrix(10);

GeneticTSPSolver gaSolver = new GeneticTSPSolver(
            distanceMatrix: distanceMatrix,
            populationSize: 100,
            mutationRate: 0.05,
            crossoverRate: 0.9,
            maxGenerations: 1000
            CrossoverMethodEnum.PMX       
        );

Chromosome bestSolution = gaSolver.Solve();

Console.WriteLine("Najlepsze rozwiązanie:");
Console.WriteLine(bestSolution);