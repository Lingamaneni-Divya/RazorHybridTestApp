import json
import os

def json_to_cs_class(json_obj, class_name):
    lines = []
    lines.append(f"public class {class_name}")
    lines.append("{")
    for key, value in json_obj.items():
        csharp_type, nested_classes = get_csharp_type(value, key)
        property_name = key[0].upper() + key[1:]
        lines.append(f"    public {csharp_type} {property_name} {{ get; set; }}")
        lines.extend(nested_classes)
    lines.append("}")
    return "\n".join(lines)

def get_csharp_type(value, key):
    if isinstance(value, int):
        return "int", []
    elif isinstance(value, float):
        return "double", []
    elif isinstance(value, bool):
        return "bool", []
    elif isinstance(value, list):
        if value:
            list_type, nested_classes = get_csharp_type(value[0], key)
            return f"List<{list_type}>", nested_classes
        else:
            return "List<object>", []
    elif isinstance(value, dict):
        class_name = key[0].upper() + key[1:]
        nested_class = json_to_cs_class(value, class_name)
        return class_name, [nested_class]
    else:
        return "string", []

def convert_json_file_to_cs(json_file_path, cs_file_path, class_name):
    with open(json_file_path, 'r') as json_file:
        json_obj = json.load(json_file)
    
    cs_class = json_to_cs_class(json_obj, class_name)
    
    with open(cs_file_path, 'w') as cs_file:
        cs_file.write(cs_class)
    
    print(f"C# class definition saved to {cs_file_path}")

# Example usage
json_file_path = "input.json"  # Path to your JSON file
cs_file_path = "OutputClass.cs"  # Desired path for the generated C# class file
class_name = "Root"  # Desired root class name

convert_json_file_to_cs(json_file_path, cs_file_path, class_name)