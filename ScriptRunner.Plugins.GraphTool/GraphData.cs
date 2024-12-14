using System;
using System.Collections.Generic;
using System.Linq;
using ScriptRunner.Plugins.GraphTool.Interfaces;
using ScriptRunner.Plugins.GraphTool.Models;

namespace ScriptRunner.Plugins.GraphTool;

/// <summary>
///     Implementation of <see cref="IGraphData" />.
///     Representing graph data structure operations, including node and edge management.
/// </summary>
public class GraphData : IGraphData
{
    private readonly List<Edge> _edges = [];
    private readonly NodeFactory _nodeFactory;
    private readonly List<Node> _nodes = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="GraphData" /> class.
    /// </summary>
    /// <param name="nodeFactory">The factory used to create nodes in the graph.</param>
    public GraphData(NodeFactory nodeFactory)
    {
        _nodeFactory = nodeFactory;
    }

    /// <summary>
    ///     Gets the collection of nodes in the graph.
    /// </summary>
    public IEnumerable<Node> Nodes => _nodes;

    /// <summary>
    ///     Gets the collection of edges in the graph.
    /// </summary>
    public IEnumerable<Edge> Edges => _edges;

    /// <summary>
    ///     Finds an existing node by name or adds a new one if it does not exist.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <returns>The found or newly added node.</returns>
    public Node FindOrAddNode(string name)
    {
        var node = _nodes.FirstOrDefault(n => n.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (node != null) return node;

        node = _nodeFactory.CreateNode(name);
        _nodes.Add(node);
        return node;
    }

    /// <summary>
    ///     Finds an existing node by name or adds a new one if it does not exist, with optional metadata.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="metadata">Optional metadata to merge into the node if it is created.</param>
    /// <returns>The found or newly added node.</returns>
    public Node FindOrAddNodeWithMeta(string name, Dictionary<string, object>? metadata = null)
    {
        var node = _nodes.FirstOrDefault(n => n.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (node != null)
        {
            // Merge provided metadata with existing metadata
            if (metadata != null) node.Metadata = MetadataUtils.MergeMetadata(node.Metadata, metadata);
            return node;
        }

        // If a node doesn't exist, create with metadata
        node = _nodeFactory.CreateNodeWithMeta(name, metadata);
        _nodes.Add(node);
        return node;
    }

    /// <summary>
    ///     Finds an existing edge between two nodes or adds a new one if it does not exist.
    /// </summary>
    /// <param name="fromName">The name of the source node.</param>
    /// <param name="toName">The name of the target node.</param>
    /// <param name="primaryKey">The primary key for the edge.</param>
    /// <returns>The found or newly added edge.</returns>
    public Edge FindOrAddEdge(string fromName, string toName, string primaryKey)
    {
        var edge = _edges.FirstOrDefault(
            n => n.From.Name.Equals(fromName, StringComparison.InvariantCultureIgnoreCase) &&
                 n.To.Name.Equals(toName, StringComparison.InvariantCultureIgnoreCase));
        if (edge != null) return edge;

        var from = FindOrAddNode(fromName);
        var to = FindOrAddNode(toName);
        edge = new Edge(from, to, primaryKey);
        _edges.Add(edge);
        return edge;
    }

    /// <summary>
    ///     Finds the shortest path between two nodes.
    /// </summary>
    /// <param name="from">The name of the source node.</param>
    /// <param name="to">The name of the target node.</param>
    /// <returns>A list of nodes representing the shortest path, or null if no path exists.</returns>
    public List<Node>? ShortestPath(string from, string to)
    {
        var fromNode = FindNode(from);
        var toNode = FindNode(to);

        // Initialize values
        var distance = new Dictionary<Node, double>();
        var previous = new Dictionary<Node, Node>();
        var unvisited = new List<Node>(_nodes);

        foreach (var node in _nodes) distance[node] = double.PositiveInfinity;

        distance[fromNode] = 0;

        while (unvisited.Count > 0)
        {
            var current = unvisited.OrderBy(n => distance[n]).First();
            unvisited.Remove(current);

            if (current == toNode) break;

            var neighbors = _edges.Where(e => e.From == current).Select(e => e.To);
            foreach (var neighbor in neighbors)
            {
                var tentativeDistance = distance[current] + 1;
                if (!(tentativeDistance < distance[neighbor])) continue;

                distance[neighbor] = tentativeDistance;
                previous[neighbor] = current;
            }
        }

        var path = new List<Node>();
        var currentNode = toNode;
        while (previous.ContainsKey(currentNode))
        {
            path.Add(currentNode);
            currentNode = previous[currentNode];
        }

        path.Add(fromNode);
        path.Reverse();

        return path.Count > 1 ? path : null;
    }

    /// <summary>
    ///     Updates or adds metadata to a node.
    /// </summary>
    /// <param name="nodeName">The name of the node.</param>
    /// <param name="key">The metadata key to update or add.</param>
    /// <param name="value">The metadata value.</param>
    public void UpdateNodeMetadata(string nodeName, string key, object value)
    {
        var node = FindOrAddNode(nodeName);
        node.Metadata[key] = value;
    }

    /// <summary>
    ///     Updates or adds metadata to an edge.
    /// </summary>
    /// <param name="fromName">The source node name of the edge.</param>
    /// <param name="toName">The target node name of the edge.</param>
    /// <param name="key">The metadata key to update or add.</param>
    /// <param name="value">The metadata value.</param>
    public void UpdateEdgeMetadata(string fromName, string toName, string key, object value)
    {
        var edge = _edges.FirstOrDefault(
            e => e.From.Name.Equals(fromName, StringComparison.InvariantCultureIgnoreCase) &&
                 e.To.Name.Equals(toName, StringComparison.InvariantCultureIgnoreCase));

        if (edge == null)
            throw new InvalidOperationException($"Edge between '{fromName}' and '{toName}' does not exist.");

        edge.Metadata[key] = value;
    }

    /// <summary>
    ///     Finds an existing node by name.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <returns>The found node.</returns>
    /// <exception cref="Exception">Thrown when the node is not found.</exception>
    private Node FindNode(string name)
    {
        try
        {
            return _nodes.Single(n => n.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception)
        {
            Console.Write(name);
            throw;
        }
    }
}