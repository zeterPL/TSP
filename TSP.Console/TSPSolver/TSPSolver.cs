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

            for (int i = 0; i < nodesCount; i++)
            {
                for (int j = 0; j < nodesCount; j++)
                {
                    if (i == j) distanceMatrix[i, j] = 0;
                    else
                    {
                        distanceMatrix[i, j] = CalculateEuclidesDistance(nodes[i], nodes[j]);
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
