import json

class ClassGenerator:
    def __init__(self):
        self.generated_classes = {}

    def json_to_cs_class(self, json_obj, class_name):
        if class_name in self.generated_classes:
            return ""
        
        lines = []
        lines.append(f"public class {class_name}")
        lines.append("{")
        
        for key, value in json_obj.items():
            if key.startswith('@'):
                continue
            csharp_type, nested_class_def = self.get_csharp_type(value, key)
            property_name = key[0].upper() + key[1:]
            lines.append(f"    public {csharp_type} {property_name} {{ get; set; }}")
            if nested_class_def:
                lines.append(nested_class_def)
        
        lines.append("}")
        class_definition = "\n".join(lines)
        self.generated_classes[class_name] = class_definition
        return class_definition

    def get_csharp_type(self, value, key):
        if isinstance(value, int):
            return "int", None
        elif isinstance(value, float):
            return "double", None
        elif isinstance(value, bool):
            return "bool", None
        elif isinstance(value, list):
            if value:
                list_type, nested_class_def = self.get_csharp_type(value[0], key)
                return f"List<{list_type}>", nested_class_def
            else:
                return "List<object>", None
        elif isinstance(value, dict):
            class_name = key[0].upper() + key[1:]
            nested_class_def = self.json_to_cs_class(value, class_name)
            return class_name, nested_class_def
        else:
            return "string", None

    def convert_json_file_to_cs(self, json_file_path, cs_file_path, class_name):
        with open(json_file_path, 'r') as json_file:
            json_obj = json.load(json_file)
        
        cs_classes = ""
        if isinstance(json_obj, list):
            cs_classes += self.json_to_cs_class(json_obj[0], class_name)
        elif 'value' in json_obj:
            for item in json_obj['value']:
                cs_classes += self.json_to_cs_class(item, class_name)
        else:
            cs_classes += self.json_to_cs_class(json_obj, class_name)
        
        with open(cs_file_path, 'w') as cs_file:
            for class_def in self.generated_classes.values():
                cs_file.write(class_def + "\n\n")
        
        print(f"C# class definition saved to {cs_file_path}")

# Example usage
json_file_path = "input.json"  # Path to your JSON file
cs_file_path = "OutputClass.cs"  # Desired path for the generated C# class file
class_name = "RootClass"  # Desired root class name

generator = ClassGenerator()
generator.convert_json_file_to_cs(json_file_path, cs_file_path, class_name)