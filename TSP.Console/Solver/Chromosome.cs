using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Console.Solver
{
    /// <summary>
    /// Reprezentacja rozwiązania problemu TSP w postaci chromosomu – permutacji wierzchołków.
    /// </summary>
    public class Chromosome
    {
        /// <summary>
        /// Kolejność odwiedzanych miast.
        /// </summary>
        public int[] Route { get; private set; }

        /// <summary>
        /// Długość trasy przypisanej do tego chromosomu.
        /// </summary>
        public double Distance { get; private set; }

        /// <summary>
        /// Macierz odległości.
        /// </summary>
        private double[,] distanceMatrix;

        /// <summary>
        /// Inicjalizuje chromosom na podstawie podanej permutacji i macierzy odległości.
        /// </summary>
        /// <param name="route">Permutacja miast.</param>
        /// <param name="distanceMatrix">Macierz odległości między miastami.</param>
        public Chromosome(int[] route, double[,] distanceMatrix)
        {
            this.Route = route;
            this.distanceMatrix = distanceMatrix;
            this.Distance = CalculateDistance();
        }

        /// <summary>
        /// Tworzy losowy chromosom (losową trasę) dla podanej liczby miast i macierzy odległości.
        /// </summary>
        /// <param name="numberOfCities">Liczba miast.</param>
        /// <param name="distanceMatrix">Macierz odległości.</param>
        public Chromosome(int numberOfCities, double[,] distanceMatrix)
        {
            this.distanceMatrix = distanceMatrix;
            this.Route = GenerateRandomRoute(numberOfCities);
            this.Distance = CalculateDistance();
        }

        /// <summary>
        /// Tworzy kopię bieżącego chromosomu.
        /// </summary>
        /// <returns>Nowy chromosom będący kopią bieżącego.</returns>
        public Chromosome Clone()
        {
            int[] newRoute = new int[Route.Length];
            Array.Copy(Route, newRoute, Route.Length);
            return new Chromosome(newRoute, distanceMatrix);
        }

        /// <summary>
        /// Zwraca reprezentację tekstową chromosomu.
        /// </summary>
        /// <returns>Łańcuch znaków z trasą i długością.</returns>
        public override string ToString()
        {
            return $"Route: {string.Join("-", Route)}, Distance: {Distance}";
        }

        #region Helpers

        /// <summary>
        /// Oblicza długość trasy na podstawie permutacji i macierzy odległości.
        /// </summary>
        /// <returns>Długość trasy.</returns>
        private double CalculateDistance() 
        {
            double sum = 0.0;
            for (int i = 0; i < Route.Length - 1; i++)
            {
                sum += distanceMatrix[Route[i], Route[i + 1]];
            }
            sum += distanceMatrix[Route[Route.Length - 1], Route[0]];
            return sum;
        }

        /// <summary>
        /// Generuje losową trasę zawierającą wszystkie miasta.
        /// </summary>
        /// <param name="numberOfCities">Liczba miast.</param>
        /// <returns>Tablica z losową permutacją.</returns>
        private int[] GenerateRandomRoute(int numberOfCities)
        {
            var route = Enumerable.Range(0, numberOfCities).ToArray();
            var rnd = new Random();

            for (int i = route.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                int temp = route[i];
                route[i] = route[j];
                route[j] = temp;
            }
            return route;
        }


        #endregion
    }
}
