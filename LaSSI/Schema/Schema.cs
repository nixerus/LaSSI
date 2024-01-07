using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LaSSI.Schema;

public class Schema
{
    private readonly Dictionary<string, string[]> Types;
    private readonly SchemaNode Root;

    public Schema()
    {
        var schema = LoadSchema();
        this.Types = schema.Types;
        
        var root = LoadRoot(schema);
        this.Root = root;
    }

    public string[]? GetOptions(Node node, Oncler oncler)
    {
        var path = GetPath(node);
        var schemaNode = Traverse(Root, path.Split("/").Skip(1).ToArray());
        if (schemaNode == null)
            return null;

        if (!schemaNode.Mappings.ContainsKey(oncler.Key))
            return null;

        var type = schemaNode.Mappings[oncler.Key];
        if (!Types.ContainsKey(type))
            return null;

        return Types[type];
    }

    private SchemaNode? Traverse(SchemaNode node, string[] path)
    {
        if (node.Children.ContainsKey(path[0]))
        {
            if (path.Length == 1)
            {
                return node.Children[path[0]];
            }

            return Traverse(node.Children[path[0]], path.Skip(1).ToArray());
        }

        return null;
    }

    private string GetPath(Node node)
    {
        if (node.Parent == null)
            return node.Name;
        var parentPath = GetPath((Node) node.Parent);
        return parentPath + "/" + node.BaseName;
    }
    
    private SchemaNode ParseObject(SchemaNode? parent, string key, JObject jObject)
    {
        var node = new SchemaNode(parent, key);
        foreach(var child in jObject.Properties())
        {
            if (child.Value.Type == JTokenType.String)
            {
                node.Mappings.Add(child.Name, child.Value.Value<string>()!);
            }

            if (child.Value.Type == JTokenType.Object)
            {
                node.Children.Add(child.Path, ParseObject(node, child.Path, child.Value.Value<JObject>()!));
            }
        }

        return node;
    }

    private SchemaFile LoadSchema()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LaSSI.schema.json");
        TextReader reader = new StreamReader(stream!);
        var text = reader.ReadToEnd();

        var schemaFile = JsonConvert.DeserializeObject<SchemaFile>(text);
        if (schemaFile == null)
            throw new SystemException("Failed to load schema!");
        return schemaFile;
    }
    
    private SchemaNode LoadRoot(SchemaFile schemaFile)
    {
        var tree = schemaFile.Tree as JObject;
        var root = ParseObject(null, "root", tree!);
        return root;
    }
}

public class SchemaFile
{
    [JsonProperty("tree")] public readonly dynamic Tree;
    
    [JsonProperty("types")]
    public readonly Dictionary<string, string[]> Types;
}