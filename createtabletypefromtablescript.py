import re

def parse_create_table(sql_script):
    # Extract table name
    table_name_match = re.search(r'CREATE TABLE\s+(\w+)', sql_script, re.IGNORECASE)
    if not table_name_match:
        raise ValueError("Could not find table name in the SQL script.")
    table_name = table_name_match.group(1)

    # Extract column definitions
    column_pattern = re.compile(r'(\w+)\s+([\w\(\)]+)')
    columns = column_pattern.findall(sql_script)

    return table_name, columns

def generate_table_type_script(table_name, columns, schema_name):
    # Create the TableType name by appending 'Type' to the table name
    table_type_name = f"{table_name}Type"
    
    # Start building the script with the schema name
    script_lines = [f"CREATE TYPE {schema_name}.{table_type_name} AS TABLE("]
    
    # Add columns to the TableType
    for column_name, data_type in columns:
        script_lines.append(f"    {column_name} {data_type},")
    
    # Remove the last comma and close the definition
    script_lines[-1] = script_lines[-1].rstrip(',')
    script_lines.append(");")
    
    return "\n".join(script_lines)

def process_sql_file(input_file_path, output_file_path, schema_name):
    with open(input_file_path, 'r') as file:
        sql_script = file.read()

    # Split the file by "CREATE TABLE" to handle multiple tables
    table_scripts = sql_script.split("CREATE TABLE")
    table_scripts = [f"CREATE TABLE {script}" for script in table_scripts if script.strip()]

    table_type_scripts = []
    
    for script in table_scripts:
        try:
            table_name, columns = parse_create_table(script)
            table_type_script = generate_table_type_script(table_name, columns, schema_name)
            table_type_scripts.append(table_type_script)
        except Exception as e:
            print(f"Error processing table script: {e}")

    # Write the TableType scripts to the output file
    with open(output_file_path, 'w') as file:
        file.write("\n\n".join(table_type_scripts))

def main():
    input_file_path = 'Create tables.sql'
    output_file_path = 'Create table types.sql'
    schema_name = 'IntuneMobilityViolence'

    process_sql_file(input_file_path, output_file_path, schema_name)
    print(f"Table types have been generated in {output_file_path}")

if __name__ == "__main__":
    main()