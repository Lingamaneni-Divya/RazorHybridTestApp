import json

# Load the schema from the input.json file
with open('input.json', 'r') as f:
    schema = json.load(f)

def generate_csharp_class(schema, class_name):
    class_def = f"public class {class_name}\n{{\n"
    
    for field in schema['Schema']:
        column_name = field['Column']
        prop_type = field['PropertyType']
        if prop_type == 'String':
            prop_type = 'string'
        elif prop_type == 'Int':
            prop_type = 'int'
        # Add more type conversions if necessary
        class_def += f"    public {prop_type} {column_name} {{ get; set; }}\n"
    
    class_def += "}\n"
    return class_def

csharp_class = generate_csharp_class(schema, "DataObject")

# Save the generated class to a file
with open('DataObject.cs', 'w') as f:
    f.write(csharp_class)

print("C# class generated and saved to DataObject.cs")