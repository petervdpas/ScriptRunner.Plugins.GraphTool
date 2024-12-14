using System;
using ScriptRunner.Plugins.GraphTool.Enums;

namespace ScriptRunner.Plugins.GraphTool.Interfaces;

/// <summary>
/// Defines the interface for rendering visual representations of graph data structures using different templates.
/// </summary>
public interface IGraphVisualization
{
    /// <summary>
    /// Renders a visual representation of the provided graph data using the specified template type.
    /// </summary>
    /// <param name="graphData">
    /// The graph data to be rendered, including nodes and edges that define the graph structure.
    /// </param>
    /// <param name="templateType">
    /// The type of template to use for rendering the visualization. Supported templates include:
    /// <see cref="TemplateType.MermaidErd" />, <see cref="TemplateType.MermaidClassDiagram" />,
    /// <see cref="TemplateType.PlantUmlClass" />, and <see cref="TemplateType.GraphvizDiGraph" />.
    /// </param>
    /// <returns>
    /// A string containing the rendered visualization in the format corresponding to the selected template type.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the specified <paramref name="templateType" /> is not supported.
    /// </exception>
    string Render(IGraphData graphData, TemplateType templateType);
}