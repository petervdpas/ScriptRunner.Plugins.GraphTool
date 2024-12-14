using System.Collections.Generic;

namespace ScriptRunner.Plugins.GraphTool.Models;

/// <summary>
///     Represents an edge in the graph, which connects two nodes.
/// </summary>
public class Edge
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Edge" /> class.
    /// </summary>
    /// <param name="from">The source node of the edge.</param>
    /// <param name="to">The target node of the edge.</param>
    /// <param name="edgeKey">A unique identifier or key for the edge.</param>
    /// <param name="metadata">
    ///     An optional dictionary containing metadata associated with the edge.
    ///     If no metadata is provided, an empty dictionary is initialized.
    /// </param>
    public Edge(Node from, Node to, string edgeKey, Dictionary<string, object>? metadata = null)
    {
        From = from;
        To = to;
        EdgeKey = edgeKey;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    /// <summary>
    ///     Gets the source node of the edge.
    /// </summary>
    public Node From { get; }

    /// <summary>
    ///     Gets the target node of the edge.
    /// </summary>
    public Node To { get; }

    /// <summary>
    ///     Gets the primary key of the edge, which uniquely identifies it.
    /// </summary>
    public string EdgeKey { get; }

    /// <summary>
    ///     Gets or sets the metadata associated with the edge.
    /// </summary>
    /// <remarks>
    ///     Metadata provides additional context or properties for the edge, such as weights,
    ///     labels, or custom attributes. This can be useful for graph analysis or visualization.
    /// </remarks>
    public Dictionary<string, object> Metadata { get; set; }
}