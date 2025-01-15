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
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.GraphTool;

/// <summary>
///     A plugin that registers and provides ...
/// </summary>
/// <remarks>
///     This plugin demonstrates how to ...
/// </remarks>
[PluginMetadata(
    "GraphTool",
    "A plugin that cam perform graph operations",
    "Peter van de Pas",
    "1.0.0",
    PluginSystemConstants.CurrentPluginSystemVersion,
    PluginSystemConstants.CurrentFrameworkVersion,
    [
        "IClassDiagramPlugin",
        "IErdPlugin",
        "ILineagePlugin",
        "IGraphData",
        "IGraphPluginManager",
        "IGraphDataPersistence",
        "IGraphVisualization"
    ])]
public class Plugin : BaseAsyncServicePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    public override string Name => "GraphTool";

    /// <summary>
    ///     Asynchronously registers the plugin's services into the application's dependency injection container.
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
    ///     Initializes the plugin using the provided configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    public override async Task InitializeAsync(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Store settings into LocalStorage
        PluginSettingsHelper.StoreSettings(configuration);

        // Optionally display the settings
        PluginSettingsHelper.DisplayStoredSettings();

        await Task.CompletedTask;
    }

    /// <summary>
    ///     Executes the plugin's main functionality.
    /// </summary>
    public override async Task ExecuteAsync()
    {
        // Example execution logic
        await Task.Delay(50);

        var storedSetting = PluginSettingsHelper.RetrieveSetting<string>("PluginName", true);
        Console.WriteLine($"Retrieved PluginName: {storedSetting}");
    }
}