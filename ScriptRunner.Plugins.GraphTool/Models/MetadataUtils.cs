using System.Collections.Generic;
using System.Linq;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Models;

/// <summary>
///     Utility class for metadata handling.
///     Provides methods for merging, converting, and generating metadata for nodes and entities.
/// </summary>
public static class MetadataUtils
{
    /// <summary>
    ///     Merges a base metadata dictionary with an additional metadata dictionary.
    ///     Custom values in the additional metadata will override those in the base metadata.
    /// </summary>
    /// <param name="baseMetadata">The base metadata dictionary.</param>
    /// <param name="additionalMetadata">The additional metadata dictionary to merge with the base.</param>
    /// <returns>A merged dictionary with combined metadata.</returns>
    public static Dictionary<string, object> MergeMetadata(
        Dictionary<string, object> baseMetadata,
        Dictionary<string, object>? additionalMetadata)
    {
        var merged = new Dictionary<string, object>(baseMetadata);
        if (additionalMetadata == null) return merged;

        foreach (var (key, value) in additionalMetadata)
            merged[key] = value;

        return merged;
    }

    /// <summary>
    ///     Converts an entity's attributes into a metadata dictionary.
    ///     Includes derived properties, such as the attribute count.
    /// </summary>
    /// <param name="entity">The entity to convert into metadata.</param>
    /// <returns>A dictionary representing the entity's metadata.</returns>
    public static Dictionary<string, object> ConvertEntityToMetadata(Entity entity)
    {
        var metadata = new Dictionary<string, object>(entity.Attributes)
        {
            { "AttributeCount", entity.Attributes.Count }
        };
        return metadata;
    }

    /// <summary>
    ///     Generates default metadata for a dynamically created node.
    ///     Includes basic context derived from the node's name.
    /// </summary>
    /// <param name="name">The name of the node to generate metadata for.</param>
    /// <returns>A dictionary containing default metadata for the node.</returns>
    public static Dictionary<string, object> GenerateDynamicMetadata(string name)
    {
        var parts = name.Split('.');
        return new Dictionary<string, object>
        {
            { "System", parts.ElementAtOrDefault(0) ?? "Unknown" },
            { "Table", parts.ElementAtOrDefault(1) ?? "Unknown" },
            { "Field", parts.ElementAtOrDefault(2) ?? "Unknown" }
        };
    }
}