﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Console.Common.Enums;

namespace TSP.Console.Solver
{
    /// <summary>
    /// Rozwiązuje problem komiwojażera (TSP) za pomocą algorytmu genetycznego.
    /// </summary>
    public class GeneticTSPSolver
    {
        /// <summary>
        /// Macierz odległości.
        /// </summary>
        private readonly double[,] distanceMatrix;

        /// <summary>
        /// Liczba miast.
        /// </summary>
        private readonly int numberOfCities;

        /// <summary>
        /// Rozmiar populacji.
        /// </summary>
        private readonly int populationSize;

        /// <summary>
        /// Prawdopodobieństwo mutacji
        /// </summary>
        private readonly double mutationRate;

        /// <summary>
        /// Prawdopodobieństwo krzyżowania
        /// </summary>
        private readonly double crossoverRate;

        /// <summary>
        /// Maksymalna liczba generacji
        /// </summary>
        private readonly int maxGenerations;

        /// <summary>
        /// Random
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// Wybrana metoda krzyżowania
        /// </summary>
        public CrossoverMethodEnum CrossoverMethod { get; private set; }
        
        /// <summary>
        /// Wybrana heurystyka w memetycznym algorytmie
        /// </summary>
        public HeuristicMethodEnum? HeuristicMethod { get; private set; }

        /// <summary>
        /// Wyświetlaj informacje w Solve() jak np. nr generacji czy czas ukończenia
        /// </summary>
        private readonly bool debug;

        /// <summary>
        /// Inicjalizuje solver algorytmu genetycznego dla TSP.
        /// </summary>
        /// <param name="distanceMatrix">Macierz odległości między miastami.</param>
        /// <param name="populationSize">Rozmiar populacji.</param>
        /// <param name="mutationRate">Prawdopodobieństwo mutacji.</param>
        /// <param name="crossoverRate">Prawdopodobieństwo krzyżowania.</param>
        /// <param name="maxGenerations">Maksymalna liczba generacji.</param>
        public GeneticTSPSolver(
            double[,] distanceMatrix,
            int populationSize = 100,
            double mutationRate = 0.05,
            double crossoverRate = 0.9,
            int maxGenerations = 1000,
            CrossoverMethodEnum crossoverMethod = CrossoverMethodEnum.PMX,
            HeuristicMethodEnum? heuristicMethod = HeuristicMethodEnum.LK,
            bool debug = true
            )
        {
            this.distanceMatrix = distanceMatrix;
            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            this.maxGenerations = maxGenerations;
            this.numberOfCities = distanceMatrix.GetLength(0);
            this.CrossoverMethod = crossoverMethod;
            this.HeuristicMethod = heuristicMethod;
            this.debug = debug;
        }

        /// <summary>
        /// Uruchamia algorytm genetyczny w celu znalezienia przybliżonego rozwiązania TSP.
        /// </summary>
        /// <returns>Najlepszy znaleziony chromosom (trasę) wraz z długością.</returns>
        public Chromosome Solve()
        {
            List<Chromosome> population = InitializePopulation();
            Chromosome bestSolution = null;
            var stopwatch = new System.Diagnostics.Stopwatch();

            string debugFilePath = "debug_results.txt";
            if (debug && File.Exists(debugFilePath))
            {
                File.Delete(debugFilePath); // Usunięcie starego pliku
            }

            for (int gen = 0; gen < maxGenerations; gen++)
            {
                stopwatch.Restart(); // Start measuring time for the generation
                var currentBest = GetBest(population);

                // Sprawdzenie, czy pojawił się nowy najlepszy wynik
                bool isNewBest = false;
                if (bestSolution == null || currentBest.Distance < bestSolution.Distance)
                {
                    bestSolution = currentBest;
                    isNewBest = true;
                }

                if (debug)
                {
                    // Wyświetlenie logów z kolorami
                    if (isNewBest)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Green; // Zielony dla nowego najlepszego wyniku
                        System.Console.WriteLine($"[Generation {gen}] New Best Distance: {bestSolution.Distance:F2}");
                    }
                    else
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red; // Czerwony, jeśli brak poprawy
                        System.Console.WriteLine($"[Generation {gen}] No improvement. Current Best: {bestSolution.Distance:F2}");
                    }

                    File.AppendAllText(debugFilePath, $"{currentBest.Distance:F2}\n");
                }

                // Resetowanie koloru na domyślny
                System.Console.ResetColor();

                var newPopulation = new List<Chromosome>();

                int processedOffspring = 0; // Track progress in the generation
                while (newPopulation.Count < populationSize)
                {
                    var parent1 = TournamentSelection(population);
                    var parent2 = TournamentSelection(population);

                    Chromosome offspring;

                    if (random.NextDouble() < crossoverRate)
                    {
                        offspring = CrossoverMethod switch
                        {
                            CrossoverMethodEnum.PMX => PMX(parent1, parent2),
                            CrossoverMethodEnum.OX => OX(parent1, parent2),
                            CrossoverMethodEnum.EX => EX(parent1, parent2),
                            _ => throw new NotImplementedException($"Crossover method {CrossoverMethod} not implemented")
                        };
                    }
                    else
                    {
                        offspring = parent1.Clone();
                    }

                    offspring = Mutate(offspring);

                    // Log co 10-tego potomka dla optymalizacji
                    //if (processedOffspring % 10 == 0)
                    //{
                    //    System.Console.WriteLine($"  [Generation {gen}] Optimizing offspring {processedOffspring + 1}/{populationSize} with 3-opt...");
                    //}

                    offspring = HeuristicMethod switch
                    {
                        HeuristicMethodEnum.OPT2 => TwoOptImprove(offspring),
                        HeuristicMethodEnum.OPT3 => ThreeOptImprove(offspring),
                        _ => offspring
                    };

                    newPopulation.Add(offspring);
                    processedOffspring++;
                }

                population = newPopulation;

                stopwatch.Stop(); // Measure time for the generation
                if (debug)
                    System.Console.WriteLine($"[Generation {gen}] Completed in {stopwatch.ElapsedMilliseconds} ms.");
            }

            return bestSolution;
        }

        #region Helpers

        /// <summary>
        /// Wypisuje metrukę algorytmu zawietającą informację o parametrach konfiguracyjnych
        /// </summary>
        public void PrintAlgorithmMetrics(string problemInstance)
        {
            System.Console.WriteLine("=== Genetic TSP Algorithm ===");
            System.Console.ForegroundColor = ConsoleColor.Cyan;

            System.Console.WriteLine($"Problem Instance:       {problemInstance}");
            System.Console.WriteLine($"Number of Cities:       {numberOfCities}");
            System.Console.WriteLine($"Population Size:        {populationSize}");
            System.Console.WriteLine($"Max Generations:        {maxGenerations}");
            System.Console.WriteLine($"Crossover Rate:         {crossoverRate:P}");
            System.Console.WriteLine($"Mutation Rate:          {mutationRate:P}");
            System.Console.WriteLine($"Crossover Method:       {CrossoverMethod}");
            System.Console.WriteLine($"Heuristic Method:       {HeuristicMethod.ToString()}");

            System.Console.ResetColor();
            System.Console.WriteLine("=================================");
        }


        /// <summary>
        /// Inicjalizuje populację losowych chromosomów.
        /// </summary>
        /// <returns>Lista chromosomów stanowiących populację początkową.</returns>
        private List<Chromosome> InitializePopulation()
        {
            var population = new List<Chromosome>();
            for (int i = 0; i < populationSize; i++)
            {
                var c = new Chromosome(numberOfCities, distanceMatrix);
                population.Add(c);
            }
            return population;
        }

        /// <summary>
        /// Zwraca najlepszego osobnika z populacji.
        /// </summary>
        /// <param name="population">Lista chromosomów.</param>
        /// <returns>Chromosom o minimalnej długości trasy.</returns>
        private Chromosome GetBest(List<Chromosome> population)
        {
            return population.Aggregate((best, current) => current.Distance < best.Distance ? current : best);
        }

        /// <summary>
        /// Wybiera rodzica do krzyżowania metodą turniejową.
        /// </summary>
        /// <param name="population">Populacja chromosomów.</param>
        /// <param name="tournamentSize">Rozmiar turnieju (liczba losowo wybranych osobników).</param>
        /// <returns>Najlepszy osobnik wybrany spośród turnieju.</returns>
        private Chromosome TournamentSelection(List<Chromosome> population, int tournamentSize = 5)
        {
            Chromosome best = null;
            for (int i = 0; i < tournamentSize; i++)
            {
                var idx = random.Next(population.Count);
                var candidate = population[idx];
                if (best == null || candidate.Distance < best.Distance)
                    best = candidate;
            }
            return best;
        }

        /// <summary>
        /// Operator krzyżowania PMX (Partially Mapped Crossover).
        /// </summary>
        private Chromosome PMX(Chromosome parent1, Chromosome parent2)
        {
            int length = parent1.Route.Length;
            int[] child = new int[length];
            for (int i = 0; i < length; i++)
                child[i] = -1;

            int start = random.Next(length);
            int end = random.Next(length);
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            // Kopiujemy segment z parent1 do dziecka
            for (int i = start; i <= end; i++)
            {
                child[i] = parent1.Route[i];
            }

            // PMX mapping
            for (int i = start; i <= end; i++)
            {
                int gene = parent2.Route[i];
                if (!child.Contains(gene))
                {
                    int pos = i;
                    int mappedGene = parent1.Route[pos];

                    while (child[pos] != -1)
                    {
                        pos = Array.IndexOf(parent2.Route, mappedGene);
                        mappedGene = parent1.Route[pos];
                    }

                    child[pos] = gene;
                }
            }

            for (int i = 0; i < length; i++)
            {
                if (child[i] == -1)
                {
                    child[i] = parent2.Route[i];
                }
            }

            return new Chromosome(child, distanceMatrix);
        }

        /// <summary>
        /// Operator krzyżowania OX (Order Crossover).
        /// </summary>
        private Chromosome OX(Chromosome parent1, Chromosome parent2)
        {
            int length = parent1.Route.Length;
            int[] child = new int[length];
            for (int i = 0; i < length; i++)
                child[i] = -1;

            int start = random.Next(length);
            int end = random.Next(length);
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            // Kopiujemy segment z parent1 do dziecka
            for (int i = start; i <= end; i++)
            {
                child[i] = parent1.Route[i];
            }

            // Uzupełniamy pozostałe geny zgodnie z kolejnością z parent2
            int currentIndex = (end + 1) % length;
            for (int i = 0; i < length; i++)
            {
                int gene = parent2.Route[(end + 1 + i) % length];
                if (!child.Contains(gene))
                {
                    child[currentIndex] = gene;
                    currentIndex = (currentIndex + 1) % length;
                }
            }

            return new Chromosome(child, distanceMatrix);
        }

        /// <summary>
        /// Operator krzyżowania EX (Edge Crossover)
        /// </summary>
        private Chromosome EX(Chromosome parent1, Chromosome parent2)
        {
            int length = parent1.Route.Length;
            // Build the edge map
            Dictionary<int, HashSet<int>> edgeMap = new Dictionary<int, HashSet<int>>();
            BuildEdgeMap(parent1.Route, edgeMap);
            BuildEdgeMap(parent2.Route, edgeMap);
            // Start with a random city
            Random rnd = new Random();
            int currentCity = rnd.Next(length);
            HashSet<int> visited = new HashSet<int>();
            List<int> childRoute = new List<int>();
            // Create the child route
            while (visited.Count < length)
            {
                childRoute.Add(currentCity);
                visited.Add(currentCity);
                // Remove this city from all adjacency lists
                if (edgeMap.ContainsKey(currentCity))
                {
                    foreach (var key in edgeMap.Keys)
                    {
                        edgeMap[key].Remove(currentCity);
                    }
                }
                // Select the next city
                if (edgeMap.ContainsKey(currentCity) && edgeMap[currentCity].Count > 0)
                {
                    currentCity = edgeMap[currentCity].OrderBy(x => edgeMap[x].Count).First();
                }
                else
                {
                    // Pick an unvisited city randomly
                    currentCity = visited.Count < length ? Enumerable.Range(0, length).FirstOrDefault(c => !visited.Contains(c)) : -1;
                    if (currentCity == -1) break; // Safeguard against infinite loop
                }
            }
            // Validate the child route completion
            if (childRoute.Count < length)
            {
                var remainingCities = Enumerable.Range(0, length).Where(c => !visited.Contains(c)).ToList();
                childRoute.AddRange(remainingCities);
            }
            return new Chromosome(childRoute.ToArray(), distanceMatrix);
        }

        /// <summary>
        /// Funkcja pomocnicza dla EX (EdgeCrossover)
        /// </summary>
        private void BuildEdgeMap(int[] route, Dictionary<int, HashSet<int>> edgeMap)
        {
            int length = route.Length;
            for (int i = 0; i < length; i++)
            {
                int city = route[i];
                if (!edgeMap.ContainsKey(city))
                    edgeMap[city] = new HashSet<int>();

                // Add edges if not already present
                int prev = route[(i - 1 + length) % length];
                int next = route[(i + 1) % length];
                edgeMap[city].Add(prev);
                edgeMap[city].Add(next);
            }
        }

        /// <summary>
        /// Operator mutacji zamieniający miejscami dwa losowo wybrane miasta w trasie.
        /// </summary>
        private Chromosome Mutate(Chromosome chromosome)
        {
            if (random.NextDouble() < mutationRate)
            {
                int[] route = (int[])chromosome.Route.Clone();
                int a = random.Next(route.Length);
                int b = random.Next(route.Length);

                int temp = route[a];
                route[a] = route[b];
                route[b] = temp;

                return new Chromosome(route, distanceMatrix);
            }

            return chromosome;
        }

        /// <summary>
        /// Próbuje poprawić rozwiązanie za pomocą operatora 3-opt.
        /// </summary>
        private Chromosome ThreeOptImprove(Chromosome chromosome)
        {
            int[] route = (int[])chromosome.Route.Clone();
            bool improvement = true;
            int maxIterations = 20;
            int iteration = 0;

            double bestDistance = chromosome.Distance;
            var distanceCache = new Dictionary<(int, int), double>();

            while (improvement && iteration < maxIterations)
            {
                improvement = false;
                iteration++;
                for (int i = 0; i < numberOfCities - 2; i++)
                {
                    for (int j = i + 1; j < numberOfCities - 1; j++)
                    {
                        for (int k = j + 1; k < numberOfCities; k++)
                        {
                            double oldDist = GetCachedDistance(distanceCache, route, i, i + 1) +
                                             GetCachedDistance(distanceCache, route, j, j + 1) +
                                             GetCachedDistance(distanceCache, route, k, (k + 1) % numberOfCities);

                            for (int moveType = 1; moveType <= 4; moveType++)
                            {
                                double gain = Try3OptMove(route, i, j, k, moveType);
                                if (gain < 0)
                                {
                                    bestDistance += gain;
                                    improvement = true;
                                    break;
                                }
                            }
                            if (improvement) break;
                        }
                        if (improvement) break;
                    }
                    if (improvement) break;
                }
            }
            return new Chromosome(route, distanceMatrix);
        }

        private double GetCachedDistance(Dictionary<(int, int), double> cache, int[] route, int from, int to)
        {
            var key = (route[from], route[to]);
            if (!cache.TryGetValue(key, out var distance))
            {
                distance = CalcDistanceOfRouteSegment(route, from, to);
                cache[key] = distance;
            }
            return distance;
        }

        /// <summary>
        /// Przeprowadza próbę zastosowania ruchu 3-opt i zwraca "zysk" (ujemny, jeśli poprawiono).
        /// Parametr moveType określa wariant odwracania segmentów.
        /// </summary>
        private double Try3OptMove(int[] route, int i, int j, int k, int moveType)
        {
            // Oblicz dystans starych krawędzi
            double oldDist = CalcDistanceOfRouteSegment(route, i, i + 1) +
                             CalcDistanceOfRouteSegment(route, j, j + 1) +
                             CalcDistanceOfRouteSegment(route, k, (k + 1) % route.Length);

            // Modyfikujemy trasę zgodnie z typem ruchu
            switch (moveType)
            {
                case 1:
                    ReverseSegment(route, i + 1, j); // Odwracamy (i+1, j)
                    break;
                case 2:
                    ReverseSegment(route, j + 1, k); // Odwracamy (j+1, k)
                    break;
                case 3:
                    ReverseSegment(route, i + 1, j); // Odwracamy (i+1, j)
                    ReverseSegment(route, j + 1, k); // Odwracamy (j+1, k)
                    break;
                case 4:
                    ReverseSegment(route, i + 1, k); // Odwracamy (i+1, k)
                    break;
                default:
                    return 0; // Niepoprawny typ ruchu
            }

            // Oblicz dystans nowych krawędzi
            double newDist = CalcDistanceOfRouteSegment(route, i, i + 1) +
                             CalcDistanceOfRouteSegment(route, j, j + 1) +
                             CalcDistanceOfRouteSegment(route, k, (k + 1) % route.Length);

            double gain = newDist - oldDist;

            // Cofamy zmiany, jeśli nie poprawiono wyniku
            if (gain >= 0)
            {
                // Przywróć pierwotną trasę
                switch (moveType)
                {
                    case 1:
                        ReverseSegment(route, i + 1, j); // Przywróć (i+1, j)
                        break;
                    case 2:
                        ReverseSegment(route, j + 1, k); // Przywróć (j+1, k)
                        break;
                    case 3:
                        ReverseSegment(route, j + 1, k); // Przywróć (j+1, k)
                        ReverseSegment(route, i + 1, j); // Przywróć (i+1, j)
                        break;
                    case 4:
                        ReverseSegment(route, i + 1, k); // Przywróć (i+1, k)
                        break;
                }
            }

            return gain;
        }

        /// <summary>
        /// Oblicza dystans dla segmentu między dwoma miastami.
        /// </summary>
        private double CalcDistanceOfRouteSegment(int[] route, int start, int end)
        {
            return distanceMatrix[route[start], route[end]];
        }

        /// <summary>
        /// Odwraca segment tablicy route od start do end.
        /// </summary>
        private void ReverseSegment(int[] route, int start, int end)
        {
            while (start < end)
            {
                int temp = route[start];
                route[start] = route[end];
                route[end] = temp;
                start++;
                end--;
            }
        }

        /// <summary>
        /// Liczy całkowitą odległość trasy.
        /// </summary>
        private double CalcTotalDistance(int[] route)
        {
            double dist = 0;
            for (int i = 0; i < route.Length; i++)
            {
                int next = (i + 1) % route.Length;
                dist += distanceMatrix[route[i], route[next]];
            }
            return dist;
        }


        /// <summary>
        /// Improves a TSP solution using the Kernighan–Lin heuristic.
        /// </summary>
        private Chromosome TwoOptImprove(Chromosome chromosome)
        {
            int[] route = (int[])chromosome.Route.Clone();
            double bestDistance = chromosome.Distance;
            bool improvement = true;

            while (improvement)
            {
                improvement = false;
                // Iterate through all possible pairs of edges
                for (int i = 0; i < numberOfCities - 1; i++)
                {
                    for (int j = i + 2; j < numberOfCities; j++)
                    {
                        // Ensure edges don't overlap (no wrap-around issues)
                        if (i == 0 && j == numberOfCities - 1) continue;

                        // Calculate the change in distance if we swap edges (i, i+1) and (j, j+1)
                        double delta = CalculateSwapGain(route, i, j);
                        if (delta < 0) // Only perform the swap if it improves the distance
                        {
                            ReverseSegment(route, i + 1, j);
                            bestDistance += delta;
                            improvement = true;
                        }
                    }
                }
            }
            return new Chromosome(route, distanceMatrix);
        }

        /// <summary>
        /// Calculates the gain (change in distance) for swapping edges.
        /// </summary>
        private double CalculateSwapGain(int[] route, int i, int j)
        {
            int a = route[i];
            int b = route[i + 1];
            int c = route[j];
            int d = route[(j + 1) % numberOfCities];
            // Calculate the difference in edge lengths
            double currentCost = distanceMatrix[a, b] + distanceMatrix[c, d];
            double newCost = distanceMatrix[a, c] + distanceMatrix[b, d];
            return newCost - currentCost;
        }
        #endregion
    }
}
