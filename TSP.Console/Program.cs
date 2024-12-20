﻿using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.ComponentModel;
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
        description: "\nCrossover method: 'PMX' (Partially Mapped Crossover) or 'OX' (Order Crossover).\n"),
    new Option<string>(
        "--heuristic-method", 
        ()  => "LK", 
        description: "Heuristic method for the Memetic Algorithm (default: LK - Lin–Kernighan).\n"),
    new Option<bool>(
        "--debug",
        () => true,
        description: "Enable or disable debug output in GeneticTSPSolver.Solve() (default: true).\n"),
    new Option<bool>(
        "--compare",
        () => false,
        description: "Compare results of 2OPT solver and Genetic Algorithm (default: false).\n")

};

rootCommand.Description = "\nSolving the TSP using a Genetic Algorithm or 2-opt heuristic.\n";

rootCommand.Handler = CommandHandler.Create<string, int, int, string, int, int, double, double, string, string, bool, bool>(
    (inputFile, cities, range, solver, population, generations, mutationRate, crossoverRate, crossoverMethod, heuristicMethod, debug, compare) =>
    {
        // Load or generate graph
        double[,] distanceMatrix;
        string problemInstance = "Random Instance";
        if (!string.IsNullOrEmpty(inputFile))
        {
            var data = TSPLIBImporter.Import(inputFile);
            distanceMatrix = Helpers.CalculateDistanceMatrix(data.Nodes, data.EdgeWeightType);
            problemInstance = inputFile;
            Console.WriteLine($"Loaded data from file: {inputFile}");
        }
        else
        {
            var nodes = GraphGenerator.GenerateRandomCities(cities, range);
            distanceMatrix = Helpers.CalculateDistanceMatrix(nodes, EdgeWeightTypeEnum.EUC_2D);
            Console.WriteLine($"Generated random graph: {cities} cities, coordinate range: {range}");
        }

        // Solve TSP using the selected method
        void SolveWithGA()
        {
            var crossover = crossoverMethod.ToUpper() switch
            {
                "PMX" => CrossoverMethodEnum.PMX,
                "OX" => CrossoverMethodEnum.OX,
                "EX" => CrossoverMethodEnum.EX,
                _ => CrossoverMethodEnum.PMX // Default to PMX if the argument method is not recognized
            };

            var heuristic = heuristicMethod.ToUpper() switch
            {
                "LK" => HeuristicMethodEnum.LK,
                "3OPT" => HeuristicMethodEnum.OPT3,
                "2OPT" => HeuristicMethodEnum.OPT2,
                _ => HeuristicMethodEnum.LK // Default to LK if the argument heuristic is not recognized
            };

            var gaSolver = new GeneticTSPSolver(
                distanceMatrix: distanceMatrix,
                populationSize: population,
                mutationRate: mutationRate,
                crossoverRate: crossoverRate,
                maxGenerations: generations,
                crossoverMethod: crossover,
                heuristicMethod: heuristic,
                debug: debug
            );

            gaSolver.PrintAlgorithmMetrics(problemInstance);

            Console.WriteLine("Solving using the Genetic Algorithm...");
            var bestSolution = gaSolver.Solve();
            Console.WriteLine($"Best solution (GA): {bestSolution}");
        }

        void SolveWith2OPT()
        {
            Console.WriteLine("Solving using the 2-opt heuristic...");
            var twoOptSolution = TwoOptSolver.Solve(distanceMatrix);
            Console.WriteLine($"Best solution (2-opt): {twoOptSolution}");
        }

        if (compare)
        {
            Console.WriteLine("\n=== Comparing Algorithms ===");
            SolveWithGA();
            SolveWith2OPT(); 
        } else
        {
            if (solver.ToUpper() == "GA")
                SolveWithGA();
            else if (solver.ToUpper() == "2OPT")
                SolveWith2OPT();
            else
                Console.WriteLine("Unknown solution method! Use 'GA', '2OPT', or enable '--compare'.");
        }
    });

return rootCommand.Invoke(args);
