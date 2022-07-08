using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Base {
  // TODO: Proper design with sub-classes etc. depending on kind of entry
  internal class ProgramEntry {
    public Dispatcher Dispatcher { get; set; }
    public bool Concurrent { get; set; }
    public List<Routine> Routines { get; } = new();
    public Dictionary<TypeDeclarationSyntax, Object> Instances { get; } = new();
    public MultiDictionary<IEventSymbol, MethodDeclarationSyntax> UIEvents { get; set; }
  }
}
