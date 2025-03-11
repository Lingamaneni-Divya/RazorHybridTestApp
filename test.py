import os
import re
from datetime import datetime
import random

# Define mock data generators
MOCK_VALUES = {
    "int": lambda: random.randint(1, 1000),
    "string": lambda: f"SampleText{random.randint(1, 100)}",
    "bool": lambda: random.choice(["true", "false"]),
    "DateTime": lambda: f"DateTime.UtcNow.AddDays(-{random.randint(0, 30)})",
    "double": lambda: round(random.uniform(1.0, 100.0), 2),
    "float": lambda: round(random.uniform(1.0, 100.0), 2),
    "Guid": lambda: f'Guid.NewGuid()'
}

# Extract all C# classes and properties from a .cs file
def extract_classes_from_file(file_path):
    with open(file_path, "r", encoding="utf-8") as file:
        content = file.read()

    # Match class definitions
    class_matches = re.findall(r'class\s+(\w+)', content)

    # Match properties (public {Type} {PropertyName} { get; set; })
    properties = re.findall(r'public\s+(\w+)\s+(\w+)\s*{\s*get;\s*set;\s*}', content)

    classes = {}
    for class_name in class_matches:
        classes[class_name] = {
            prop_name: MOCK_VALUES.get(prop_type, lambda: "null")()
            for prop_type, prop_name in properties
        }
    return classes

# Scan all .cs files in the Models folder and subfolders
def scan_models_folder(models_folder):
    mock_classes = {}
    for root, _, files in os.walk(models_folder):
        for file in files:
            if file.endswith(".cs"):
                file_path = os.path.join(root, file)
                extracted_classes = extract_classes_from_file(file_path)
                mock_classes.update(extracted_classes)
    return mock_classes

# Generate mock data
models_folder = "Models"  # Change if your models folder is different
mock_classes = scan_models_folder(models_folder)

# Generate the C# mock file content
mock_file_content = """using System;
using System.Collections.Generic;
using IntuneMobilityViolationJob.Models.DataTableModels;

public static class InputDataMock
{
"""

for class_name, properties in mock_classes.items():
    mock_file_content += f"    public static List<{class_name}> Get{class_name}Mock()\n    {{\n"
    mock_file_content += f"        return new List<{class_name}>\n        {{\n"
    
    for _ in range(2):  # Generate two mock entries
        mock_file_content += "            new " + class_name + " { "
        mock_file_content += ", ".join(f"{key} = {value}" for key, value in properties.items())
        mock_file_content += " },\n"

    mock_file_content += "        };\n    }\n\n"

mock_file_content += "}\n"

# Save the file
with open("InputDataMock.cs", "w", encoding="utf-8") as cs_file:
    cs_file.write(mock_file_content)

print("Mock data class generated and saved as InputDataMock.cs")