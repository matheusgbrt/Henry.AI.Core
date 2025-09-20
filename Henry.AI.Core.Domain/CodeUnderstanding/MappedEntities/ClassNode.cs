using Henry.AI.Core.Domain.CodeUnderstanding.Helpers;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;
using Henry.AI.Core.Infrastructure.Neo4J.Utils;

namespace Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

public class ClassNode : INeo4JEntity
{
    public string Namespace { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TypeKind Kind { get; set; }

    public string KindString => TypeKindHelper.ToStringValue(Kind);
    public string Description { get; set; } = string.Empty;
    
    
    public Dictionary<string, object?> ToCypherParameters()
    {
        return new Dictionary<string, object?>
        {
            ["Namespace"] = Namespace,
            ["Name"] = Name,
            ["Kind"] = Kind,
            ["Description"] = Description
        };
    }

    public string ToCypherCreate()
    {
        return "(entity:ClassNode {Namespace:$Namespace, Name:$Name, Kind:$Kind, Description:$Description})";
    }

    public string ToCypherSearch()
    {
        return "(entity:ClassNode {Namespace:$Namespace, Name:$Name, Kind:$Kind})";
    }
}
