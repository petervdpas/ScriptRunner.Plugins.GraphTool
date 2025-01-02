using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Enums;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

/// <summary>
///     Defines the interface for creating and managing graph data structures from entities and relationships.
/// </summary>
public interface IGraphTool
{
    /// <summary>
    ///     Creates a graph using the specified plugin type and the provided entities and relationships.
    /// </summary>
    /// <param name="pluginType">
    ///     The type of plugin to use for creating the graph (e.g., ERD, ClassDiagram, Lineage).
    /// </param>
    /// <param name="entities">A collection of entities to include in the graph.</param>
    /// <param name="relationships">A collection of relationships between the entities.</param>
    /// <param name="entityList">
    ///     An optional array of entity names to focus on. If <c>null</c> or empty, all entities and relationships
    ///     are included in the graph. If provided, the graph is limited to the specified entities and their relationships.
    /// </param>
    /// <param name="bubbleUp">
    ///     Determines the direction of traversal for relationships. If <c>true</c>, relationships are traversed
    ///     from child to parent (upwards). If <c>false</c>, relationships are traversed from parent to child (downwards).
    /// </param>
    /// <param name="includeUnfocusedNeighbors">
    ///     Indicates whether to include indirect entities (i.e., entities not specified in <paramref name="entityList" />)
    ///     in the graph. If <c>true</c>, neighboring entities are included. Otherwise, only the specified entities
    ///     and their direct relationships are included.
    /// </param>
    /// <returns>
    ///     A <see cref="GraphData" /> structure representing the entities and their relationships, either fully or filtered
    ///     based on the provided <paramref name="entityList" />.
    /// </returns>
    GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        string[]? entityList = null,
        bool bubbleUp = true,
        bool includeUnfocusedNeighbors = false);

    /// <summary>
    ///     Creates a graph using the specified plugin type, provided entities, relationships, and optional metadata.
    /// </summary>
    /// <param name="pluginType">
    ///     The type of plugin to use for creating the graph (e.g., ERD, ClassDiagram, Lineage).
    /// </param>
    /// <param name="entities">A collection of entities to include in the graph.</param>
    /// <param name="relationships">A collection of relationships between the entities.</param>
    /// <param name="nodeMetadata">
    ///     Optional metadata for nodes. Each dictionary key represents the node identifier, and the value is another
    ///     dictionary containing key-value pairs of metadata.
    /// </param>
    /// <param name="edgeMetadata">
    ///     Optional metadata for edges. Each dictionary key represents the edge identifier (e.g., "fromNode->toNode"),
    ///     and the value is another dictionary containing key-value pairs of metadata.
    /// </param>
    /// <param name="entityList">
    ///     An optional array of entity names to focus on. If <c>null</c> or empty, all entities and relationships
    ///     are included in the graph. If provided, the graph is limited to the specified entities and their relationships.
    /// </param>
    /// <param name="bubbleUp">
    ///     Determines the direction of traversal for relationships. If <c>true</c>, relationships are traversed
    ///     from child to parent (upwards). If <c>false</c>, relationships are traversed from parent to child (downwards).
    /// </param>
    /// <param name="includeUnfocusedNeighbors">
    ///     Indicates whether to include indirect entities (i.e., entities not specified in <paramref name="entityList" />)
    ///     in the graph. If <c>true</c>, neighboring entities are included. Otherwise, only the specified entities
    ///     and their direct relationships are included.
    /// </param>
    /// <returns>
    ///     A <see cref="GraphData" /> structure representing the entities and their relationships, either fully or filtered
    ///     based on the provided <paramref name="entityList" />.
    /// </returns>
    GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        Dictionary<string, Dictionary<string, object>>? nodeMetadata,
        Dictionary<string, Dictionary<string, object>>? edgeMetadata,
        string[]? entityList = null,
        bool bubbleUp = true,
        bool includeUnfocusedNeighbors = false);

    /// <summary>
    ///     Exports the provided graph data to a JSON representation.
    /// </summary>
    /// <param name="graphData">The graph data to serialize.</param>
    /// <returns>
    ///     A JSON-formatted string representing the graph data, including nodes and edges.
    /// </returns>
    string GenerateJsonFromGraph(GraphData graphData);
}