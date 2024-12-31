using System;
using System.Collections.Generic;

namespace ScriptRunner.Plugins.GraphTool.Models.Generics;

/// <summary>
///     Represents a node in a lineage-specific graph, containing lineage-specific metadata.
/// </summary>
public class LineageNode
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LineageNode" /> class.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    public LineageNode(string name, Dictionary<string, object>? metadata = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    /// <summary>
    ///     Gets or sets the name of the node.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the metadata associated with this node, specifically for lineage.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; }

    /// <summary>
    ///     Updates the metadata of the node with new key-value pairs.
    /// </summary>
    /// <param name="newMetadata">The metadata to merge with the existing metadata.</param>
    public void UpdateMetadata(Dictionary<string, object> newMetadata)
    {
        foreach (var kvp in newMetadata) Metadata[kvp.Key] = kvp.Value;
    }
}