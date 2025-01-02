namespace ScriptRunner.Plugins.GraphTool.Enums;

/// <summary>
///     Specifies the types of plugins used to create or modify graph data structures.
/// </summary>
public enum PluginType
{
    /// <summary>
    ///     Represents a plugin for creating Entity-Relationship Diagrams (ERDs).
    /// </summary>
    Erd,

    /// <summary>
    ///     Represents a plugin for creating Class Diagrams.
    /// </summary>
    ClassDiagram,

    /// <summary>
    ///     Represents a plugin for managing Data Lineage graphs.
    /// </summary>
    Lineage
}