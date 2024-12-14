using ScriptRunner.Plugins.GraphTool.Enums;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

public interface IGraphVisualization
{
    string Render(IGraphData graphData, TemplateType templateType);
}