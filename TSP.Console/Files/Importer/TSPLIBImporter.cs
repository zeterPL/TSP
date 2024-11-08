using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Console.Files
{
    public static class TSPLIBImporter
    {
        public static TSPLIBData Import(string path)
        {
            var lines = File.ReadAllLines(path);

            foreach(var line in lines)
            {

            }
        }
    }
}
