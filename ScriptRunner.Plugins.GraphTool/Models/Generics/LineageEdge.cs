using System;
using System.Collections.Generic;

namespace ScriptRunner.Plugins.GraphTool.Models.Generics;

/// <summary>
///     Represents an edge in a lineage-specific graph, connecting two lineage nodes.
/// </summary>
public class LineageEdge
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LineageEdge" /> class.
    /// </summary>
    /// <param name="from">The source node of the edge.</param>
    /// <param name="to">The target node of the edge.</param>
    /// <param name="edgeKey">The key uniquely identifying this edge.</param>
    /// <param name="metadata">The metadata associated with the edge.</param>
    public LineageEdge(LineageNode from, LineageNode to, string edgeKey, Dictionary<string, object>? metadata = null)
    {
        From = from ?? throw new ArgumentNullException(nameof(from));
        To = to ?? throw new ArgumentNullException(nameof(to));
        EdgeKey = edgeKey ?? throw new ArgumentNullException(nameof(edgeKey));
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    /// <summary>
    ///     Gets or sets the source node of the edge.
    /// </summary>
    public LineageNode From { get; set; }

    /// <summary>
    ///     Gets or sets the target node of the edge.
    /// </summary>
    public LineageNode To { get; set; }

    /// <summary>
    ///     Gets or sets the key uniquely identifying this edge.
    /// </summary>
    public string EdgeKey { get; set; }

    /// <summary>
    ///     Gets or sets the metadata associated with this edge.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; }

    /// <summary>
    ///     Updates the metadata of the edge with new key-value pairs.
    /// </summary>
    /// <param name="newMetadata">The metadata to merge with the existing metadata.</param>
    public void UpdateMetadata(Dictionary<string, object> newMetadata)
    {
        foreach (var kvp in newMetadata) Metadata[kvp.Key] = kvp.Value;
    }
}