using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Blocks;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Scopes {
  internal sealed class BlockScope : Scope {
    public EnterTryBlock EnterTry { get; set; }
    public IList<VariableDeclarationSyntax> Disposables = new List<VariableDeclarationSyntax>();
  }
}
