import re

def parse_csharp_classes(file_path):
    with open(file_path, 'r') as file:
        csharp_code = file.read()

    # Regex to match class names and properties
    class_pattern = re.compile(r'class\s+(\w+)\s*{(.*?)}', re.DOTALL)
    property_pattern = re.compile(r'\s*public\s+(?P<type>\w+|\w+\[\])\s+(?P<name>\w+)\s*\{')

    # Extract class names and properties
    classes = class_pattern.findall(csharp_code)
    parsed_classes = {}
    
    for class_name, class_body in classes:
        properties = property_pattern.findall(class_body)
        parsed_classes[class_name] = properties

    return parsed_classes

def generate_flat_classes(parsed_classes):
    flat_classes = {}
    for class_name, properties in parsed_classes.items():
        flat_properties = []
        for prop_type, prop_name in properties:
            if prop_type in parsed_classes:  # Nested class
                nested_properties = parsed_classes[prop_type]
                for nested_type, nested_name in nested_properties:
                    new_prop_name = f"{prop_name}_{nested_name}".replace('_', '')
                    flat_properties.append((nested_type, new_prop_name))
            elif prop_type.endswith('[]') and prop_type[:-2] in parsed_classes:  # List of nested class
                new_class_name = f"{class_name}_{prop_name}".replace('_', '')
                new_class_properties = [(nested_type, nested_name) if nested_name != 'Id' else ('string', f'{class_name}Id')
                                        for nested_type, nested_name in parsed_classes[prop_type[:-2]]]
                flat_classes[new_class_name] = new_class_properties
                flat_properties.append(('string', f"{new_class_name}_Id"))
            else:
                flat_properties.append((prop_type, prop_name.replace('_', '')))
        flat_classes[class_name] = flat_properties

    return flat_classes

def generate_csharp_class_definitions(flat_classes):
    class_definitions = ""
    for class_name, properties in flat_classes.items():
        class_definitions += f"public class {class_name}\n{{\n"
        for prop_type, prop_name in properties:
            class_definitions += f"    public {prop_type} {prop_name} {{ get; set; }}\n"
        class_definitions += "}\n\n"

    return class_definitions

def write_output(file_path, content):
    with open(file_path, 'w') as file:
        file.write(content)

def main():
    input_file = 'classes.cs'  # Path to your input C# file
    output_file = 'FlatClasses.cs'  # Path to your output file

    try:
        parsed_classes = parse_csharp_classes(input_file)
        flat_classes = generate_flat_classes(parsed_classes)
        class_definitions = generate_csharp_class_definitions(flat_classes)
        write_output(output_file, class_definitions)
        print(f'Flattened class definitions generated in {output_file}')
    except Exception as e:
        print(f'Error: {e}')

if __name__ == '__main__':
    main()