using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class LocalFunctionTask {
    static void Main(string[] args) {
      System.Threading.Tasks.Task.Run(() => { });

    }

    Dictionary<int, Action> dictionary = new Dictionary<int, Action>();

    void Test() {
      var b = new B();
      dictionary.Add(1, Handle<int>);
      dictionary.Add(2, b.Handle2<bool>);
      dictionary.Add(3, this.Handle<int>);
    }

    void Handle<T>() {

    }
  }

  class B {
    public void Handle2<T>() {

    }
  }
}
