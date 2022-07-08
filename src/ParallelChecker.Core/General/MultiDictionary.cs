using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.General {
  internal class MultiDictionary<TKey, TValue> : IEquatable<MultiDictionary<TKey, TValue>> {
    private readonly Dictionary<TKey, HashSet<TValue>> _dictionary = new();

    public MultiDictionary() {
    }

    public MultiDictionary(MultiDictionary<TKey, TValue> other) {
      Add(other);
    }

    public int Count {
      get {
        return _dictionary.Count;
      }
    }

    public void Add(MultiDictionary<TKey, TValue> other) {
      foreach (var key in other.Keys) {
        Add(key, other[key]);
      }
    }

    public void Add(TKey key, IEnumerable<TValue> values) {
      foreach (var value in values) {
        Add(key, value);
      }
    }

    public void Add(TKey key, TValue value) {
      if (!_dictionary.ContainsKey(key)) {
        _dictionary.Add(key, new HashSet<TValue>());
      }
      _dictionary[key].Add(value);
    }

    public void Remove(TKey key) {
      _dictionary.Remove(key);
    }

    public void Remove(IEnumerable<TKey> keys) {
      foreach (var key in keys) {
        Remove(key);
      }
    }

    public void Remove(TKey key, TValue value) {
      if (_dictionary.ContainsKey(key)) {
        _dictionary[key].Remove(value);
      }
    }

    public void Replace(TKey key, IEnumerable<TValue> values) {
      Remove(key);
      Add(key, values);
    }

    public HashSet<TValue> GetValueSet(TKey key) {
      if (!ContainsKey(key)) {
        _dictionary[key] = new HashSet<TValue>();
      }
      return _dictionary[key];
    }

    public bool ContainsKey(TKey key) {
      return _dictionary.ContainsKey(key);
    }

    public IEnumerable<TValue> this[TKey key] {
      get { return _dictionary[key]; }
    }

    public IEnumerable<TKey> Keys {
      get { return _dictionary.Keys; }
    }

    public IEnumerable<TValue> Values {
      get {
        return
          from key in _dictionary.Keys
          from value in _dictionary[key]
          select value;
      }
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> Entries {
      get {
        return
          from key in _dictionary.Keys
          from value in _dictionary[key]
          select new KeyValuePair<TKey, TValue>(key, value);
      }
    }

    public bool Equals(MultiDictionary<TKey, TValue> other) {
      if (other == null) {
        return false;
      }
      if (_dictionary.Count != other._dictionary.Count) {
        return false;
      }
      foreach (var pair in _dictionary) {
        if (!other._dictionary.ContainsKey(pair.Key)) {
          return false;
        }
        if (!other._dictionary[pair.Key].SetEquals(pair.Value)) {
          return false;
        }
      }
      return true;
    }

    public MultiDictionary<TKey, TValue> Union(MultiDictionary<TKey, TValue> other) {
      var result = new MultiDictionary<TKey, TValue>();
      foreach (var key in Keys) {
        foreach (var value in this[key]) {
          result.Add(key, value);
        }
      }
      foreach (var key in other.Keys) {
        foreach (var value in other[key]) {
          result.Add(key, value);
        }
      }
      return result;
    }

    public IEnumerable<TValue> Select(Func<TKey, bool> predicate) {
      return
        from entry in Entries
        where predicate(entry.Key)
        select entry.Value;
    }
  }
}
