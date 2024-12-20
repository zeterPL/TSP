
using TSP.Console.Common;
using TSP.Console.Files.Importer;

namespace TSP.Console.TSPSolver
{
    public static class Helpers
    {
    
        public static double[,] CalculateDistanceMatrix(List<Node> nodes, EdgeWeightTypeEnum edgeWeightType)
        {
            int nodesCount = nodes.Count;
            double[,] distanceMatrix = new double[nodesCount, nodesCount];

            foreach (var city1 in nodes)
            {
                foreach (var city2 in nodes)
                {
                    int index1 = city1.Id - 1;
                    int index2 = city2.Id - 1;

                    if (index1 == index2) distanceMatrix[index1, index2] = int.MaxValue;

                    else
                    {
                        distanceMatrix[index1, index2] = CalculateEuclidesDistance(city1, city2);

                        distanceMatrix[index1, index2] = edgeWeightType switch
                        {
                            EdgeWeightTypeEnum.EUC_2D => CalculateEuclidesDistance(nodes[index1], nodes[index2]),
                            EdgeWeightTypeEnum.GEO => CalculateGeoDistance(nodes[index1], nodes[index2]),
                            EdgeWeightTypeEnum.ATT => CalculateAttDistance(nodes[index1], nodes[index2]),
                            EdgeWeightTypeEnum.CEIL_2D => CalculateCeil2DDistance(nodes[index1], nodes[index2]),
                            _ => throw new NotSupportedException($"Edge weight type {edgeWeightType} not supported.")
                        };
                    }
                }
            }

            return distanceMatrix;
        }

        #region Helpers

        private static double CalculateEuclidesDistance(Node node1, Node node2) 
        {
            double dx = node1.X - node2.X;
            double dy = node1.Y - node2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static double CalculateCeil2DDistance(Node node1, Node node2)
        {
            double dx = node1.X - node2.X;
            double dy = node1.Y - node2.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return Math.Ceiling(distance); // Round up to the nearest integer
        }


        private static double CalculateAttDistance(Node node1, Node node2)
        {
            double xd = node1.X - node2.X;
            double yd = node1.Y - node2.Y;
            double rij = Math.Sqrt((xd * xd + yd * yd) / 10.0);
            return Math.Ceiling(rij); // Round up to the nearest integer
        }

        private static double CalculateGeoDistance(Node node1, Node node2)
        {
            const double R = 6371; // Earth's radius in kilometers
            double lat1 = DegreesToRadians(node1.X);
            double lon1 = DegreesToRadians(node1.Y);
            double lat2 = DegreesToRadians(node2.X);
            double lon2 = DegreesToRadians(node2.Y);

            double dlat = lat2 - lat1;
            double dlon = lon2 - lon1;

            double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dlon / 2) * Math.Sin(dlon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in kilometers
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        #endregion
    }
}
