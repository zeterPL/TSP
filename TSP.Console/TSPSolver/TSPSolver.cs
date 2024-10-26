using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Console.Files.Importer;
using TSP.Console.Interfaces;

namespace TSP.Console.TSPSolver
{
    public class TSPSolver : ITSPSolver
    {
        public TSPSolver()
        {
            
        }

        public void SolveTSP(double[,] distanceMatrix)
        {
            throw new NotImplementedException();
        }

        public double[,] CalculateDistanceMatrix(List<Node> nodes)
        {
            int nodesCount = nodes.Count;
            double[,] distanceMatrix = new double[nodesCount, nodesCount];

            foreach (var city1 in nodes)
            {
                foreach (var city2 in nodes)
                {
                    int index1 = city1.Id - 1;
                    int index2 = city2.Id - 1;

                    if (index1 == index2) distanceMatrix[index1, index2] = 0; 

                    else
                    {
                        distanceMatrix[index1, index2] = CalculateEuclidesDistance(city1, city2);
                    }
                }
            }

            return distanceMatrix;
        }
   

        #region Helpers

        private double CalculateEuclidesDistance(Node node1, Node node2) 
        {
            double dx = node1.X - node2.X;
            double dy = node1.Y - node2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion
    }
}
