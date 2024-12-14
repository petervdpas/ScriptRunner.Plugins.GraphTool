using ScriptRunner.Plugins.GraphTool.Models;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

public interface IGraphDataPersistence<in TNodeMetadata, in TEdgeMetadata>
    where TNodeMetadata : class
    where TEdgeMetadata : class
{
    void SetDatabase(IDatabase database);
    void SaveGraph(GraphData graphData);
    GraphData LoadGraph(NodeFactory nodeFactory);
    void AddOrUpdateNode(string nodeName, TNodeMetadata metadata);
    void DeleteNode(string nodeName);
    void AddOrUpdateEdge(string fromNode, string toNode, string edgeKey, TEdgeMetadata metadata);
    void DeleteEdge(string fromNode, string toNode, string edgeKey);
}