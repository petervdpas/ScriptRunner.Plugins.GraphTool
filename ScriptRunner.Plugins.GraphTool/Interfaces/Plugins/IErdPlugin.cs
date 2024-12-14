using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Interfaces.Plugins;

/// <summary>
/// Provides methods for creating and managing Entity-Relationship Diagram (ERD) graph data.
/// </summary>
public interface IErdPlugin
{
    /// <summary>
    /// Adds an entity to the graph, including its attributes as metadata.
    /// </summary>
    /// <param name="entity">The entity to add to the graph.</param>
    /// <param name="graphData">The graph data structure to which the entity is added.</param>
    void AddEntity(Entity entity, GraphData graphData);

    /// <summary>
    /// Adds a relationship between two entities in the graph.
    /// </summary>
    /// <param name="relationship">The relationship to add, specifying the source and target entities.</param>
    /// <param name="graphData">The graph data structure to which the relationship is added.</param>
    void AddRelationship(Relationship relationship, GraphData graphData);

    /// <summary>
    /// Retrieves all nodes (entities) from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the nodes.</param>
    /// <returns>An enumerable collection of nodes in the graph.</returns>
    IEnumerable<Node> GetNodes(GraphData graphData);

    /// <summary>
    /// Retrieves all edges (relationships) from the graph data.
    /// </summary>
    /// <param name="graphData">The graph data structure containing the edges.</param>
    /// <returns>An enumerable collection of edges in the graph.</returns>
    IEnumerable<Edge> GetEdges(GraphData graphData);
}