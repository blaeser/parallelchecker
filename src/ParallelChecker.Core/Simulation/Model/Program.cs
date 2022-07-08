using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Program {
    public ControlFlowModel ControlFlowModel { get; }
    public AnalysisOptions Options { get; }
    public SimulationBounds Bounds { get; }
    public long Round { get; }
    public Random Random { get; }
    public HashSet<Issue> Issues { get; } = new();
    public VariableSet StaticFields { get; } = new();
    public HashSet<Thread> AllThreads { get; } = new();
    public OrderedSet<Thread> RunnableThreads { get; } = new();
    public Thread ActiveThread { get; set; }
    public HashSet<ITypeSymbol> LoadedTypes { get; } = new(SymbolEqualityComparer.Default);
    public Thread StaticLoader { get; }
    public bool ModuleInitialized { get; set; }
    public HashSet<Object> ExtraRootSet { get; } = new();
    public HashSet<Object> Finalizeables { get; } = new();
    public Dispatcher Finalizer { get; }
    public long HeapSize { get; private set; }
    public long SimulationTime { get; private set; }
    public long Variations { get; set; }
    public MultiDictionary<ITypeSymbol, Epoch> UnsafeCalls { get; } = new();

    public Program(ControlFlowModel controlFlowModel, AnalysisOptions options, SimulationBounds bounds, long round, Random random) {
      ControlFlowModel = controlFlowModel ?? throw new ArgumentNullException(nameof(controlFlowModel));
      Options = options ?? throw new ArgumentNullException(nameof(options));
      Bounds = bounds ?? throw new ArgumentNullException(nameof(bounds));
      Round = round;
      Random = random ?? throw new ArgumentNullException(nameof(random));
      StaticLoader = new Thread(this, new Cause("static loader", Location.None)) {
        SyncWithStatic = false
      };
      Finalizer = new Dispatcher(random);
      PreviousLocation = Location.None;
    }

    public CompilationModel CompilationModel {
      get { return ControlFlowModel.CompilationModel; }
    }

    public Method ActiveMethod {
      get { return ActiveThread.ActiveMethod; }
    }

    public BasicBlock ActiveBlock {
      get { return ActiveMethod.ActiveBlock; }
    }

    public Location ActiveLocation {
      get { return ActiveBlock.Location; }
    }

    public Cause ActiveCause {
      get {
        if (ActiveThread.CallStack.Count == 0) {
          return ActiveThread.Cause;
        }
        return ActiveMethod.Cause;
      }
    }

    public Location PreviousLocation { get; set; }

    public void IncreaseHeapSize(long amount) {
      if (HeapSize > long.MaxValue - amount || HeapSize + amount > SimulationBounds.HeapSizeBound) {
        throw new BoundException("Heap size bound exceeded");
      }
      HeapSize += amount;
    }

    public void IncreaseSimulationTime(long amount) {
      if (amount < 0) {
        throw new ArgumentException(nameof(amount));
      }
      SimulationTime += amount;
      if (ActiveThread == StaticLoader && SimulationTime > Bounds.TimeBound * SimulationBounds.InitializationTimePercent / 100) {
        StaticLoader.State = ThreadState.Terminated;
        RunnableThreads.Remove(StaticLoader);
      }
      if (SimulationTime > Bounds.TimeBound * SimulationBounds.PerRunTimePercent / 100) {
        throw new BoundException("Simulation time bound exceeded");
      }
    }

    public void ResetThreads() {
      AllThreads.Clear();
      RunnableThreads.Clear();
    }
  }
}
