using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Interfaces.Plugins;

/// <summary>
///     Provides methods for creating and managing Class Diagram graph data.
/// </summary>
public interface IClassDiagramPlugin
{
    /// <summary>
    ///     Adds a class to the graph with associated metadata.
    /// </summary>
    /// <param name="className">The name of the class to add.</param>
    /// <param name="metadata">A dictionary containing the class attributes as key-value pairs.</param>
    /// <param name="graphData">The graph data structure to which the class is added.</param>
    void AddClass(string className, Dictionary<string, object> metadata, GraphData graphData);

    /// <summary>
    ///     Adds relationships between classes to the graph.
    /// </summary>
    /// <param name="relationships">A collection of relationships to add to the graph.</param>
    /// <param name="graphData">The graph data structure to which the relationships are added.</param>
    void AddRelationships(IEnumerable<Relationship> relationships, GraphData graphData);

    /// <summary>
    ///     Retrieves all nodes (classes) from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the nodes.</param>
    /// <returns>An enumerable collection of nodes in the graph.</returns>
    IEnumerable<Node> GetNodes(GraphData graphData);

    /// <summary>
    ///     Retrieves all edges (relationships) from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>An enumerable collection of edges in the graph.</returns>
    IEnumerable<Edge> GetEdges(GraphData graphData);
}