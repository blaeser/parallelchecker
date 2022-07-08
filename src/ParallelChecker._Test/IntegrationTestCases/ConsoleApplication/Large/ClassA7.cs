namespace Large {
  internal class ClassA7 {
    private readonly ClassA8 _next = new ClassA8();
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
