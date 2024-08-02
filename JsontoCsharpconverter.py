import json
import os

def json_to_cs_class(json_obj, class_name, nested_classes=set()):
    lines = []
    lines.append(f"public class {class_name}")
    lines.append("{")
    
    for key, value in json_obj.items():
        if key.startswith('@'):
            continue
        csharp_type, nested = get_csharp_type(value, key, nested_classes)
        property_name = key[0].upper() + key[1:]
        lines.append(f"    public {csharp_type} {property_name} {{ get; set; }}")
        nested_classes.update(nested)
    
    lines.append("}")
    nested_definitions = "\n\n".join(nested_classes)
    return "\n".join(lines) + ("\n\n" + nested_definitions if nested_definitions else "")

def get_csharp_type(value, key, nested_classes):
    if isinstance(value, int):
        return "int", set()
    elif isinstance(value, float):
        return "double", set()
    elif isinstance(value, bool):
        return "bool", set()
    elif isinstance(value, list):
        if value:
            list_type, nested = get_csharp_type(value[0], key, nested_classes)
            return f"List<{list_type}>", nested
        else:
            return "List<object>", set()
    elif isinstance(value, dict):
        class_name = key[0].upper() + key[1:]
        nested_class = json_to_cs_class(value, class_name, nested_classes)
        nested_classes.add(nested_class)
        return class_name, nested_classes
    else:
        return "string", set()

def convert_json_file_to_cs(json_file_path, cs_file_path, class_name):
    with open(json_file_path, 'r') as json_file:
        json_obj = json.load(json_file)
    
    if 'value' in json_obj:
        cs_class = json_to_cs_class(json_obj['value'][0], class_name)
    else:
        cs_class = json_to_cs_class(json_obj, class_name)
    
    with open(cs_file_path, 'w') as cs_file:
        cs_file.write(cs_class)
    
    print(f"C# class definition saved to {cs_file_path}")

# Example usage
json_file_path = "input.json"  # Path to your JSON file
cs_file_path = "OutputClass.cs"  # Desired path for the generated C# class file
class_name = "User"  # Desired root class name

convert_json_file_to_cs(json_file_path, cs_file_path, class_name)