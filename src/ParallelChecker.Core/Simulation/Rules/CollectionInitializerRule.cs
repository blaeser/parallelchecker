using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class CollectionInitializerRule : Rule<CollectionInitializerBlock> {
    public override int TimeCost => 10;

    public override void Apply(Program program, CollectionInitializerBlock block) {
      if (program.InitializeStatics(block.Type)) {
        return;
      }
      var stack = program.ActiveMethod.EvaluationStack;
      if (block.Sizes.All(size => size == 1)) {
        InitializeCollection(block, stack);
      } else if (block.Sizes.All(size => size == 2)) {
        InitializeDictionary(block, stack);
      } else {
        IgnoreTupleList(block, stack);
      }
      program.GoToNextBlock();
    }

    private void IgnoreTupleList(CollectionInitializerBlock block, Stack<object> stack) {
      // TODO: Support general Add() methods on collection instances
      var sum = block.Sizes.Sum();
      for (int index = 0; index < sum; index++) {
        stack.Pop();
      }
      if (!block.AddAll) {
        stack.Push(Unknown.Value);
      }
    }

    private void InitializeCollection(CollectionInitializerBlock block, Stack<object> stack) {
      var temporary = new List<object>();
      for (int index = 0; index < block.Sizes.Length; index++) {
        temporary.Add(stack.Pop());
      }
      temporary.Reverse();
      FillItems(block, stack, temporary);
    }

    private void InitializeDictionary(CollectionInitializerBlock block, Stack<object> stack) {
      var temporary = new Dictionary<object, object>();
      for (int index = 0; index < block.Sizes.Length; index++) {
        var value = stack.Pop();
        var key = stack.Pop();
        if (key == null) {
          throw new Model.Exception(block.Location, new ArgumentException("Dictionary key cannot be null"));
        }
        if (temporary.ContainsKey(key)) {
          if (key != Unknown.Value) {
            throw new Model.Exception(block.Location, new ArgumentException("An element with the same key already exists in the dictionary"));
          }
        } else {
          temporary.Add(key, value);
        }
      }
      FillItems(block, stack, temporary);
    }

    private static void FillItems(CollectionInitializerBlock block, Stack<object> stack, IEnumerable temporary) {
      if (block.AddAll) {
        var value = stack.Peek();
        if (value is SystemObject instance) {
          var target = (IEnumerable)instance.NativeInstance;
          AddAll(target, temporary);
        }
      } else if (SystemObject.IsSystemDefined(block.Type)) {
        var instance = SystemObject.Create(block.Type);
        var target = (IEnumerable)instance.NativeInstance;
        AddAll(target, temporary);
        stack.Push(instance);
      } else {
        stack.Push(Unknown.Value);
      }
    }

    private static void AddAll(IEnumerable target, IEnumerable source) {
      if (target is IDictionary dictionaryOutput) {
        var input = (IDictionary)source;
        var output = dictionaryOutput;
        foreach (var key in input.Keys) {
          output.Add(key, input[key]);
        }
      } else if (target is ICollection<object> collectionOutput) {
        foreach (var item in source) {
          collectionOutput.Add(item);
        }
      } else if (target is IList listOutput) {
        foreach (var item in source) {
          listOutput.Add(item);
        }
      } else {
        throw new NotImplementedException();
      }
    }
  }
}
