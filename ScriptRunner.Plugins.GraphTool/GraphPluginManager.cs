using System;
using System.Collections.Generic;
using System.Linq;
using ScriptRunner.Plugins.GraphTool.Enums;
using ScriptRunner.Plugins.GraphTool.Interfaces;
using ScriptRunner.Plugins.GraphTool.Interfaces.Plugins;
using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Logging;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool;

/// <summary>
///     Manages plugins that create or modify graph data structures.
/// </summary>
public class GraphPluginManager : IGraphPluginManager
{
    private readonly IClassDiagramPlugin? _classDiagramPlugin;
    private readonly IErdPlugin? _erdPlugin;
    private readonly ILineagePlugin? _lineagePlugin;

    private readonly IPluginLogger? _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GraphPluginManager" /> class with optional plugins.
    /// </summary>
    /// <param name="logger">
    ///     The <see cref="IPluginLogger" /> instance used for logging messages within the Azure Key Vault operations.
    /// </param>
    /// <param name="erdPlugin">The plugin for generating ERD-based graph data (optional).</param>
    /// <param name="classDiagramPlugin">The plugin for generating class diagram-based graph data (optional).</param>
    /// <param name="lineagePlugin">The plugin for managing data lineage in graph data (optional).</param>
    public GraphPluginManager(
        IPluginLogger? logger,
        IErdPlugin? erdPlugin = null,
        IClassDiagramPlugin? classDiagramPlugin = null,
        ILineagePlugin? lineagePlugin = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _erdPlugin = erdPlugin;
        _classDiagramPlugin = classDiagramPlugin;
        _lineagePlugin = lineagePlugin;
    }

    /// <summary>
    ///     Creates a graph using the specified plugin type.
    /// </summary>
    /// <param name="pluginType">The type of plugin to use (e.g., ERD, ClassDiagram, Lineage).</param>
    /// <param name="entities">The entities to include in the graph.</param>
    /// <param name="relationships">The relationships between the entities.</param>
    /// <param name="nodeMetadata"></param>
    /// <param name="edgeMetadata"></param>
    /// <returns>
    ///     A <see cref="GraphData" /> object representing the generated graph.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if an unsupported or missing plugin type is provided.</exception>
    /// <exception cref="InvalidOperationException">Thrown if a required plugin is not configured.</exception>
    public GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        Dictionary<string, Dictionary<string, object>>? nodeMetadata = null,
        Dictionary<string, Dictionary<string, object>>? edgeMetadata = null)
    {
        if (_classDiagramPlugin == null && pluginType == PluginType.ClassDiagram)
            throw new InvalidOperationException("Class diagram plugin is not configured.");

        if (_erdPlugin == null && pluginType == PluginType.Erd)
            throw new InvalidOperationException("ERD plugin is not configured.");

        if (_lineagePlugin == null && pluginType == PluginType.Lineage)
            throw new InvalidOperationException("Lineage plugin is not configured.");

        // Materialize collections
        var entityList = entities.ToList();
        var relationshipList = relationships.ToList();

        // Initialize graph
        var graphData = new GraphData(new NodeFactory(entityList));

        // Dispatch based on a plugin type
        switch (pluginType)
        {
            case PluginType.Erd:
                HandleErdPlugin(entityList, relationshipList, graphData);
                break;

            case PluginType.ClassDiagram:
                HandleClassDiagramPlugin(entityList, relationshipList, graphData);
                break;

            case PluginType.Lineage:
                HandleLineagePlugin(entityList, relationshipList, graphData, nodeMetadata, edgeMetadata);
                break;

            default:
                throw new ArgumentException($"Unsupported plugin type: {pluginType}");
        }

        return graphData;
    }

    /// <summary>
    ///     Handles the creation of graph data using the ERD plugin.
    /// </summary>
    /// <param name="entities">The entities to include in the graph.</param>
    /// <param name="relationships">The relationships between the entities.</param>
    /// <param name="graphData">The graph data structure to populate.</param>
    private void HandleErdPlugin(List<Entity> entities, List<Relationship> relationships, GraphData graphData)
    {
        foreach (var entity in entities) _erdPlugin?.AddEntity(entity, graphData);
        foreach (var relationship in relationships) _erdPlugin?.AddRelationship(relationship, graphData);
    }

    /// <summary>
    ///     Handles the creation of graph data using the Class Diagram plugin.
    /// </summary>
    /// <param name="entities">The entities to include in the graph.</param>
    /// <param name="relationships">The relationships between the entities.</param>
    /// <param name="graphData">The graph data structure to populate.</param>
    private void HandleClassDiagramPlugin(List<Entity> entities, List<Relationship> relationships, GraphData graphData)
    {
        foreach (var entity in entities)
        {
            var metadata = entity.Attributes.ToDictionary(
                attribute => attribute.Key,
                attribute => attribute.Value);
            _classDiagramPlugin?.AddClass(entity.Name, metadata, graphData);
        }

        _classDiagramPlugin?.AddRelationships(relationships, graphData);
    }

    /// <summary>
    ///     Handles the creation of graph data using the Lineage plugin.
    /// </summary>
    /// <param name="entities">The entities to include in the graph.</param>
    /// <param name="relationships">The relationships between the entities.</param>
    /// <param name="graphData">The graph data structure to populate.</param>
    /// <param name="nodeMetadata">Optional metadata to attach to nodes.</param>
    /// <param name="edgeMetadata">Optional metadata to attach to edges.</param>
    private void HandleLineagePlugin(
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        GraphData graphData,
        Dictionary<string, Dictionary<string, object>>? nodeMetadata,
        Dictionary<string, Dictionary<string, object>>? edgeMetadata)
    {
        foreach (var entity in entities)
        foreach (var attribute in entity.Attributes)
        {
            var nodeName = $"{entity.Name}.{attribute.Key}";
            var customMetadata = nodeMetadata?.GetValueOrDefault(nodeName);

            _logger?.Debug($"Processing node: {nodeName} with metadata: {customMetadata}");
            graphData.FindOrAddNodeWithMeta(nodeName, customMetadata);
        }

        foreach (var relationship in relationships)
        {
            var fromField = $"{relationship.FromEntity}.{relationship.Key}";
            var toField = $"{relationship.ToEntity}.{relationship.Key}";
            var customEdgeMetadata = edgeMetadata?.GetValueOrDefault($"{fromField}->{toField}");

            _logger?.Debug($"Creating edge: {fromField} -> {toField} with metadata: {customEdgeMetadata}");
            var edge = graphData.FindOrAddEdge(fromField, toField, "lineage");
            edge.Metadata = MetadataUtils.MergeMetadata(edge.Metadata, customEdgeMetadata);
        }
    }
}