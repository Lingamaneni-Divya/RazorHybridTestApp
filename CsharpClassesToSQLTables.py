import re

def parse_csharp_file(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()

    classes = {}
    current_class = None

    class_pattern = re.compile(r'\bclass\b\s+(\w+)')
    property_pattern = re.compile(r'\bpublic\b\s+(\w+)\s+(\w+)\s*{')

    for line in lines:
        class_match = class_pattern.search(line)
        if class_match:
            current_class = class_match.group(1)
            classes[current_class] = []
            continue
        
        if current_class:
            property_match = property_pattern.search(line)
            if property_match:
                prop_type = property_match.group(1)
                prop_name = property_match.group(2)
                classes[current_class].append((prop_name, prop_type))

    return classes

def map_csharp_to_sql(csharp_type):
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
    if 'List' in csharp_type or 'IEnumerable' in csharp_type or 'ICollection' in csharp_type:
        return 'VARCHAR(MAX)'
    return type_mapping.get(csharp_type, 'VARCHAR(256)')  # Default to VARCHAR(256) for unknown types

def generate_sql_create_table_scripts(classes):
    sql_scripts = []

    for class_name, properties in classes.items():
        sql = f'CREATE TABLE {class_name} (\n'
        columns = []
        for prop_name, prop_type in properties:
            sql_type = map_csharp_to_sql(prop_type)
            columns.append(f'    {prop_name} {sql_type}')
        sql += ',\n'.join(columns)
        sql += '\n);'
        sql_scripts.append(sql)

    return sql_scripts

# Example usage:
file_path = 'classes.cs'
output_file_path = 'create_tables.sql'
classes = parse_csharp_file(file_path)
sql_scripts = generate_sql_create_table_scripts(classes)

# Write the SQL scripts to a file
with open(output_file_path, 'w') as file:
    for script in sql_scripts:
        file.write(script + '\n')