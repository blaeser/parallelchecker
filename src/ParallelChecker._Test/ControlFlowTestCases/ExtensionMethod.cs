using System;

namespace ParallelChecker._Test {
  static class Library {
    public static void ExtensionMethod(this string text, int count) {
      for (int i = 0; i < count; i++) {
        Console.WriteLine(text);
      }
    }
  }

  class Test {
    public static void Main() {
      var text = "Hello";
      text.ExtensionMethod(3);
      Library.ExtensionMethod(text, 2);
    }
  }
}
