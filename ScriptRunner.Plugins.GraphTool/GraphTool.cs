using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ScriptRunner.Plugins.GraphTool.Enums;
using ScriptRunner.Plugins.GraphTool.Interfaces;
using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool;

/// <summary>
///     Tool for creating graph data structures from entities and relationships.
/// </summary>
public class GraphTool : IGraphTool
{
    private readonly IGraphPluginManager? _pluginManager;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GraphTool" /> class.
    /// </summary>
    /// <param name="pluginManager">The optional plugin manager used to handle graph operations.</param>
    public GraphTool(IGraphPluginManager? pluginManager = null)
    {
        _pluginManager = pluginManager;
    }

    /// <summary>
    ///     Creates a graph from the provided entities and relationships, typically representing database structures.
    /// </summary>
    /// <param name="pluginType">The plugin type to use (e.g., ERD, ClassDiagram, Lineage).</param>
    /// <param name="entities">The entities (e.g., tables or objects) to include in the graph.</param>
    /// <param name="relationships">The relationships between the entities (e.g., foreign key references).</param>
    /// <param name="entityList">
    ///     An optional array of entity names to focus on. If <c>null</c> or empty, all entities and relationships
    ///     will be included in the graph. If provided, the graph will be limited to the specified entities and
    ///     their relationships.
    /// </param>
    /// <param name="bubbleUp">
    ///     Determines the direction of traversal for relationships in the graph. If <c>true</c>, the traversal
    ///     follows relationships from child to parent (upwards). If <c>false</c>, the traversal follows relationships
    ///     from parent to child (downwards).
    /// </param>
    /// <param name="includeUnfocusedNeighbors">
    ///     Indicates whether to include indirect entities (i.e., entities not directly related) in the graph.
    ///     If <c>true</c>, neighboring entities not specified in <paramref name="entityList" /> will be included.
    ///     If <c>false</c>, only the specified entities and their direct relationships will be included.
    /// </param>
    /// <returns>
    ///     A <see cref="GraphData" /> structure representing the entities and their relationships, either fully
    ///     or filtered based on the provided <paramref name="entityList" />.
    /// </returns>
    public GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        string[]? entityList = null,
        bool bubbleUp = true,
        bool includeUnfocusedNeighbors = false)
    {
        return CreateGraph(
            pluginType,
            entities,
            relationships,
            null, // No node metadata
            null, // No edge metadata
            entityList,
            bubbleUp,
            includeUnfocusedNeighbors);
    }

    /// <summary>
    ///     Creates a graph from the provided entities, relationships, and metadata.
    /// </summary>
    /// <param name="pluginType">The plugin type to use (e.g., ERD, ClassDiagram, Lineage).</param>
    /// <param name="entities">The entities (e.g., tables or objects) to include in the graph.</param>
    /// <param name="relationships">The relationships between the entities (e.g., foreign key references).</param>
    /// <param name="nodeMetadata">Optional metadata for nodes.</param>
    /// <param name="edgeMetadata">Optional metadata for edges.</param>
    /// <param name="entityList">
    ///     An optional array of entity names to focus on. If <c>null</c> or empty, all entities and relationships
    ///     will be included in the graph. If provided, the graph will be limited to the specified entities and
    ///     their relationships.
    /// </param>
    /// <param name="bubbleUp">
    ///     Determines the direction of traversal for relationships in the graph. If <c>true</c>, the traversal
    ///     follows relationships from child to parent (upwards). If <c>false</c>, the traversal follows relationships
    ///     from parent to child (downwards).
    /// </param>
    /// <param name="includeUnfocusedNeighbors">
    ///     Indicates whether to include indirect entities (i.e., entities not directly related) in the graph.
    ///     If <c>true</c>, neighboring entities not specified in <paramref name="entityList" /> will be included.
    ///     If <c>false</c>, only the specified entities and their direct relationships will be included.
    /// </param>
    /// <returns>
    ///     A <see cref="GraphData" /> structure representing the entities and their relationships, either fully
    ///     or filtered based on the provided <paramref name="entityList" />.
    /// </returns>
    public GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        Dictionary<string, Dictionary<string, object>>? nodeMetadata,
        Dictionary<string, Dictionary<string, object>>? edgeMetadata,
        string[]? entityList = null,
        bool bubbleUp = true,
        bool includeUnfocusedNeighbors = false)
    {
        // Materialize entities and relationships to avoid multiple enumeration
        var entityListMaterialized = entities as IList<Entity> ?? entities.ToList();
        var relationshipListMaterialized = relationships as IList<Relationship> ?? relationships.ToList();

        // Use the plugin manager if available, or build the base graph directly
        var graphData = _pluginManager != null
            ? _pluginManager.CreateGraph(
                pluginType,
                entityListMaterialized,
                relationshipListMaterialized,
                nodeMetadata,
                edgeMetadata)
            : CreateBaseGraph(entityListMaterialized, relationshipListMaterialized);

        // No filtering? Return data as is
        if (entityList == null || entityList.Length == 0) return graphData;

        // Filter the graph based on the provided entity list
        var edgesToDocument = GetEdgesToDocument(graphData, bubbleUp, includeUnfocusedNeighbors, entityList);

        var filteredData = new GraphData(new NodeFactory(entityListMaterialized));

        // Add the filtered nodes and edges to the new graph
        foreach (var edge in edgesToDocument)
        {
            filteredData.FindOrAddNode(edge.From.Name);
            filteredData.FindOrAddNode(edge.To.Name);
            filteredData.FindOrAddEdge(edge.From.Name, edge.To.Name, edge.EdgeKey);
        }

        return filteredData;
    }

    /// <summary>
    ///     Exports the graph data to a JSON format.
    /// </summary>
    /// <param name="graphData">The graph data to serialize.</param>
    /// <returns>A JSON string representing the graph data.</returns>
    public string GenerateJsonFromGraph(GraphData graphData)
    {
        var graphRepresentation = new
        {
            Nodes = graphData.Nodes.Select(node => new
            {
                node.Name, node.Metadata
            }),
            Edges = graphData.Edges.Select(edge => new
            {
                From = edge.From.Name,
                To = edge.To.Name,
                edge.Metadata
            })
        };

        return JsonConvert.SerializeObject(graphRepresentation, Formatting.Indented);
    }

    /// <summary>
    ///     Creates the base graph by adding all entities and relationships.
    /// </summary>
    /// <param name="entities">The entities to include.</param>
    /// <param name="relationships">The relationships to include.</param>
    /// <returns>The constructed <see cref="GraphData" />.</returns>
    private static GraphData CreateBaseGraph(IList<Entity> entities, IList<Relationship> relationships)
    {
        var nodeFactory = new NodeFactory(entities);
        var graphData = new GraphData(nodeFactory);

        foreach (var entity in entities) graphData.FindOrAddNode(entity.Name);
        foreach (var relationship in relationships)
            graphData.FindOrAddEdge(relationship.FromEntity, relationship.ToEntity, relationship.Key);

        return graphData;
    }

    /// <summary>
    ///     Gets the edges to be documented from the graph data.
    /// </summary>
    /// <param name="data">The graph data structure.</param>
    /// <param name="bubbleUp">Determines the direction of traversal for relationships.</param>
    /// <param name="includeUnfocusedNeighbors">Indicates whether to include indirect entities in the graph.</param>
    /// <param name="focusNodes">The entities to focus on.</param>
    /// <returns>A collection of edges to document.</returns>
    private static IEnumerable<Edge> GetEdgesToDocument(
        GraphData data,
        bool bubbleUp,
        bool includeUnfocusedNeighbors,
        string[] focusNodes)
    {
        // Only work with focus nodes that exist in the graph
        var nodes = data.Nodes.Where(n => focusNodes.Contains(n.Name)).ToList();

        // If no focus nodes are provided, document everything
        if (nodes.Count == 0) return GetEdges(data, data.Nodes, bubbleUp, includeUnfocusedNeighbors);

        var relevantNodes = new List<Node>();

        // Traverse from each focus node to others and collect the paths
        foreach (var fromNode in nodes)
        foreach (var toNode in nodes)
            if (fromNode != toNode)
            {
                var path = data.ShortestPath(fromNode.Name, toNode.Name);
                if (path != null) relevantNodes.AddRange(path);
            }

        // Ensure we have a distinct list of nodes to document
        relevantNodes = relevantNodes.Distinct().ToList();

        // Now retrieve the edges involving those nodes
        return GetEdges(data, relevantNodes, bubbleUp, includeUnfocusedNeighbors);
    }

    /// <summary>
    ///     Gets the edges from the graph data based on the specified entities.
    /// </summary>
    /// <param name="data">The graph data structure.</param>
    /// <param name="nodes">The entities to include in the edge search.</param>
    /// <param name="bubbleUp">Determines the direction of traversal for relationships.</param>
    /// <param name="includeUnfocusedNeighbors">Indicates whether to include indirect entities in the graph.</param>
    /// <returns>A collection of edges.</returns>
    private static IEnumerable<Edge> GetEdges(
        GraphData data,
        IEnumerable<Node> nodes,
        bool bubbleUp,
        bool includeUnfocusedNeighbors)
    {
        var edges = new List<Edge>();
        var enumerable = nodes.ToList();
        foreach (var node in enumerable)
            edges.AddRange(data.Edges
                .Where(e => (bubbleUp ? e.To == node : e.From == node) &&
                            (includeUnfocusedNeighbors || enumerable.Contains(bubbleUp ? e.From : e.To))));

        return edges.Distinct();
    }
}