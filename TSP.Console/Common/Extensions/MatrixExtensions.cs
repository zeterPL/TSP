using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Console.Common.Extensions
{
    public static class MatrixExtensions
    {
        public static void PrintMatrix(this double[,] matrix, int? maxIndex = null)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int displayLimit = maxIndex ?? Math.Min(rows, cols);

            displayLimit = Math.Min(displayLimit, Math.Min(rows, cols));

            System.Console.Write("    ");  
            for (int i = 0; i < displayLimit; i++)
            {
                System.Console.Write($"{i + 1,8} ");  
            }
            System.Console.WriteLine();

            System.Console.WriteLine(new string('-', 8 * (displayLimit + 1)));

            for (int i = 0; i < displayLimit; i++)
            {
                System.Console.Write($"{i + 1,4} | "); 
                for (int j = 0; j < displayLimit; j++)
                {
                    System.Console.Write($"{matrix[i, j],8:F2} ");  
                }
                System.Console.WriteLine();
            }

            if (displayLimit < rows || displayLimit < cols)
            {
                System.Console.WriteLine($"... displayed only first {displayLimit} nodes.");
            }
        }
    }
}
