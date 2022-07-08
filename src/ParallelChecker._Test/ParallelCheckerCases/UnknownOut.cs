using System;
using System.Collections.Generic;

namespace CheckerDevTest {
  internal class PropertyDictionary : Dictionary<string, object> {
    public T GetOrDefault<T>(string name, T @default) {
      object value;
      if (this.TryGetValue(name, out value)) {
        return (T)value;
      }
      return @default;
    }

    public new object this[string name] {
      get {
        object value;
        return this.TryGetValue(name, out value)
            ? value : null;
      }
      set { base[name] = value; }
    }
  }

  class Program {
    internal readonly PropertyDictionary _store = new PropertyDictionary();

    public bool ProvideCommandLineArgs {
      set {
        _store[nameof(ProvideCommandLineArgs)] = value;
      }

      get {
        return _store.GetOrDefault(nameof(ProvideCommandLineArgs), false);
      }
    }

    void Test() {
      if (ProvideCommandLineArgs) {
        Console.WriteLine("DONE");
      }
    }

    static void Main() {
      new Program().Test();
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}
