using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace superscalar_arch_sim_gui.Utilis
{
    internal static class TextFormatting
    {
        private const string CamelCaseRegex = @"([A-Z]+(?![a-z])|[A-Z][a-z]+|[0-9]+|[a-z]+)";
        public static IEnumerable<string> SplitCamelCaseWords(string text)
            => Regex.Matches(text, CamelCaseRegex).OfType<Match>().Select(m => m.Value);

        public static string SeparateCamelCaseWords(string text, string separator = " ")
            => string.Join(separator, SplitCamelCaseWords(text)); 
    }
}
