using System;

namespace RestKit
{
    internal static class KnuckleheadExtensions
    {
        public static T DisallowNull<T>(this T t, string name) where T: class
        {
            if (t == null) throw new ArgumentNullException(name);
            return t;
        }

        public static string DisallowNullOrEmpty(this string t, string name)
        {
            if (string.IsNullOrEmpty(t)) throw new ArgumentException("The string value must not be null or empty.", name);
            return t;
        }
    }
}
