namespace Engine;

public sealed partial class App
{
    /// <summary>
    /// Adds and builds a plugin. Each concrete plugin type can only be added once;
    /// duplicate registrations are silently skipped.
    /// </summary>
    /// <param name="plugin">The plugin instance to register and build.</param>
    /// <returns>This <see cref="App"/> instance for fluent chaining.</returns>
    /// <exception cref="Exception">Re-throws any exception thrown by the plugin's <see cref="IPlugin.Build"/> method.</exception>
    /// <remarks>
    /// Plugins are identified by their concrete <see cref="Type"/>. Calling <c>AddPlugin</c> twice
    /// with the same plugin type is a no-op. The plugin's <see cref="IPlugin.Build"/> method is
    /// invoked immediately during this call - not deferred.
    /// </remarks>
    /// <example>
    /// <code>
    /// app.AddPlugin(new TimePlugin())
    ///    .AddPlugin(new InputPlugin())
    ///    .AddPlugin(new EcsPlugin());
    /// </code>
    /// </example>
    /// <seealso cref="IPlugin"/>
    /// <seealso cref="HasPlugin{T}"/>
    public App AddPlugin(IPlugin plugin)
    {
        var pluginType = plugin.GetType();
        var pluginName = pluginType.Name;

        if (_plugins.ContainsKey(pluginType))
        {
            Logger.Trace($"Plugin already registered, skipping: {pluginName}");
            return this;
        }

        Logger.Info($"Adding plugin: {pluginName}...");
        _plugins[pluginType] = plugin;

        try
        {
            plugin.Build(this);
            Logger.Info($"Plugin built: {pluginName} (total plugins: {_plugins.Count})");
        }
        catch (Exception ex)
        {
            Logger.Error($"Plugin '{pluginName}' failed during Build()", ex);
            throw;
        }

        return this;
    }

    /// <summary>Checks whether a plugin of type <typeparamref name="T"/> has been registered.</summary>
    /// <typeparam name="T">The concrete plugin type to look up.</typeparam>
    /// <returns><c>true</c> if a plugin of type <typeparamref name="T"/> has been added; otherwise <c>false</c>.</returns>
    public bool HasPlugin<T>() where T : IPlugin
        => _plugins.ContainsKey(typeof(T));

    /// <summary>Number of plugins currently registered.</summary>
    /// <returns>The count of registered plugins.</returns>
    public int PluginCount => _plugins.Count;

    /// <summary>Snapshot of all registered plugin types at the time of the call.</summary>
    /// <returns>A read-only collection of <see cref="Type"/> objects representing each registered plugin.</returns>
    public IReadOnlyCollection<Type> Plugins => _plugins.Keys.ToArray();

}
