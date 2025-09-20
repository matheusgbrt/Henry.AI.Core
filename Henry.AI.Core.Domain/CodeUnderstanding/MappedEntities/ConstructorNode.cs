using System.Text.Json;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;

namespace Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

public class ConstructorNode : INeo4JEntity
{
    public string ClassName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    public string ToCypherCreate()
    {
        return "(entity:ConstructorNode {ClassName:$ClassName, Description:$Description, Parameters:$Parameters})";
    }

    public string ToCypherSearch()
    {
        return "(entity:ConstructorNode {ClassName:$ClassName})";
    }

    public Dictionary<string, object?> ToCypherParameters()
    {
        return new Dictionary<string, object?>
        {
            ["ClassName"] = ClassName,
            ["Description"] = Description,
            ["Parameters"] = JsonSerializer.Serialize(Parameters.Select(p => p.ToCypherParameters()))
        };
    }
}