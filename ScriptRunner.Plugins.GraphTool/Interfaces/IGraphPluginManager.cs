using System.Collections.Generic;
using ScriptRunner.Plugins.GraphTool.Enums;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

public interface IGraphPluginManager
{
    GraphData CreateGraph(
        PluginType pluginType,
        IEnumerable<Entity> entities,
        IEnumerable<Relationship> relationships,
        Dictionary<string, Dictionary<string, object>>? nodeMetadata = null,
        Dictionary<string, Dictionary<string, object>>? edgeMetadata = null);
}