using TSP.Console.Files.Importer;

namespace TSP.Console.GraphGenerator
{
    public static class GraphGenerator
    {
        /// <summary>
        /// Generuje losową instancję TSP z podaną liczbą miast i zakresem współrzędnych.
        /// </summary>
        /// <param name="numberOfCities">Liczba miast.</param>
        /// <param name="coordinateRange">Zakres współrzędnych (np. 100 oznacza współrzędne w przedziale [0,100]).</param>
        /// <returns>Lista węzłów (miast) z ich współrzędnymi.</returns>
        public static List<Node> GenerateRandomCities(int numberOfCities, int coordinateRange = 100)
        {
            Random random = new Random();
            var cities = new List<Node>();

            for (int i = 1; i <= numberOfCities; i++)
            {
                cities.Add(new Node
                {
                    Id = i,
                    X = random.NextDouble() * coordinateRange,
                    Y = random.NextDouble() * coordinateRange
                });
            }

            return cities;
        }
    }
}
