namespace Large {
  internal class ClassA6 {
    private readonly ClassA7 _next = new ClassA7();
    private int nofCalls = 0;

    public void F() {
      nofCalls++;
      G();
    }

    private void G() {
      _next.F();
    }
  }
}
