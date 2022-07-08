using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.General {
  internal static class CollectionExtensions {
    public static T DeepPeek<T>(this Stack<T> stack, int position) {
      return stack.ToArray()[position];
    }

    public static T FirstOfType<T>(this object[] items) {
      return
        (from value in items
         where value is T
         select (T)value).FirstOrDefault();
    }

    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> input) {
      var output = new Dictionary<TKey, TValue>();
      foreach (var pair in input) {
        output.Add(pair.Key, pair.Value);
      }
      return output;
    }

    public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> items) {
      foreach (var item in items) {
        collection.Add(item);
      }
    }

    public static void CopyTo<T>(this Stack<T> from, Stack<T> to) {
      foreach (var value in from.Reverse()) {
        to.Push(value);
      }
    }

    public static IEnumerable<T> Minus<T>(this IEnumerable<T> first, IEnumerable<T> second) {
      var result = new HashSet<T>(first);
      foreach (var item in second) {
        result.Remove(item);
      }
      return result;
    }

    public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second) {
      if (first == null || second == null) {
        return first == second;
      }
      if (first.Count != second.Count) {
        return false;
      }
      foreach (var pair in first) {
        if (!second.ContainsKey(pair.Key)) {
          return false;
        }
        if (!second[pair.Key].Equals(pair.Value)) {
          return false;
        }
      }
      return true;
    }

    public static int DictionaryHashCode<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
      int hashCode = 0;
      foreach (var pair in dictionary) {
        hashCode += pair.Key.GetHashCode() * 31 + pair.Value.GetHashCode();
      }
      return hashCode;
    }

    public static bool ArrayEquals<T>(this T[] first, T[] second) {
      if (first == null || second == null) {
        return first == second;
      }
      if (first.Length != second.Length) {
        return false;
      }
      for (int index = 0; index < first.Length; index++) {
        if (!Equals(first[index], second[index])) {
          return false;
        }
      }
      return true;
    }

    public static int ArrayHashCode<T>(this T[] array) {
      var hash = array.Length;
      if (array.Length > 0) {
        hash = hash * 31 + array[0].GetHashCode();
      }
      if (array.Length > 1) {
        hash = hash * 31 + array[1].GetHashCode();
      }
      return hash;
    }

    public static T[] Append<T>(this T[] array, T value) {
      var result = new T[array.Length + 1];
      for (int index = 0; index < array.Length; index++) {
        result[index] = array[index];
      }
      result[array.Length] = value;
      return result;
    }

    public static T[] AppendArray<T>(this T[] first, T[] second) {
      var result = new T[first.Length + second.Length];
      for (int index = 0; index < first.Length; index++) {
        result[index] = first[index];
      }
      for (int index = 0; index < second.Length; index++) {
        result[index + first.Length] = second[index];
      }
      return result;
    }

    public static long Product(this int[] array) {
      long product = 1L;
      foreach (var value in array) {
        product *= value;
      }
      return product;
    }
  }
}
