import json
# version 3
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
                if isinstance(value[0], dict):
                    nested_class_def = self.json_to_cs_class(value[0], list_type)
                return f"List<{list_type}>", nested_class_def
            else:
                return "List<object>", None
        elif isinstance(value, dict):
            class_name = key[0].upper() + key[1:]
            nested_class_def = self.json_to_cs_class(value, class_name)
            return class_name, nested_class_def
        else:
            return "string", None

    def convert_json_file_to_cs(self, json_file_path, cs_file_path, root_class_name):
        with open(json_file_path, 'r') as json_file:
            json_obj = json.load(json_file)
        
        if isinstance(json_obj, dict) and 'value' in json_obj:
            if isinstance(json_obj['value'], list):
                if json_obj['value'] and isinstance(json_obj['value'][0], dict):
                    # Handle array of nested JSON objects
                    root_class_def = f"public class {root_class_name}\n{{\n    public List<Root> Value {{ get; set; }}\n}}\n"
                    self.generated_classes[root_class_name] = root_class_def
                    self.json_to_cs_class(json_obj['value'][0], "Root")
                else:
                    # Handle array of simple JSON objects
                    root_class_def = f"public class {root_class_name}\n{{\n    public List<object> Value {{ get; set; }}\n}}\n"
                    self.generated_classes[root_class_name] = root_class_def
            elif isinstance(json_obj['value'], dict):
                # Handle single nested JSON object
                root_class_def = f"public class {root_class_name}\n{{\n    public Root Value {{ get; set; }}\n}}\n"
                self.generated_classes[root_class_name] = root_class_def
                self.json_to_cs_class(json_obj['value'], "Root")
            else:
                # Handle simple JSON object
                root_class_def = f"public class {root_class_name}\n{{\n    public object Value {{ get; set; }}\n}}\n"
                self.generated_classes[root_class_name] = root_class_def
        else:
            # Handle case where root is a single object
            self.json_to_cs_class(json_obj, root_class_name)
        
        with open(cs_file_path, 'w') as cs_file:
            for class_def in self.generated_classes.values():
                cs_file.write(class_def + "\n\n")
        
        print(f"C# class definition saved to {cs_file_path}")

# Example usage
json_file_path = "input.json"  # Path to your JSON file
cs_file_path = "OutputClass.cs"  # Desired path for the generated C# class file
root_class_name = "RootClass"  # Desired root class name

generator = ClassGenerator()
generator.convert_json_file_to_cs(json_file_path, cs_file_path, root_class_name)