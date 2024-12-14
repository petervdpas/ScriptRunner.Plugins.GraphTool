using System;
using System.Collections.Generic;
using System.Text;
using ScriptRunner.Plugins.GraphTool.Enums;
using ScriptRunner.Plugins.GraphTool.Interfaces;

namespace ScriptRunner.Plugins.GraphTool;

/// <summary>
///     Service to generate visual representations of graph structures based on given templates.
///     Implements <see cref="IGraphVisualization" />.
/// </summary>
public class GraphVisualization : IGraphVisualization
{
    private const string Tab = "  ";

    /// <summary>
    ///     Renders a visual representation of the provided graph structure using the specified template.
    /// </summary>
    /// <param name="graphData">The graph data to be rendered.</param>
    /// <param name="templateType">The type of template to use for rendering.</param>
    /// <returns>A string representation of the generated visualization.</returns>
    public string Render(IGraphData graphData, TemplateType templateType)
    {
        return templateType switch
        {
            TemplateType.MermaidErd => RenderMermaidErdDiagram(graphData),
            TemplateType.MermaidClassDiagram => RenderMermaidClassDiagram(graphData),
            TemplateType.PlantUmlClass => RenderPlantUmlClassDiagram(graphData),
            TemplateType.GraphvizDiGraph => RenderGraphvizDiGraphDiagram(graphData),
            _ => throw new ArgumentException($"Unsupported template type: {templateType}")
        };
    }

    /// <summary>
    ///     Generates a Mermaid ERD diagram representation of the provided graph data.
    /// </summary>
    /// <param name="graphData">The graph data to be rendered, such as entity-relationship data or a class hierarchy.</param>
    /// <returns>A string representation of the Mermaid diagram.</returns>
    private static string RenderMermaidErdDiagram(IGraphData graphData)
    {
        var sb = new StringBuilder();
        sb.AppendLine("erDiagram");

        // Render nodes as entities
        foreach (var node in graphData.Nodes)
        {
            sb.AppendLine($"{Tab}{node.Name} {{");

            // Render metadata as attributes
            foreach (var (attributeName, attributeDetails) in node.Metadata)
            {
                if (attributeDetails is not Dictionary<string, object> details) continue;

                var type = details.GetValueOrDefault("Type", "unknown");
                var isKey = details.TryGetValue("IsKey", out var value) && (bool)value ? "PK" : string.Empty;
                sb.AppendLine($"{Tab}{Tab}{type} {attributeName} {isKey}");
            }

            sb.AppendLine($"{Tab}}}");
        }

        // Render edges as relationships
        foreach (var edge in graphData.Edges)
            sb.AppendLine($"{Tab}{edge.From.Name} ||--o| {edge.To.Name} : {edge.EdgeKey}");

        return sb.ToString();
    }

    /// <summary>
    ///     Generates a Mermaid class diagram representation of the provided graph data.
    /// </summary>
    /// <param name="graphData">The graph data to be rendered, such as class diagrams or other object relationships.</param>
    /// <returns>A string representation of the Mermaid class diagram.</returns>
    private static string RenderMermaidClassDiagram(IGraphData graphData)
    {
        var sb = new StringBuilder();
        sb.AppendLine("classDiagram");

        // Render nodes as classes
        foreach (var node in graphData.Nodes)
        {
            sb.AppendLine($"{Tab}class {node.Name} {{");

            if (node.Metadata.TryGetValue("Values", out var metaValue))
            {
                // Render enum
                sb.AppendLine($"{Tab}{Tab}<<enumeration>>");

                var values = (string[])metaValue;
                foreach (var value in values) sb.AppendLine($"{Tab}{Tab}{value}");
            }
            else
            {
                // Render class
                foreach (var (attributeName, value) in node.Metadata)
                {
                    if (value is not Dictionary<string, object> attributeDetails) continue;
                    var type = attributeDetails.GetValueOrDefault("Type", "unknown").ToString();
                    sb.AppendLine($"{Tab}{Tab}+{type} {attributeName}");
                }
            }

            sb.AppendLine($"{Tab}}}");
        }

        // Render edges as inheritance or associations
        foreach (var edge in graphData.Edges)
        {
            var relationshipArrow = edge.EdgeKey switch
            {
                "inherits" => "<|--",
                "enum" => "-->",
                _ => "-->"
            };
            sb.AppendLine($"{Tab}{edge.From.Name} {relationshipArrow} {edge.To.Name} : {edge.EdgeKey}");
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Generates a PlantUML diagram representation of the provided graph data.
    /// </summary>
    /// <param name="graphData">The graph data to be rendered, such as class diagrams or other object relationships.</param>
    /// <returns>A string representation of the PlantUML diagram.</returns>
    private static string RenderPlantUmlClassDiagram(IGraphData graphData)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@startuml");

        // Render nodes as classes
        foreach (var node in graphData.Nodes)
        {
            sb.AppendLine($"{Tab}class {node.Name} {{");

            // Render metadata as attributes
            foreach (var (attributeName, value) in node.Metadata)
            {
                if (value is not Dictionary<string, object> attributeDetails) continue;

                var type = attributeDetails.GetValueOrDefault("Type", "unknown");
                sb.AppendLine($"{Tab}{Tab}{attributeName} : {type}");
            }

            sb.AppendLine($"{Tab}}}");
        }

        // Render edges as associations
        foreach (var edge in graphData.Edges)
            sb.AppendLine($"{Tab}{edge.From.Name} --> {edge.To.Name} : {edge.EdgeKey}");

        sb.AppendLine("@enduml");
        return sb.ToString();
    }

    /// <summary>
    ///     Generates a Graphviz (DOT) diagram representation of the provided graph data.
    /// </summary>
    /// <param name="graphData">The graph data to be rendered, such as interconnected entities or network diagrams.</param>
    /// <returns>A string representation of the Graphviz (DOT) diagram.</returns>
    private static string RenderGraphvizDiGraphDiagram(IGraphData graphData)
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph G {");

        // Render nodes as graph vertices
        foreach (var node in graphData.Nodes)
            sb.AppendLine($"{Tab}{node.Name} [label=\"{node.Name}\"];");

        // Render edges as graph edges
        foreach (var edge in graphData.Edges)
            sb.AppendLine($"{Tab}{edge.From.Name} -> {edge.To.Name} [label=\"{edge.EdgeKey}\"];");

        sb.AppendLine("}");
        return sb.ToString();
    } 
}