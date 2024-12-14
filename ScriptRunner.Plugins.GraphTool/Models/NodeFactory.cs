using System;
using System.Collections.Generic;
using System.Linq;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Models;

/// <summary>
///     Factory for creating nodes from entities.
/// </summary>
public class NodeFactory
{
    private readonly Dictionary<string, Entity> _entityLookup;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeFactory" /> class.
    /// </summary>
    /// <param name="entities">
    ///     Optional collection of entities to populate the factory.
    ///     Can be null or empty if no preloaded entities are required.
    /// </param>
    public NodeFactory(IEnumerable<Entity>? entities = null)
    {
        _entityLookup = entities?.ToDictionary(entity => entity.Name, StringComparer.InvariantCultureIgnoreCase)
                        ?? new Dictionary<string, Entity>();
    }

    /// <summary>
    ///     Creates a new node with the specified name.
    /// </summary>
    /// <param name="name">The name of the node to create.</param>
    /// <returns>The newly created node, or null if the entity does not exist.</returns>
    public Node CreateNode(string name)
    {
        if (!_entityLookup.TryGetValue(name, out var entity))
            return new Node(name, MetadataUtils.GenerateDynamicMetadata(name));

        // Use ConvertEntityToMetadata to populate metadata for the node
        var metadata = MetadataUtils.ConvertEntityToMetadata(entity);
        return new Node(name, metadata);

        // Fallback for dynamically created nodes
    }

    /// <summary>
    ///     Creates a new node with the specified name and optional custom metadata.
    /// </summary>
    /// <param name="name">The name of the node to create.</param>
    /// <param name="metadata">Optional custom metadata to merge with node's default metadata.</param>
    /// <returns>A new <see cref="Node" /> instance populated with merged metadata.</returns>
    public Node CreateNodeWithMeta(string name, Dictionary<string, object>? metadata = null)
    {
        // Extract entity and field information
        var entityName = name.Split('.')[0];
        if (!_entityLookup.TryGetValue(entityName, out var entity))
        {
            var dynamicMetadata = MetadataUtils.GenerateDynamicMetadata(name);
            return new Node(name, MetadataUtils.MergeMetadata(dynamicMetadata, metadata));
        }

        var fieldName = name.Split('.').LastOrDefault();
        if (fieldName == null || !entity.Attributes.TryGetValue(fieldName, out var attribute))
        {
            var dynamicMetadata = MetadataUtils.GenerateDynamicMetadata(name);
            return new Node(name, MetadataUtils.MergeMetadata(dynamicMetadata, metadata));
        }

        var fieldMetadata = MetadataUtils.GenerateDynamicMetadata(name);
        fieldMetadata["Type"] = attribute;

        return new Node(name, MetadataUtils.MergeMetadata(fieldMetadata, metadata));
    }
}