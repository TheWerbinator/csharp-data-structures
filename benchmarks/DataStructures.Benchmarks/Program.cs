using System.Reflection;
using BenchmarkDotNet.Running;

// Runs every benchmark class in the assembly.
// Usage: dotnet run -c Release --project benchmarks/DataStructures.Benchmarks
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
