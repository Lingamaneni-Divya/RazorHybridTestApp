using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

public class JsonToCSharpConverter
{
    private readonly HashSet<string> _definedClasses = new HashSet<string>();

    public void ConvertJsonToCSharp(string json, string outputPath)
    {
        var jObject = JObject.Parse(json);
        var classes = GenerateClasses(jObject, "Root");
        File.WriteAllText(outputPath, classes);
    }

    private string GenerateClasses(JObject jObject, string className)
    {
        var classDefinitions = new List<string>();

        GenerateClass(jObject, className, classDefinitions);

        return string.Join("\n\n", classDefinitions);
    }

    private void GenerateClass(JObject jObject, string className, List<string> classDefinitions)
    {
        if (_definedClasses.Contains(className))
        {
            return;
        }

        _definedClasses.Add(className);

        var properties = new List<string>();

        foreach (var property in jObject.Properties())
        {
            var propertyName = property.Name;
            var propertyType = GetCSharpType(property.Value, propertyName, classDefinitions);

            properties.Add($"    public {propertyType} {propertyName} {{ get; set; }}");
        }

        var classDefinition = $"public class {className}\n{{\n{string.Join("\n", properties)}\n}}";
        classDefinitions.Add(classDefinition);
    }

    private string GetCSharpType(JToken token, string propertyName, List<string> classDefinitions)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                var className = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
                GenerateClass((JObject)token, className, classDefinitions);
                return className;

            case JTokenType.Array:
                var array = (JArray)token;
                if (array.Count > 0)
                {
                    var arrayType = GetCSharpType(array[0], propertyName, classDefinitions);
                    return $"List<{arrayType}>";
                }
                return "List<object>";

            case JTokenType.Integer:
                return "int";

            case JTokenType.Float:
                return "double";

            case JTokenType.String:
                return "string";

            case JTokenType.Boolean:
                return "bool";

            case JTokenType.Date:
                return "DateTime";

            case JTokenType.Null:
                return "object";

            default:
                return "string";
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var json = @"{
            'id': '1',
            'name': 'Device1',
            'details': {
                'manufacturer': 'ABC Corp',
                'model': 'XYZ123',
                'specs': {
                    'processor': 'Intel i7',
                    'ram': '16GB',
                    'storage': '512GB'
                }
            },
            'status': 'Active'
        }";

        var converter = new JsonToCSharpConverter();
        converter.ConvertJsonToCSharp(json, "output.cs");
    }
}