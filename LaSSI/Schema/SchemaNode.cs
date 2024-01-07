using System.Collections.Generic;

namespace LaSSI.Schema;

public class SchemaNode
{
    public readonly string Key;
    public readonly SchemaNode? Parent;
    public readonly Dictionary<string, SchemaNode> Children;
    public readonly Dictionary<string, string> Mappings;

    public SchemaNode(SchemaNode? parent, string key)
    {
        this.Key = key;
        this.Parent = parent;
        this.Children = new Dictionary<string, SchemaNode>();
        this.Mappings = new Dictionary<string, string>();
    }
}