using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Collections.Concurrent", "BlockingCollection`1")]
  internal sealed class BlockingCollection : Model.Object {
    private readonly int _capacity;
    private readonly Queue<object> _elements = new();
    private readonly Queue<Thread> _nonEmptyWaiters = new();
    private readonly Queue<Thread> _nonFullWaiters = new();
    private readonly VectorTime _accessTime = new();

    [Member]
    public BlockingCollection() :
      this(int.MaxValue) {
    }

    [Member]
    public BlockingCollection(int capacity) :
      base(typeof(System.Collections.Concurrent.BlockingCollection<>)) {
      _capacity = capacity;
    }

    [Member]
    public int GetCount() {
      return _elements.Count;
    }

    [Member]
    public int GetBoundedCapacity() {
      return _capacity;
    }

    [Member]
    public void Add(Program program, object item) {
      if (_elements.Count >= _capacity) {
        program.Wait(this, _nonFullWaiters);
      } else {
        _elements.Enqueue(item);
        program.Pass(_accessTime);
        program.SignalSingle(_nonEmptyWaiters, _accessTime);
      }
    }

    [Member]
    public void Add(Program program, object item, object _) {
      Add(program, item);
    }

    [Member]
    public object Take(Program program) {
      object item = null;
      if (_elements.Count == 0) {
        program.Wait(this, _nonEmptyWaiters);
      } else {
        item = _elements.Dequeue();
        program.Pass(_accessTime);
        program.SignalSingle(_nonFullWaiters, _accessTime);
      }
      return item;
    }

    [Member]
    public object Take(Program program, object _) {
      return Take(program);
    }
  }
}
