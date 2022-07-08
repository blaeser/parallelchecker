namespace Large {
  internal class ClassA8 {
    private readonly ClassA9 _next = new ClassA9();
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
