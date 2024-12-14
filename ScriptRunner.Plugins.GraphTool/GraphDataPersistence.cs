using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using ScriptRunner.Plugins.GraphTool.Interfaces;
using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.GraphTool;

/// <summary>
///     Generic persistence class for storing graph data into SQLite.
/// </summary>
/// <typeparam name="TNodeMetadata">The type of metadata associated with nodes.</typeparam>
/// <typeparam name="TEdgeMetadata">The type of metadata associated with edges.</typeparam>
public class GraphDataPersistence<TNodeMetadata, TEdgeMetadata>
    : IGraphDataPersistence<TNodeMetadata, TEdgeMetadata>
    where TNodeMetadata : class
    where TEdgeMetadata : class
{
    private IDatabase? _database;

    /// <summary>
    ///     Sets the database instance for the persistence layer.
    /// </summary>
    /// <param name="database">The SQLite database instance to use.</param>
    public void SetDatabase(IDatabase database)
    {
        if (_database != null)
            throw new InvalidOperationException("Database instance has already been set.");

        _database = database;

        // Initialize database schema
        InitializeSchema();
    }

    /// <summary>
    ///     Saves a graph to the SQLite database.
    /// </summary>
    /// <param name="graphData">The graph data to save.</param>
    public void SaveGraph(GraphData graphData)
    {
        if (_database == null) throw new InvalidOperationException("Database instance has not been set.");

        _database.ExecuteNonQuery("BEGIN TRANSACTION;");

        try
        {
            // Save nodes
            foreach (var node in graphData.Nodes)
            {
                if (IsNodeDuplicate(node)) continue;

                var metadataJson = SerializeMetadata((TNodeMetadata)(object)node.Metadata);
                const string query = "INSERT OR REPLACE INTO Nodes (Name, Metadata) VALUES (@name, @metadata)";
                _database.ExecuteNonQuery(query, new Dictionary<string, object>
                {
                    { "@name", node.Name },
                    { "@metadata", metadataJson }
                });
            }

            // Save edges
            foreach (var edge in graphData.Edges)
            {
                if (IsEdgeDuplicate(edge)) continue;

                var metadataJson = SerializeMetadata((TEdgeMetadata)(object)edge.Metadata);
                const string query =
                    "INSERT OR REPLACE INTO Edges (FromNode, ToNode, EdgeKey, Metadata) VALUES (@from, @to, @key, @metadata)";
                _database.ExecuteNonQuery(query, new Dictionary<string, object>
                {
                    { "@from", edge.From.Name },
                    { "@to", edge.To.Name },
                    { "@key", edge.EdgeKey },
                    { "@metadata", metadataJson }
                });
            }

            _database.ExecuteNonQuery("COMMIT;");
        }
        catch
        {
            _database.ExecuteNonQuery("ROLLBACK;");
            throw;
        }
    }

    /// <summary>
    ///     Loads a graph from the SQLite database.
    /// </summary>
    /// <param name="nodeFactory">The factory for creating nodes.</param>
    /// <returns>The loaded graph data.</returns>
    public GraphData LoadGraph(NodeFactory nodeFactory)
    {
        if (_database == null) throw new InvalidOperationException("Database instance has not been set.");

        var graphData = new GraphData(nodeFactory);

        // Load nodes
        const string nodesQuery = "SELECT Name, Metadata FROM Nodes";
        var nodesTable = _database.ExecuteQuery(nodesQuery);
        foreach (DataRow row in nodesTable.Rows)
        {
            var name = row["Name"].ToString()!;
            var metadataJson = row["Metadata"].ToString()!;
            var metadata = DeserializeMetadata<TNodeMetadata>(metadataJson);

            var node = graphData.FindOrAddNode(name);
            node.Metadata = (Dictionary<string, object>)(object)metadata;
        }

        // Load edges
        const string edgesQuery = "SELECT FromNode, ToNode, EdgeKey, Metadata FROM Edges";
        var edgesTable = _database.ExecuteQuery(edgesQuery);
        foreach (DataRow row in edgesTable.Rows)
        {
            var from = row["FromNode"].ToString()!;
            var to = row["ToNode"].ToString()!;
            var edgeKey = row["EdgeKey"].ToString()!;
            var metadataJson = row["Metadata"].ToString()!;
            var metadata = DeserializeMetadata<TEdgeMetadata>(metadataJson);

            var edge = graphData.FindOrAddEdge(from, to, edgeKey);
            edge.Metadata = (Dictionary<string, object>)(object)metadata;
        }

        return graphData;
    }

    /// <summary>
    ///     Adds or updates a node in the database.
    /// </summary>
    /// <param name="nodeName">The name of the node to add or update.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    public void AddOrUpdateNode(string nodeName, TNodeMetadata metadata)
    {
        if (_database == null) throw new InvalidOperationException("Database instance has not been set.");

        var metadataJson = SerializeMetadata(metadata);
        const string query = """
                             INSERT INTO Nodes (Name, Metadata)
                             VALUES (@name, @metadata)
                             ON CONFLICT(Name) DO UPDATE SET Metadata = @metadata
                             """;

        _database.ExecuteNonQuery(query, new Dictionary<string, object>
        {
            { "@name", nodeName },
            { "@metadata", metadataJson }
        });
    }

    /// <summary>
    ///     Deletes a node from the database.
    /// </summary>
    /// <param name="nodeName">The name of the node to delete.</param>
    public void DeleteNode(string nodeName)
    {
        if (_database == null) throw new InvalidOperationException("Database instance has not been set.");

        _database.ExecuteNonQuery("BEGIN TRANSACTION;");

        try
        {
            const string deleteEdgesQuery = "DELETE FROM Edges WHERE FromNode = @node OR ToNode = @node";
            _database.ExecuteNonQuery(deleteEdgesQuery, new Dictionary<string, object> { { "@node", nodeName } });

            const string deleteNodeQuery = "DELETE FROM Nodes WHERE Name = @node";
            _database.ExecuteNonQuery(deleteNodeQuery, new Dictionary<string, object> { { "@node", nodeName } });

            _database.ExecuteNonQuery("COMMIT;");
        }
        catch
        {
            _database.ExecuteNonQuery("ROLLBACK;");
            throw;
        }
    }

    /// <summary>
    ///     Adds or updates an edge in the database.
    /// </summary>
    /// <param name="fromNode">The source node of the edge.</param>
    /// <param name="toNode">The target node of the edge.</param>
    /// <param name="edgeKey">The key that uniquely identifies the edge.</param>
    /// <param name="metadata">The metadata associated with the edge.</param>
    public void AddOrUpdateEdge(string fromNode, string toNode, string edgeKey, TEdgeMetadata metadata)
    {
        if (_database == null) throw new InvalidOperationException("Database instance has not been set.");

        var metadataJson = SerializeMetadata(metadata);
        const string query = """
                             INSERT INTO Edges (FromNode, ToNode, EdgeKey, Metadata)
                             VALUES (@from, @to, @key, @metadata)
                             ON CONFLICT(FromNode, ToNode, EdgeKey) DO UPDATE SET Metadata = @metadata
                             """;

        _database.ExecuteNonQuery(query, new Dictionary<string, object>
        {
            { "@from", fromNode },
            { "@to", toNode },
            { "@key", edgeKey },
            { "@metadata", metadataJson }
        });
    }

    /// <summary>
    ///     Deletes an edge from the database.
    /// </summary>
    /// <param name="fromNode">The source node of the edge.</param>
    /// <param name="toNode">The target node of the edge.</param>
    /// <param name="edgeKey">The key that uniquely identifies the edge.</param>
    public void DeleteEdge(string fromNode, string toNode, string edgeKey)
    {
        if (_database == null) throw new InvalidOperationException("Database instance has not been set.");

        const string query = "DELETE FROM Edges WHERE FromNode = @from AND ToNode = @to AND EdgeKey = @key";
        _database.ExecuteNonQuery(query, new Dictionary<string, object>
        {
            { "@from", fromNode },
            { "@to", toNode },
            { "@key", edgeKey }
        });
    }

    /// <summary>
    ///     Checks if a node already exists in the database.
    /// </summary>
    /// <param name="node">The node to check for duplication.</param>
    /// <returns><c>true</c> if the node exists; otherwise, <c>false</c>.</returns>
    private bool IsNodeDuplicate(Node node)
    {
        const string query = """
                             SELECT COUNT(*) 
                             FROM Nodes 
                             WHERE Name = @name 
                               AND json_extract(Metadata, '$') = @metadata
                             """;

        var metadataJson = SerializeMetadata(node.Metadata);
        var result = _database?.ExecuteScalar(query, new Dictionary<string, object>
        {
            { "@name", node.Name },
            { "@metadata", metadataJson }
        });

        return Convert.ToInt32(result) > 0;
    }

    /// <summary>
    ///     Checks if an edge already exists in the database.
    /// </summary>
    /// <param name="edge">The edge to check for duplication.</param>
    /// <returns><c>true</c> if the edge exists; otherwise, <c>false</c>.</returns>
    private bool IsEdgeDuplicate(Edge edge)
    {
        const string query = """
                             SELECT COUNT(*) 
                             FROM Edges 
                             WHERE FromNode = @from 
                               AND ToNode = @to 
                               AND EdgeKey = @key 
                               AND json_extract(Metadata, '$') = @metadata
                             """;

        var metadataJson = SerializeMetadata(edge.Metadata);
        var result = _database?.ExecuteScalar(query, new Dictionary<string, object>
        {
            { "@from", edge.From.Name },
            { "@to", edge.To.Name },
            { "@key", edge.EdgeKey },
            { "@metadata", metadataJson }
        });

        return Convert.ToInt32(result) > 0;
    }

    /// <summary>
    ///     Deserializes metadata from JSON format.
    /// </summary>
    /// <typeparam name="T">The type of the metadata to deserialize.</typeparam>
    /// <param name="metadataJson">The JSON string containing the metadata.</param>
    /// <returns>The deserialized metadata object.</returns>
    private static T DeserializeMetadata<T>(string metadataJson) where T : class
    {
        return JsonConvert.DeserializeObject<T>(metadataJson) ??
               throw new InvalidOperationException("Failed to deserialize metadata.");
    }

    /// <summary>
    ///     Serializes metadata to JSON format.
    /// </summary>
    /// <typeparam name="T">The type of the metadata to serialize.</typeparam>
    /// <param name="metadata">The metadata object to serialize.</param>
    /// <returns>The JSON string representation of the metadata.</returns>
    private static string SerializeMetadata<T>(T metadata) where T : class
    {
        return JsonConvert.SerializeObject(metadata);
    }

    /// <summary>
    ///     Initializes the database schema for storing nodes and edges.
    /// </summary>
    private void InitializeSchema()
    {
        if (_database == null) throw new InvalidOperationException("Database instance has not been set.");

        const string createNodesTable = """
                                        CREATE TABLE IF NOT EXISTS Nodes (
                                            Name TEXT PRIMARY KEY,
                                            Metadata TEXT
                                        );
                                        """;

        const string createEdgesTable = """
                                        CREATE TABLE IF NOT EXISTS Edges (
                                            FromNode TEXT NOT NULL,
                                            ToNode TEXT NOT NULL,
                                            EdgeKey TEXT NOT NULL,
                                            Metadata TEXT,
                                            PRIMARY KEY (FromNode, ToNode, EdgeKey),
                                            FOREIGN KEY (FromNode) REFERENCES Nodes (Name) ON DELETE CASCADE,
                                            FOREIGN KEY (ToNode) REFERENCES Nodes (Name) ON DELETE CASCADE
                                        );
                                        """;

        _database.ExecuteNonQuery(createNodesTable);
        _database.ExecuteNonQuery(createEdgesTable);
    }
}