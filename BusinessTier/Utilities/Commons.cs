using System.Text.RegularExpressions;

namespace BusinessTier.Utilities
{
    public static class Commons
    {
        public static string ToSnakeCase(this string o) =>
       Regex.Replace(o, @"(\w)([A-Z])", "$1-$2").ToLower();
    }
}
