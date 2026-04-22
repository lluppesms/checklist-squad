// Disable parallel execution — smoke tests hit a live server sequentially
// to avoid overwhelming it with concurrent SignalR connections.
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 1)]
