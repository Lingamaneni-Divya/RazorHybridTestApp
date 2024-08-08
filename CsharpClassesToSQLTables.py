import re

# Version 1.3 - Improved handling of nullable types and lists/arrays
def parse_csharp_file(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()

    classes = {}
    current_class = None

    class_pattern = re.compile(r'\bclass\b\s+(\w+)')
    property_pattern = re.compile(r'\bpublic\b\s+(required\s+)?(\w+(\[\])?|List<\w+>|IEnumerable<\w+>|ICollection<\w+>)\s+(\w+)\s*{')

    for line in lines:
        class_match = class_pattern.search(line)
        if class_match:
            current_class = class_match.group(1)
            classes[current_class] = []
            continue
        
        if current_class:
            property_match = property_pattern.findall(line)
            for match in property_match:
                required = 'required' in match[0]
                prop_type = match[1]
                prop_name = match[3]
                # Check if property type is a list or array
                if 'List' in prop_type or 'IEnumerable' in prop_type or 'ICollection' in prop_type or '[]' in prop_type:
                    prop_type = 'List'  # Use a common identifier for lists
                classes[current_class].append((prop_name, prop_type, required))

    return classes

def map_csharp_to_sql(prop_name, csharp_type, required):
    type_mapping = {
        'int': 'INT',
        'long': 'BIGINT',
        'float': 'FLOAT',
        'double': 'DOUBLE',
        'decimal': 'DECIMAL',
        'bool': 'BIT',
        'string': 'VARCHAR(256)',
        'DateTime': 'DATETIME'
    }
    if csharp_type == 'List':
        sql_type = 'VARCHAR(MAX)'
    else:
        sql_type = type_mapping.get(csharp_type, 'VARCHAR(256)')  # Default to VARCHAR(256) for unknown types
    
    if required:
        sql_type += ' NOT NULL'
    
    return f'    {prop_name} {sql_type}'

def generate_sql_create_table_scripts(classes, schema_name):
    sql_scripts = []

    for class_name, properties in classes.items():
        sql = f'CREATE TABLE {schema_name}.{class_name} (\n'
        columns = [map_csharp_to_sql(prop_name, prop_type, required) for prop_name, prop_type, required in properties]
        sql += ',\n'.join(columns)
        sql += '\n);'
        sql_scripts.append(sql)

    return sql_scripts

# Example usage:
file_path = 'classes.cs'
output_file_path = 'create_tables.sql'
schema_name = 'IntuneMobilityViolence'
classes = parse_csharp_file(file_path)
sql_scripts = generate_sql_create_table_scripts(classes, schema_name)

# Write the SQL scripts to a file
with open(output_file_path, 'w') as file:
    for script in sql_scripts:
        file.write(script + '\n')