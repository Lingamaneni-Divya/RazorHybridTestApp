using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class GenericEqualityComparer<T> : IEqualityComparer<T>
{
    public bool Equals(T x, T y)
    {
        if (x == null || y == null)
            return false;

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var valueX = prop.GetValue(x);
            var valueY = prop.GetValue(y);

            if (valueX == null && valueY == null)
                continue;

            if (valueX == null || valueY == null || !valueX.Equals(valueY))
                return false;
        }

        return true;
    }

    public int GetHashCode(T obj)
    {
        if (obj == null)
            return 0;

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        int hash = 17;

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            if (value != null)
            {
                hash = hash * 23 + value.GetHashCode();
            }
        }

        return hash;
    }
}

public static class DistinctHelper
{
    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source)
    {
        return source.Distinct(new GenericEqualityComparer<T>());
    }
}