using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Interfaces.Plugins;
using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Plugins;

/// <summary>
///     Plugin for creating and managing Class Diagram graph data.
/// </summary>
public class ClassDiagramPlugin : IClassDiagramPlugin
{
    /// <summary>
    ///     Adds a class to the graph with its associated metadata.
    /// </summary>
    /// <param name="className">The name of the class to add.</param>
    /// <param name="metadata">A dictionary containing the class attributes as key-value pairs.</param>
    /// <param name="graphData">The graph data structure to which the class is added.</param>
    public void AddClass(string className, Dictionary<string, object> metadata, GraphData graphData)
    {
        var node = graphData.FindOrAddNode(className);
        foreach (var attribute in metadata) node.Metadata[attribute.Key] = attribute.Value;
    }

    /// <summary>
    ///     Adds relationships between classes to the graph.
    /// </summary>
    /// <param name="relationships">A collection of relationships to add to the graph.</param>
    /// <param name="graphData">The graph data structure to which the relationships are added.</param>
    public void AddRelationships(IEnumerable<Relationship> relationships, GraphData graphData)
    {
        foreach (var relationship in relationships)
            if (relationship.Key == "inherits")
                AddInheritance(relationship.FromEntity, relationship.ToEntity, graphData);
            else
                AddAssociation(relationship.FromEntity, relationship.ToEntity, relationship.Key, graphData);
    }

    /// <summary>
    ///     Retrieves all nodes from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the nodes.</param>
    /// <returns>An enumerable collection of nodes in the graph.</returns>
    public IEnumerable<Node> GetNodes(GraphData graphData)
    {
        return graphData.Nodes;
    }

    /// <summary>
    ///     Retrieves all edges (relationships) from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>An enumerable collection of edges in the graph.</returns>
    public IEnumerable<Edge> GetEdges(GraphData graphData)
    {
        return graphData.Edges;
    }

    /// <summary>
    ///     Adds an inheritance relationship between two classes in the graph.
    /// </summary>
    /// <param name="parentClass">The name of the parent class.</param>
    /// <param name="childClass">The name of the child class.</param>
    /// <param name="graphData">The graph data structure to which the relationship is added.</param>
    private static void AddInheritance(string parentClass, string childClass, GraphData graphData)
    {
        graphData.FindOrAddEdge(parentClass, childClass, "inherits");
    }

    /// <summary>
    ///     Adds an association relationship between two classes in the graph.
    /// </summary>
    /// <param name="fromClass">The name of the source class in the relationship.</param>
    /// <param name="toClass">The name of the target class in the relationship.</param>
    /// <param name="relationshipName">The name of the relationship.</param>
    /// <param name="graphData">The graph data structure to which the relationship is added.</param>
    private static void AddAssociation(string fromClass, string toClass, string relationshipName, GraphData graphData)
    {
        graphData.FindOrAddEdge(fromClass, toClass, relationshipName);
    }
}