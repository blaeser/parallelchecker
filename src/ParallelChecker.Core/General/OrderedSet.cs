using System;
using System.Collections;
using System.Collections.Generic;

namespace ParallelChecker.Core.General {
  internal class OrderedSet<T> : ICollection<T> {
    private readonly HashSet<T> _set = new();
    private readonly List<T> _list = new();

    public int Count => _list.Count;

    public bool IsReadOnly => false;

    public void Add(T item) {
      if (_set.Add(item)) {
        _list.Add(item);
      }
    }

    public void Clear() {
      _set.Clear();
      _list.Clear();
    }

    public bool Contains(T item) {
      return _set.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
      _list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator() {
      return _list.GetEnumerator();
    }

    public bool Remove(T item) {
      if (_set.Remove(item)) {
        _list.Remove(item);
        return true;
      }
      return false;
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return _list.GetEnumerator();
    }

    public T this[int index] {
      get => _list[index];
    }

    public T PickRandom(Random random) => _list[random.Next(Count)];
  }
}
