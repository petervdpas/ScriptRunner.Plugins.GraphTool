using System;
using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

/// <summary>
///     Defines an interface for persisting graph data, including nodes and edges, to a storage system.
/// </summary>
/// <typeparam name="TNodeMetadata">The type of metadata associated with nodes.</typeparam>
/// <typeparam name="TEdgeMetadata">The type of metadata associated with edges.</typeparam>
public interface IGraphDataPersistence<in TNodeMetadata, in TEdgeMetadata>
    where TNodeMetadata : class
    where TEdgeMetadata : class
{
    /// <summary>
    ///     Sets the database instance to be used for persistence operations.
    /// </summary>
    /// <param name="database">The database instance to use for storage.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the database instance is already set or not properly initialized.
    /// </exception>
    void SetDatabase(IDatabase database);

    /// <summary>
    ///     Saves the graph data to the database.
    /// </summary>
    /// <param name="graphData">The graph data to persist, including nodes and edges.</param>
    /// <exception cref="InvalidOperationException">Thrown if the database instance is not set.</exception>
    void SaveGraph(GraphData graphData);

    /// <summary>
    ///     Loads graph data from the database.
    /// </summary>
    /// <param name="nodeFactory">A factory for creating and managing nodes during graph reconstruction.</param>
    /// <returns>A <see cref="GraphData" /> object representing the loaded graph.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the database instance is not set.</exception>
    GraphData LoadGraph(NodeFactory nodeFactory);

    /// <summary>
    ///     Adds or updates a node in the database.
    /// </summary>
    /// <param name="nodeName">The name of the node to add or update.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    /// <exception cref="InvalidOperationException">Thrown if the database instance is not set.</exception>
    void AddOrUpdateNode(string nodeName, TNodeMetadata metadata);

    /// <summary>
    ///     Deletes a node from the database.
    /// </summary>
    /// <param name="nodeName">The name of the node to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown if the database instance is not set.</exception>
    void DeleteNode(string nodeName);

    /// <summary>
    ///     Adds or updates an edge in the database.
    /// </summary>
    /// <param name="fromNode">The source node of the edge.</param>
    /// <param name="toNode">The target node of the edge.</param>
    /// <param name="edgeKey">The unique key identifying the edge.</param>
    /// <param name="metadata">The metadata associated with the edge.</param>
    /// <exception cref="InvalidOperationException">Thrown if the database instance is not set.</exception>
    void AddOrUpdateEdge(string fromNode, string toNode, string edgeKey, TEdgeMetadata metadata);

    /// <summary>
    ///     Deletes an edge from the database.
    /// </summary>
    /// <param name="fromNode">The source node of the edge.</param>
    /// <param name="toNode">The target node of the edge.</param>
    /// <param name="edgeKey">The unique key identifying the edge.</param>
    /// <exception cref="InvalidOperationException">Thrown if the database instance is not set.</exception>
    void DeleteEdge(string fromNode, string toNode, string edgeKey);
}