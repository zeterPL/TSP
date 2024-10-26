using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Console.Common.Extensions
{
    public static class MatrixExtensions
    {
        public static void PrintMatrix(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            System.Console.Write("    ");  
            for (int i = 0; i < cols; i++)
            {
                System.Console.Write($"{i + 1,8} ");  
            }
            System.Console.WriteLine();

            System.Console.WriteLine(new string('-', 8 * (cols + 1)));

            for (int i = 0; i < rows; i++)
            {
                System.Console.Write($"{i + 1,4} | "); 
                for (int j = 0; j < cols; j++)
                {
                    System.Console.Write($"{matrix[i, j],8:F2} "); 
                }
                System.Console.WriteLine();
            }
        }
    }
}
