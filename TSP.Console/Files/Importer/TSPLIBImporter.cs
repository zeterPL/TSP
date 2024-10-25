using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Console.Common;
using TSP.Console.Common.Exceptions;
using TSP.Console.Files.Importer;

namespace TSP.Console.Files
{
    public static class TSPLIBImporter
    {
        public static TSPLIBData Import(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            var data = new TSPLIBData();
            var lines = File.ReadAllLines(path);

            try
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var parts = line.Split(':');

                    var key = parts[0].Trim();
                    var value = "";
                    if (parts.Length == 2)
                    {
                        key = parts[0].Trim();
                        value = parts[1].Trim();
                    }

                    

                    switch (key)
                    {
                        case "NAME":
                            data.Name = value;
                            break;

                        case "TYPE":
                            data.Type = value;
                            break;

                        case "COMMENT":
                            data.Comment = value;
                            break;

                        case "DIMENSION":
                            if (!int.TryParse(value, out var dimension))
                                throw new TSPLIBParseException("Invalid dimension value.", i + 1, line, "DIMENSION");
                            data.Dimension = dimension;
                            break;

                        case "EDGE_WEIGHT_TYPE":
                            if (!Enum.TryParse(value, true, out EdgeWeightTypeEnum edgeWeightType))
                                throw new TSPLIBParseException("Invalid edge weight type.", i + 1, line, "EDGE_WEIGHT_TYPE");
                            data.EdgeWeightType = edgeWeightType;
                            break;

                        case "NODE_COORD_SECTION":
                            ParseNodes(lines, data.Nodes);
                            break;
                    }
                }
                if (data.Nodes.Count == 0)
                    throw new TSPLIBParseException("NODE_COORD_SECTION is missing or empty.", null, null, "NODE_COORD_SECTION");
            }
            catch (TSPLIBParseException e)
            {
                System.Console.WriteLine($"Error parsing TSPLIB file: {e}");
                throw;
            }
            return data;
        }

        private static void ParseNodes(string[] lines, List<Node> nodes)
        {
            var nodeLines = lines.SkipWhile(line => !line.StartsWith("NODE_COORD_SECTION")).Skip(1);
            foreach (var line in nodeLines)
            {
                if (line.Trim() == "EOF") break;

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                nodes.Add(new Node
                {
                    Id = int.Parse(parts[0]),
                    X = double.Parse(parts[1].Replace('.', ',')),
                    Y = double.Parse(parts[2].Replace('.', ','))
                });
            }
        }
    }
}