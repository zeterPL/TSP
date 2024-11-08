using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Console.Files.Importer;

namespace TSP.Console.Interfaces
{
    public interface ITSPSolver
    {
        double[,] CalculateDistanceMatrix(List<Node> nodes);

        void SolveTSP(double[,] distanceMatrix);
    }
}
