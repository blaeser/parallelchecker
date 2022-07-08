// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0038:Use pattern matching", Justification = "not applicable in LINQ clauses", Scope = "namespaceanddescendants", Target = "~N:ParallelChecker.Core")]
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "not supported in .NET Standard 2.0", Scope = "namespaceanddescendants", Target = "~N:ParallelChecker.Core")]
[assembly: SuppressMessage("Style", "IDE0056:Use index operator", Justification = "not supported in .NET Standard 2.0", Scope = "namespaceanddescendants", Target = "~N:ParallelChecker.Core")]
[assembly: SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "analysis not working for constructor overloads", Scope = "namespaceanddescendants", Target = "~N:ParallelChecker.Core")]
