using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Models;

namespace ScriptRunner.Plugins.GraphTool.Interfaces.Plugins;

/// <summary>
/// Provides methods for managing lineage relationships in a graph structure.
/// </summary>
public interface ILineagePlugin
{
    /// <summary>
    /// Adds a node to the graph, optionally attaching metadata.
    /// </summary>
    /// <param name="nodeName">The name of the node to add.</param>
    /// <param name="graphData">The graph data structure to modify.</param>
    /// <param name="metadata">Optional metadata to attach to the node.</param>
    void AddNode(string nodeName, GraphData graphData, Dictionary<string, object>? metadata = null);

    /// <summary>
    /// Adds an edge between two nodes, optionally attaching metadata.
    /// </summary>
    /// <param name="fromNode">The source node of the edge.</param>
    /// <param name="toNode">The target node of the edge.</param>
    /// <param name="graphData">The graph data structure to modify.</param>
    /// <param name="metadata">Optional metadata to attach to the edge.</param>
    void AddEdge(string fromNode, string toNode, GraphData graphData, Dictionary<string, object>? metadata = null);

    /// <summary>
    /// Retrieves all outgoing edges starting from a specific node.
    /// </summary>
    /// <param name="nodeName">The name of the starting node.</param>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>A collection of edges originating from the specified node.</returns>
    IEnumerable<Edge> GetOutgoingEdges(string nodeName, GraphData graphData);

    /// <summary>
    /// Retrieves all incoming edges ending at a specific node.
    /// </summary>
    /// <param name="nodeName">The name of the target node.</param>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>A collection of edges ending at the specified node.</returns>
    IEnumerable<Edge> GetIncomingEdges(string nodeName, GraphData graphData);

    /// <summary>
    /// Retrieves all nodes from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the nodes.</param>
    /// <returns>An enumerable collection of nodes in the graph.</returns>
    IEnumerable<Node> GetNodes(GraphData graphData);

    /// <summary>
    /// Retrieves all edges from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>An enumerable collection of edges in the graph.</returns>
    IEnumerable<Edge> GetEdges(GraphData graphData);
}
