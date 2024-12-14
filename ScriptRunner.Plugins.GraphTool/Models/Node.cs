using System.Collections.Generic;

namespace ScriptRunner.Plugins.GraphTool.Models;

/// <summary>
///     Represents a node in the graph.
/// </summary>
public class Node
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Node" /> class.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="metadata">A dictionary containing additional metadata for the node.</param>
    public Node(string name, Dictionary<string, object> metadata)
    {
        Name = name;
        Metadata = metadata;
    }

    /// <summary>
    ///     Gets the name of the node.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets or sets additional metadata for the node.
    /// </summary>
    /// <remarks>
    ///     The metadata can contain additional information about the node, such as attributes, properties,
    ///     and labels, making the node more versatile for different graph types (e.g., ER diagrams, UML diagrams).
    /// </remarks>
    public Dictionary<string, object> Metadata { get; set; }
}