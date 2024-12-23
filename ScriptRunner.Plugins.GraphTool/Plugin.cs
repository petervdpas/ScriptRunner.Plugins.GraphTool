﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Attributes;
using ScriptRunner.Plugins.Extensions;
using ScriptRunner.Plugins.GraphTool.Interfaces;
using ScriptRunner.Plugins.GraphTool.Interfaces.Plugins;
using ScriptRunner.Plugins.GraphTool.Plugins;
using ScriptRunner.Plugins.Logging;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.GraphTool;

/// <summary>
///     A plugin that registers and provides ...
/// </summary>
/// <remarks>
///     This plugin demonstrates how to ...
/// </remarks>
[PluginMetadata(
    name: "Graph Tool",
    description: "A plugin that cam perform graph operations",
    author: "Peter van de Pas",
    version: "1.0.0",
    pluginSystemVersion: PluginSystemConstants.CurrentPluginSystemVersion,
    frameworkVersion: PluginSystemConstants.CurrentFrameworkVersion,
    services: [
    "IClassDiagramPlugin", 
    "IErdPlugin", 
    "ILineagePlugin", 
    "IGraphData", 
    "IGraphPluginManager",
    "IGraphDataPersistence",
    "IGraphVisualization"])]
public class Plugin : BaseAsyncServicePlugin
{
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public override string Name => "GraphTool";

    /// <summary>
    /// Asynchronously registers the plugin's services into the application's dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    public override async Task RegisterServicesAsync(IServiceCollection services)
    {
        // Simulate async service registration (e.g., initializing an external resource)
        await Task.Delay(50);
        
        services.AddSingleton<IClassDiagramPlugin, ClassDiagramPlugin>();
        services.AddSingleton<IErdPlugin, ErdPlugin>();
        services.AddSingleton<ILineagePlugin, LineagePlugin>();
        
        services.AddSingleton<IGraphData, GraphData>();
        services.AddSingleton<IGraphPluginManager>(provider =>
            new GraphPluginManager(
                provider.ResolveRequiredService<IPluginLogger>(),
                provider.ResolveRequiredService<IErdPlugin>(),
                provider.ResolveRequiredService<IClassDiagramPlugin>(),
                provider.ResolveRequiredService<ILineagePlugin>()));
        services.AddSingleton<IGraphDataPersistence<Dictionary<string, object>, Dictionary<string, object>>>
            (_ => new GraphDataPersistence<Dictionary<string, object>, Dictionary<string, object>>());
        services.AddSingleton<IGraphVisualization, GraphVisualization>();
    }
    
    /// <summary>
    /// Initializes the plugin using the provided configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    public override async Task InitializeAsync(IDictionary<string, object> configuration)
    {
        await Task.Delay(100);
        Console.WriteLine(configuration.TryGetValue("SomeKey", out var someValue)
            ? $"SomeKey value: {someValue}"
            : "SomeKey not found in configuration.");
    }

    /// <summary>
    /// Executes the plugin's main functionality.
    /// </summary>
    public override async Task ExecuteAsync()
    {
        await Task.Delay(50);
        Console.WriteLine("GraphTool Plugin executed.");
    }
}