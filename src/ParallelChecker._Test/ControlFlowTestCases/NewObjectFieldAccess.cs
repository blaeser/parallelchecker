using System;

namespace ParallelChecker._Test {
  class Base {
    public int x = 123;
  }

  class Sub : Base {
    public int y = 321;
  }

  class SubSub : Sub {
    public SubSub() {
      Console.WriteLine("Test");
    }
  }

  class NewObjectFieldAccess {
    public static void Main() {
      Console.WriteLine(new SubSub().x == 0);
      new SubSub().x = 1;
    }
  }
}
