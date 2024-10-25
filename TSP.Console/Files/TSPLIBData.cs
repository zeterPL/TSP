﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Console.Common;

namespace TSP.Console.Files
{
    public class TSPLIBData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public string Dimension { get; set; }
        public EdgeWeightTypeEnum EdgeWeightType { get; set; }
        //TODO nodes
    }
}
