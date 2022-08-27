using System.Diagnostics.CodeAnalysis;

namespace Musmetaniac.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? self)
        {
            return string.IsNullOrEmpty(self);
        }
    }
}
