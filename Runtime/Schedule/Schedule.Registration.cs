namespace Engine;

public sealed partial class Schedule
{
    /// <summary>Adds a system delegate to the specified execution stage.</summary>
    /// <param name="stage">The <see cref="Stage"/> to register the system in.</param>
    /// <param name="system">The system delegate to execute when the stage runs.</param>
    /// <returns>This <see cref="Schedule"/> instance for fluent chaining.</returns>
    public Schedule AddSystem(Stage stage, SystemFn system)
    {
        lock (_lock)
            _systemsByStage[stage].Add(new SystemDescriptor(system));
        return this;
    }

    /// <summary>Adds a system with a <c>run_if</c> condition to the specified stage.</summary>
    /// <param name="stage">The <see cref="Stage"/> to register the system in.</param>
    /// <param name="system">The system delegate to execute when the stage runs.</param>
    /// <param name="runCondition">A predicate evaluated each frame; the system is skipped when <c>false</c>.</param>
    /// <returns>This <see cref="Schedule"/> instance for fluent chaining.</returns>
    public Schedule AddSystem(Stage stage, SystemFn system, Func<World, bool> runCondition)
    {
        lock (_lock)
            _systemsByStage[stage].Add(new SystemDescriptor(system) { RunCondition = runCondition });
        return this;
    }

    /// <summary>Adds a fully configured <see cref="SystemDescriptor"/> to the specified stage.</summary>
    /// <param name="stage">The <see cref="Stage"/> to register the descriptor in.</param>
    /// <param name="descriptor">A pre-configured system descriptor with name, conditions, and access metadata.</param>
    /// <returns>This <see cref="Schedule"/> instance for fluent chaining.</returns>
    public Schedule AddSystem(Stage stage, SystemDescriptor descriptor)
    {
        lock (_lock)
            _systemsByStage[stage].Add(descriptor);
        return this;
    }

    /// <summary>Removes all systems matching <paramref name="predicate"/> from the specified stage.</summary>
    /// <param name="stage">The <see cref="Stage"/> to remove systems from.</param>
    /// <param name="predicate">A predicate selecting which systems to remove.</param>
    /// <returns>The number of systems removed.</returns>
    public int RemoveSystems(Stage stage, Predicate<SystemDescriptor> predicate)
    {
        lock (_lock)
            return _systemsByStage[stage].RemoveAll(predicate);
    }
}
