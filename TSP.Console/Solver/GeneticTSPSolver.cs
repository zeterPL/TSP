using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private readonly Random random = new Random();

        /// <summary>
        /// Inicjalizuje solver algorytmu genetycznego dla TSP.
        /// </summary>
        /// <param name="distanceMatrix">Macierz odległości między miastami.</param>
        /// <param name="populationSize">Rozmiar populacji.</param>
        /// <param name="mutationRate">Prawdopodobieństwo mutacji.</param>
        /// <param name="crossoverRate">Prawdopodobieństwo krzyżowania.</param>
        /// <param name="maxGenerations">Maksymalna liczba generacji.</param>
        public GeneticTSPSolver(double[,] distanceMatrix, int populationSize = 100, double mutationRate = 0.05, double crossoverRate = 0.9, int maxGenerations = 1000)
        {
            this.distanceMatrix = distanceMatrix;
            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            this.maxGenerations = maxGenerations;
            this.numberOfCities = distanceMatrix.GetLength(0);
        }

        /// <summary>
        /// Uruchamia algorytm genetyczny w celu znalezienia przybliżonego rozwiązania TSP.
        /// </summary>
        /// <returns>Najlepszy znaleziony chromosom (trasę) wraz z długością.</returns>
        public Chromosome Solve()
        {
            List<Chromosome> population = InitializePopulation();

            Chromosome bestSolution = null;

            for (int gen = 0; gen < maxGenerations; gen++)
            {
                var currentBest = GetBest(population);

                if (bestSolution == null || currentBest.Distance < bestSolution.Distance)
                {
                    bestSolution = currentBest;
                    System.Console.WriteLine($"Generation {gen}, Best Distance: {bestSolution.Distance}");
                }

                var newPopulation = new List<Chromosome>();

                while (newPopulation.Count < populationSize)
                {
                    var parent1 = TournamentSelection(population);
                    var parent2 = TournamentSelection(population);

                    Chromosome offspring;

                    if (random.NextDouble() < crossoverRate)
                    {
                        // Losowanie operatora krzyżowania
                        offspring = random.Next(2) == 0 ? PMX(parent1, parent2) : OX(parent1, parent2);
                    }
                    else
                    {
                        offspring = parent1.Clone();
                    }

                    offspring = Mutate(offspring);

                    // Dodanie lokalnej optymalizacji 3-opt
                    offspring = ThreeOptImprove(offspring);

                    newPopulation.Add(offspring);
                }

                population = newPopulation;
            }

            return bestSolution;
        }

        #region Helpers

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

            double bestDistance = chromosome.Distance;
            while (improvement)
            {
                improvement = false;
                // Przeszukujemy wszystkie trójki krawędzi
                for (int i = 0; i < numberOfCities - 2; i++)
                {
                    for (int j = i + 1; j < numberOfCities - 1; j++)
                    {
                        for (int k = j + 1; k < numberOfCities; k++)
                        {
                            // Próbujemy wszystkie warianty 3-opt
                            // Segmenty: (i,j), (j,k)
                            double oldDist = CalcDistanceOfRouteSegment(route, i, i + 1) +
                                             CalcDistanceOfRouteSegment(route, j, j + 1) +
                                             CalcDistanceOfRouteSegment(route, k, (k + 1) % numberOfCities);

                            // Cztery możliwości 3-opt (skrótowo prezentowane)
                            // 1) odwrócić segment (i+1 do j)
                            double gain = Try3OptMove(route, i, j, k, 1);
                            if (gain < 0)
                            {
                                bestDistance += gain;
                                improvement = true;
                                break;
                            }

                            // 2) odwrócić segment (j+1 do k)
                            gain = Try3OptMove(route, i, j, k, 2);
                            if (gain < 0)
                            {
                                bestDistance += gain;
                                improvement = true;
                                break;
                            }

                            // 3) odwrócić oba segmenty
                            gain = Try3OptMove(route, i, j, k, 3);
                            if (gain < 0)
                            {
                                bestDistance += gain;
                                improvement = true;
                                break;
                            }

                            // 4) inna rekoneksja 3-opt
                            gain = Try3OptMove(route, i, j, k, 4);
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
            }
            return new Chromosome(route, distanceMatrix);
        }

        /// <summary>
        /// Oblicza odległość krawędzi między route[pos] a route[nextPos], biorąc pod uwagę wrap-around.
        /// </summary>
        private double CalcDistanceOfRouteSegment(int[] route, int posA, int posB)
        {
            return distanceMatrix[route[posA], route[posB]];
        }

        /// <summary>
        /// Przeprowadza próbę zastosowania ruchu 3-opt i zwraca "zysk" (ujemny, jeśli poprawiono).
        /// Parametr moveType określa wariant odwracania segmentów.
        /// </summary>
        private double Try3OptMove(int[] route, int i, int j, int k, int moveType)
        {
            // Kopia oryginalnej trasy
            int[] newRoute = (int[])route.Clone();
            switch (moveType)
            {
                case 1:
                    // Odwracamy segment (i+1, j)
                    ReverseSegment(newRoute, i + 1, j);
                    break;
                case 2:
                    // Odwracamy segment (j+1, k)
                    ReverseSegment(newRoute, j + 1, k);
                    break;
                case 3:
                    // Odwracamy segment (i+1, j) i (j+1, k)
                    ReverseSegment(newRoute, i + 1, j);
                    ReverseSegment(newRoute, j + 1, k);
                    break;
                case 4:
                    // Odwracamy segment (i+1, k)
                    ReverseSegment(newRoute, i + 1, k);
                    break;
                default:
                    break;
            }

            double newDist = CalcTotalDistance(newRoute);
            double oldDist = CalcTotalDistance(route);
            double gain = newDist - oldDist;

            if (gain < 0)
            {
                // Jeśli zysk jest ujemny (czyli lepsza trasa), to aktualizujemy oryginał
                Array.Copy(newRoute, route, route.Length);
            }

            return gain;
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

        #endregion

    }
}
