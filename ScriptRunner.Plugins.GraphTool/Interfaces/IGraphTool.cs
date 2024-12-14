using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Enums;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

public interface IGraphTool
{
    GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        string[]? entityList = null,
        bool bubbleUp = true,
        bool includeUnfocusedNeighbors = false);

    GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        Dictionary<string, Dictionary<string, object>>? nodeMetadata,
        Dictionary<string, Dictionary<string, object>>? edgeMetadata,
        string[]? entityList = null,
        bool bubbleUp = true,
        bool includeUnfocusedNeighbors = false);

    string GenerateJsonFromGraph(GraphData graphData);
}