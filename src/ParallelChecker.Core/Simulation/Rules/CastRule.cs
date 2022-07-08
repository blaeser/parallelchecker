using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class CastRule : Rule<CastBlock> {
    public override int TimeCost => 5; 

    public override void Apply(Program program, CastBlock block) {
      if (program.InitializeStatics(block.Type)) {
        return;
      }    
      var method = program.ActiveMethod;
      var type = block.Type;
      var value = method.EvaluationStack.Pop();
      value = Cast(value, type, program.ActiveLocation);
      method.EvaluationStack.Push(value);
      program.GoToNextBlock();
    }

    private object Cast(object value, ITypeSymbol type, Location location) {
      if (type == null || type.Is(Symbols.IntPtrType)) { // TODO: Support IntPtr casts
        return Unknown.Value;
      } else if (type.IsReferenceType || type.IsStruct()) {
        return ReferenceTypeCast(value, type, location);
      } else if (type.Is(Symbols.Nullable)) {
        return NullableTypeCast(value, type, location);
      } else {
        return PrimitiveTypeCast(value, type, location);
      }
    }

    private object NullableTypeCast(object value, ITypeSymbol type, Location location) {
      if (value == null) {
        return value;
      } else {
        return Cast(value, type.UnwrapNullable(), location);
      }
    }

    private static object ReferenceTypeCast(object value, ITypeSymbol targetType, Location location) {
      if (value == null || targetType.Is(Symbols.RootClass) ||
          value is Lambda || value is Model.Delegate ||
          value is Model.Array) {
      } else if (value is Model.Object currentObject) {
        if (currentObject != Unknown.Value && currentObject.Type != null &&
            !currentObject.Type.IsCompatibleTo(targetType)) {
          throw new Model.Exception(location, new InvalidCastException());
        }
      } else {
        if (!value.GetType().IsCompatibleTo(targetType)) {
          throw new Model.Exception(location, new InvalidCastException());
        }
      }
      return value;
    }

    private static object PrimitiveTypeCast(object value, ITypeSymbol targetType, Location location) {
      var nativeType = targetType.GetNativeType();
      if (nativeType != null && value != Unknown.Value) {
        if (value is Model.Object || value is string || value is Lambda) {
          // TODO: Support overloaded conversions
          throw new Model.Exception(location, new InvalidCastException());
        }
        return value.Convert(nativeType);
      } else {
        return value;
      }
    }
  }
}
