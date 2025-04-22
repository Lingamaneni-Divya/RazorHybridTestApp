import json
from pathlib import Path

# Define a function to generate C# model class from JSON schema
def generate_model_class(schema):
    class_name = schema["title"]
    properties = schema["properties"]
    type_map = {
        "string": "string",
        "integer": "int",
        "number": "double",
        "boolean": "bool"
    }

    lines = [f"public class {class_name}", "{"]
    for prop, value in properties.items():
        csharp_type = type_map.get(value["type"], "string")
        lines.append(f"    public {csharp_type} {prop[0].upper() + prop[1:]} {{ get; set; }}")
    lines.append("}")
    return "\n".join(lines)

# Generate a controller with CRUD operations
def generate_controller(schema):
    class_name = schema["title"]
    variable_name = class_name[0].lower() + class_name[1:]
    id_type = "int"

    return f"""[ApiController]
[Route("api/[controller]")]
public class {class_name}Controller : ControllerBase
{{
    private static List<{class_name}> _{variable_name}s = new();

    [HttpGet]
    public ActionResult<IEnumerable<{class_name}>> GetAll() => _{variable_name}s;

    [HttpGet("{{id}}")]
    public ActionResult<{class_name}> GetById({id_type} id)
    {{
        var item = _{variable_name}s.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        return item;
    }}

    [HttpPost]
    public ActionResult<{class_name}> Create({class_name} input)
    {{
        _{variable_name}s.Add(input);
        return CreatedAtAction(nameof(GetById), new {{ id = input.Id }}, input);
    }}

    [HttpPut("{{id}}")]
    public IActionResult Update({id_type} id, {class_name} input)
    {{
        var index = _{variable_name}s.FindIndex(x => x.Id == id);
        if (index == -1) return NotFound();
        _{variable_name}s[index] = input;
        return NoContent();
    }}

    [HttpDelete("{{id}}")]
    public IActionResult Delete({id_type} id)
    {{
        var item = _{variable_name}s.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        _{variable_name}s.Remove(item);
        return NoContent();
    }}
}}
"""

# Sample JSON schema input
schema = {
    "title": "Feedback",
    "type": "object",
    "properties": {
        "id": {"type": "integer"},
        "customerName": {"type": "string"},
        "email": {"type": "string"},
        "rating": {"type": "integer"},
        "comments": {"type": "string"}
    },
    "required": ["customerName", "email", "rating"]
}

# Generate code
model_code = generate_model_class(schema)
controller_code = generate_controller(schema)

# Save files
output_dir = Path("/mnt/data/GeneratedApi")
output_dir.mkdir(exist_ok=True)
model_path = output_dir / f"{schema['title']}.cs"
controller_path = output_dir / f"{schema['title']}Controller.cs"

model_path.write_text(model_code)
controller_path.write_text(controller_code)

str(output_dir)
