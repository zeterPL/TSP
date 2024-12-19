using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using TSP.Console.Common.Enums;
using TSP.Console.Files;
using TSP.Console.GraphGenerator;
using TSP.Console.Solver;
using TSP.Console.TSPSolver;

var rootCommand = new RootCommand("Program for solving the Traveling Salesman Problem (TSP).")
{
    new Option<string>(
        "--input-file",
        description: "\nPath to the TSPLIB file with data. If not provided, a random graph is generated.\n"),
    new Option<int>(
        "--cities",
        () => 50,
        description: "\nNumber of cities for the random graph generator (default: 50).\n"),
    new Option<int>(
        "--range",
        () => 100,
        description: "\nCoordinate range for the random graph generator (default: 100).\n"),
    new Option<string>(
        "--solver",
        () => "GA",
        description: "\nSolution method: 'GA' for the Genetic Algorithm, '2OPT' for the 2-opt heuristic.\n"),
    new Option<int>(
        "--population",
        () => 100,
        description: "\nPopulation size for the Genetic Algorithm (default: 100).\n"),
    new Option<int>(
        "--generations",
        () => 1000,
        description: "\nNumber of generations for the Genetic Algorithm (default: 1000).\n"),
    new Option<double>(
        "--mutation-rate",
        () => 0.05,
        description: "\nMutation rate for the Genetic Algorithm (default: 0.05).\n"),
    new Option<double>(
        "--crossover-rate",
        () => 0.9,
        description: "\nCrossover rate for the Genetic Algorithm (default: 0.9).\n"),
    new Option<string>(
        "--crossover-method",
        () => "PMX",
        description: "\nCrossover method: 'PMX' (Partially Mapped Crossover) or 'OX' (Order Crossover).\n")
};

rootCommand.Description = "\nSolving the TSP using a Genetic Algorithm or 2-opt heuristic.\n";

rootCommand.Handler = CommandHandler.Create<string, int, int, string, int, int, double, double, string>(
    (inputFile, cities, range, solver, population, generations, mutationRate, crossoverRate, crossoverMethod) =>
    {
        // Load or generate graph
        double[,] distanceMatrix;
        string problemInstance = "Random Instance";
        if (!string.IsNullOrEmpty(inputFile))
        {
            var data = TSPLIBImporter.Import(inputFile);
            distanceMatrix = Helpers.CalculateDistanceMatrix(data.Nodes);
            problemInstance = inputFile;
            Console.WriteLine($"Loaded data from file: {inputFile}");
        }
        else
        {
            var nodes = GraphGenerator.GenerateRandomCities(cities, range);
            distanceMatrix = Helpers.CalculateDistanceMatrix(nodes);
            Console.WriteLine($"Generated random graph: {cities} cities, coordinate range: {range}");
        }

        // Solve TSP using the selected method
        if (solver.ToUpper() == "GA")
        {
            var crossover = crossoverMethod.ToUpper() switch
            {
                "PMX" => CrossoverMethodEnum.PMX,
                "OX" => CrossoverMethodEnum.OX,
                "EX" => CrossoverMethodEnum.EX,
                _ => CrossoverMethodEnum.PMX // Default to PMX if the argument method is not recognized
            };
            var gaSolver = new GeneticTSPSolver(
                distanceMatrix: distanceMatrix,
                populationSize: population,
                mutationRate: mutationRate,
                crossoverRate: crossoverRate,
                maxGenerations: generations,
                crossoverMethod: crossover
            );

            gaSolver.PrintAlgorithmMetrics(problemInstance);

            Console.WriteLine("Solving using the Genetic Algorithm...");
            var bestSolution = gaSolver.Solve();
            Console.WriteLine($"Best solution (GA): {bestSolution}");
        }
        else if (solver.ToUpper() == "2OPT")
        {
            Console.WriteLine("Solving using the 2-opt heuristic...");
            var twoOptSolution = TwoOptSolver.Solve(distanceMatrix);
            Console.WriteLine($"Best solution (2-opt): {twoOptSolution}");
        }
        else
        {
            Console.WriteLine("Unknown solution method! Use 'GA' or '2OPT'.");
        }
    });

return rootCommand.Invoke(args);
