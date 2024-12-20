using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Console.Common.Enums;
using TSP.Console.Files.Importer;

namespace TSP.Console.Files
{
    public class TSPLIBData
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Comment { get; set; }
        public int? Dimension { get; set; }
        public EdgeWeightTypeEnum EdgeWeightType { get; set; }
        public List<Node> Nodes { get; set; }

        public TSPLIBData()
        {
            Nodes = new List<Node>();
            EdgeWeightType = EdgeWeightTypeEnum.NONE;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Name: {Name}");
            builder.AppendLine($"Type: {Type}");
            builder.AppendLine($"Comment: {Comment}");
            builder.AppendLine($"Dimension: {Dimension}");
            builder.AppendLine($"Edge Weight Type: {EdgeWeightType}");
            builder.AppendLine("Nodes:");

            foreach (var node in Nodes)
            {
                builder.AppendLine($"\tID: {node.Id}, X: {node.X}, Y: {node.Y}");
            }

            return builder.ToString();
        }
    }
}
