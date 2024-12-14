using System.Collections.Generic;
using System.Linq;
using ScriptRunner.Plugins.GraphTool.Interfaces.Plugins;
using ScriptRunner.Plugins.GraphTool.Models;

namespace ScriptRunner.Plugins.GraphTool.Plugins;

/// <summary>
///     Plugin for managing general-purpose lineage in a graph structure.
/// </summary>
public class LineagePlugin : ILineagePlugin
{
    /// <summary>
    ///     Adds a node to the graph, dynamically attaching metadata.
    /// </summary>
    /// <param name="nodeName">The name of the node to add.</param>
    /// <param name="graphData">The graph data structure to modify.</param>
    /// <param name="metadata">Optional metadata to attach to the node.</param>
    public void AddNode(string nodeName, GraphData graphData, Dictionary<string, object>? metadata = null)
    {
        graphData.FindOrAddNodeWithMeta(nodeName, metadata);
    }

    /// <summary>
    ///     Adds an edge between two nodes, dynamically attaching metadata.
    /// </summary>
    /// <param name="fromNode">The source node of the edge.</param>
    /// <param name="toNode">The target node of the edge.</param>
    /// <param name="graphData">The graph data structure to modify.</param>
    /// <param name="metadata">Optional metadata to attach to the edge.</param>
    public void AddEdge(string fromNode, string toNode, GraphData graphData,
        Dictionary<string, object>? metadata = null)
    {
        var edge = graphData.FindOrAddEdge(fromNode, toNode, "lineage");
        edge.Metadata = MetadataUtils.MergeMetadata(edge.Metadata, metadata);
    }

    /// <summary>
    ///     Retrieves all edges starting from a specific node.
    /// </summary>
    /// <param name="nodeName">The name of the starting node.</param>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>A collection of edges originating from the specified node.</returns>
    public IEnumerable<Edge> GetOutgoingEdges(string nodeName, GraphData graphData)
    {
        var node = graphData.Nodes.FirstOrDefault(n => n.Name == nodeName);
        return node == null ? [] : graphData.Edges.Where(e => e.From == node);
    }

    /// <summary>
    ///     Retrieves all edges ending at a specific node.
    /// </summary>
    /// <param name="nodeName">The name of the target node.</param>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>A collection of edges ending at the specified node.</returns>
    public IEnumerable<Edge> GetIncomingEdges(string nodeName, GraphData graphData)
    {
        var node = graphData.Nodes.FirstOrDefault(n => n.Name == nodeName);
        return node == null ? [] : graphData.Edges.Where(e => e.To == node);
    }

    /// <summary>
    ///     Retrieves all nodes (entities) from the graph data.
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
}