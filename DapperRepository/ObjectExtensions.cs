using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperRepository
{
    public static class ObjectExtensions
    {
        public static string ToSafeString(this object obj)
        {
            if (obj == null)
                return string.Empty;

            return obj.ToString();
        }

        public static string ToSafeString(this object obj, string defaultValue = null)
        {
            if (obj == null)
                return (defaultValue ?? string.Empty);

            return obj.ToString();
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsDbNull(this object obj)
        {
            return obj == DBNull.Value;
        }

        public static bool IsEmpty<T>(this ICollection<T> source)
        {
            return (source == null || source.Count == 0);
        }

        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static T IsNullDefault<T>(this T obj)
        {
            if (obj == null)
                return default(T);
            else
                return obj;
        }

        public static T IsNullDefault<T>(this T obj, object defaultValue)
        {
            if (obj == null)
                return (T)defaultValue;

            return obj;
        }

        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            if (obj.HasProperty(propertyName))
                return obj.GetType().GetProperty(propertyName).GetValue(obj, null);

            return null;
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, value, null);
        }

        public static PropertyInfo[] GetProperties(this object obj)
        {
            return obj.GetType().GetProperties();
        }

        public static string[] GetPropertyNames(this object obj)
        {
            return obj.GetType().GetProperties().Select(x => x.Name).ToArray();
        }

        public static object[] GetPropertyValues(this object obj)
        {
            return obj.GetType().GetProperties().Select(x => x.GetValue(obj)).ToArray();
        }

        public static Dictionary<string, object> GetPropertyDictionary(this object obj)
        {
            return obj.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(obj));
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            return stringsToValidate.Any(p => string.IsNullOrEmpty(p));
        }

        /// <summary>
        /// Compare two arrasy
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="a1">Array 1</param>
        /// <param name="a2">Array 2</param>
        /// <returns>Result</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
    }
}