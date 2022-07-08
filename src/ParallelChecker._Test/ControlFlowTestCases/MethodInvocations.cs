namespace ParallelChecker._Test {
  class MethodInvocations {
    public static void Main() {
      Test();
    }

    static void Test() {
      Foo();
    }

    static void Foo() {
      Test();
    }
  }
}
