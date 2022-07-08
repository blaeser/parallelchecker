using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Array : Object {
    private readonly Variable[] _elements;
    public int[] Lengths { get; }
    public ITypeSymbol ElementType { get; }

    public Array(int[] lengths, int ranks, ITypeSymbol elementType, Program program) 
      : base() {
      Lengths = lengths ?? throw new ArgumentNullException(nameof(lengths));
      ElementType = elementType;
      var totalLength = lengths.Product();
      if (totalLength < 0) {
        throw new OverflowException("Negative array length");
      }
      program.IncreaseHeapSize(totalLength / 10);
      _elements = new Variable[totalLength];
      var arrayType = ranks > 1 ? null : elementType;
      var initial = InitialValue(ranks, elementType);
      for (int index = 0; index < _elements.Length; index++) {
        _elements[index] = new ImplicitVariable("array", arrayType, initial);
      }
    }
    
    public Array(List<object> values, Program program) :
      this(new int[] { values.Count }, 1, null, program) {
      for (int index = 0; index < values.Count; index++) {
        _elements[index].Value = values[index];
      }
    }

    private static object InitialValue(int ranks, ITypeSymbol elementType) {
      if (ranks > 1) {
        return null;
      }
      if (elementType == null || elementType.IsTypeParameter()) {
        return Unknown.Value;
      }
      if (elementType.IsStruct()) {
        return new Object(elementType);
      }
      return elementType.GetDefaultValue();
    }

    public Variable GetElement(int[] indices) {
      var total = 0;
      for (int rank = 0; rank < Dimensions; rank++) {
        if (rank > 0) {
          total *= Lengths[rank];
        }
        var index = indices[rank];
        if (index < 0 || index >= Lengths[rank]) {
          throw new IndexOutOfRangeException("Array index out of range");
        }
        total += index;
      }
      return _elements[total];
    }

    public Array GetSubRange(Program program, int start, int end) {
      if (Lengths.Length > 1) {
        throw new System.Exception("Unsupported multi-dimensional range access");
      }
      int length = Lengths[0];
      if (start < 0 || start >= end || end > length) {
        throw new IndexOutOfRangeException("Array index out of range");
      }
      var subArray = new Array(new int[] { end - start }, 1, ElementType, program);
      for (int index = start; index < end; index++) {
        subArray.GetElement(new int[] { index - start }).Value = GetElement(new int[] { index }).Value;
      }
      return subArray;
    }

    public IEnumerable<object> AllValues() {
      return
        from variable in _elements
        select variable.Value;
    }

    public IEnumerable<Variable> AllVariables() {
      return _elements;
    }
    
    public int Dimensions {
      get { return Lengths.Length; }
    }
  }
}
