using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Models;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

/// <summary>
/// Represents a graph data structure with functionality for node and edge management.
/// </summary>
public interface IGraphData
{
    /// <summary>
    /// Gets the collection of nodes in the graph.
    /// </summary>
    IEnumerable<Node> Nodes { get; }

    /// <summary>
    /// Gets the collection of edges in the graph.
    /// </summary>
    IEnumerable<Edge> Edges { get; }

    /// <summary>
    /// Finds an existing node by name or adds a new one if it does not exist.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <returns>The found or newly added node.</returns>
    Node FindOrAddNode(string name);

    /// <summary>
    /// Finds an existing node by name or adds a new one if it does not exist, with optional metadata.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="metadata">Optional metadata to merge into the node if it is created.</param>
    /// <returns>The found or newly added node.</returns>
    Node FindOrAddNodeWithMeta(string name, Dictionary<string, object>? metadata = null);

    /// <summary>
    /// Finds an existing edge between two nodes or adds a new one if it does not exist.
    /// </summary>
    /// <param name="fromName">The name of the source node.</param>
    /// <param name="toName">The name of the target node.</param>
    /// <param name="primaryKey">The primary key for the edge.</param>
    /// <returns>The found or newly added edge.</returns>
    Edge FindOrAddEdge(string fromName, string toName, string primaryKey);

    /// <summary>
    /// Finds the shortest path between two nodes.
    /// </summary>
    /// <param name="from">The name of the source node.</param>
    /// <param name="to">The name of the target node.</param>
    /// <returns>A list of nodes representing the shortest path, or null if no path exists.</returns>
    List<Node>? ShortestPath(string from, string to);

    /// <summary>
    /// Updates or adds metadata to a node.
    /// </summary>
    /// <param name="nodeName">The name of the node.</param>
    /// <param name="key">The metadata key to update or add.</param>
    /// <param name="value">The metadata value.</param>
    void UpdateNodeMetadata(string nodeName, string key, object value);

    /// <summary>
    /// Updates or adds metadata to an edge.
    /// </summary>
    /// <param name="fromName">The source node name of the edge.</param>
    /// <param name="toName">The target node name of the edge.</param>
    /// <param name="key">The metadata key to update or add.</param>
    /// <param name="value">The metadata value.</param>
    void UpdateEdgeMetadata(string fromName, string toName, string key, object value);
}
