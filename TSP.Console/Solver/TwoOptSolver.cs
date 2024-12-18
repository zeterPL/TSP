using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Console.Solver
{
    public static class TwoOptSolver
    {
        /// <summary>
        /// Rozwiązuje problem TSP przy użyciu heurystyki 2-opt.
        /// </summary>
        /// <param name="distanceMatrix">Macierz odległości między miastami.</param>
        /// <returns>Najlepsza znaleziona trasa i jej długość.</returns>
        public static Chromosome Solve(double[,] distanceMatrix)
        {
            int numberOfCities = distanceMatrix.GetLength(0);

            int[] initialRoute = Enumerable.Range(0, numberOfCities).ToArray();
            Random random = new Random();
            initialRoute = initialRoute.OrderBy(x => random.Next()).ToArray();

            return TwoOptImprove(initialRoute, distanceMatrix);
        }

        /// <summary>
        /// Optymalizuje trasę przy użyciu heurystyki 2-opt.
        /// </summary>
        private static Chromosome TwoOptImprove(int[] route, double[,] distanceMatrix)
        {
            bool improvement = true;

            while (improvement)
            {
                improvement = false;
                for (int i = 0; i < route.Length - 1; i++)
                {
                    for (int j = i + 1; j < route.Length; j++)
                    {
                        double oldDistance = CalcSegmentDistance(route, distanceMatrix, i, i + 1) +
                                             CalcSegmentDistance(route, distanceMatrix, j, (j + 1) % route.Length);

                        ReverseSegment(route, i + 1, j);

                        double newDistance = CalcSegmentDistance(route, distanceMatrix, i, i + 1) +
                                             CalcSegmentDistance(route, distanceMatrix, j, (j + 1) % route.Length);

                        if (newDistance < oldDistance)
                        {
                            improvement = true;
                        }
                        else
                        {
                            ReverseSegment(route, i + 1, j); // Przywróć trasę
                        }
                    }
                }
            }
            return new Chromosome(route, distanceMatrix);
        }

        /// <summary>
        /// Odwraca segment trasy.
        /// </summary>
        private static void ReverseSegment(int[] route, int start, int end)
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
        /// Oblicza odległość między dwoma segmentami w trasie.
        /// </summary>
        private static double CalcSegmentDistance(int[] route, double[,] distanceMatrix, int from, int to)
        {
            return distanceMatrix[route[from], route[to]];
        }
    }
}
