using System;
using System.Collections;
using System.Reflection;
using System.Linq;

public static class ObjectConverter
{
    public static T ConvertObject<T>(object source) where T : new()
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        T target = new T();
        Type sourceType = source.GetType();
        Type targetType = typeof(T);

        PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var targetProperty in targetProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(sp => sp.Name == targetProperty.Name);
            if (sourceProperty != null)
            {
                if (targetProperty.PropertyType == sourceProperty.PropertyType && targetProperty.CanWrite)
                {
                    targetProperty.SetValue(target, sourceProperty.GetValue(source));
                }
                else if (typeof(IEnumerable).IsAssignableFrom(sourceProperty.PropertyType) && targetProperty.PropertyType == typeof(string) && targetProperty.CanWrite)
                {
                    var sourceValue = sourceProperty.GetValue(source) as IEnumerable;
                    if (sourceValue != null)
                    {
                        var stringValue = string.Join(",", sourceValue.Cast<object>().Select(o => o.ToString()));
                        targetProperty.SetValue(target, stringValue);
                    }
                }
            }
        }

        return target;
    }
}

public class SourceClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; }
    public string[] Categories { get; set; }
    public IEnumerable<int> Values { get; set; }
    public string Description { get; set; }
}

public class TargetClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Tags { get; set; }
    public string Categories { get; set; }
    public string Values { get; set; }
}

class Program
{
    static void Main()
    {
        SourceClass source = new SourceClass
        {
            Id = 1,
            Name = "Source",
            CreatedAt = DateTime.Now,
            Tags = new List<string> { "tag1", "tag2", "tag3" },
            Categories = new[] { "cat1", "cat2" },
            Values = new List<int> { 1, 2, 3 },
            Description = "This is a description."
        };

        TargetClass target = ObjectConverter.ConvertObject<TargetClass>(source);

        Console.WriteLine($"Id: {target.Id}");
        Console.WriteLine($"Name: {target.Name}");
        Console.WriteLine($"CreatedAt: {target.CreatedAt}");
        Console.WriteLine($"Tags: {target.Tags}");
        Console.WriteLine($"Categories: {target.Categories}");
        Console.WriteLine($"Values: {target.Values}");
    }
}