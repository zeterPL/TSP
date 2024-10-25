using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Console.Common.Exceptions
{
    public class TSPLIBParseException : Exception
    {
        public int? LineNumber { get; }
        public string LineContent { get; }
        public string Section { get; }

        public TSPLIBParseException(string message, int? lineNumber = null, string lineContent = null, string section = null)
            : base(message)
        {
            LineNumber = lineNumber;
            LineContent = lineContent;
            Section = section;
        }

        public override string ToString()
        {
            var baseMessage = base.ToString();
            var lineInfo = LineNumber.HasValue ? $"Line {LineNumber}: {LineContent}" : "Unknown line";
            var sectionInfo = string.IsNullOrEmpty(Section) ? "General error" : $"Section: {Section}";

            return $"{baseMessage}\n{sectionInfo}\n{lineInfo}";
        }
    }
}
