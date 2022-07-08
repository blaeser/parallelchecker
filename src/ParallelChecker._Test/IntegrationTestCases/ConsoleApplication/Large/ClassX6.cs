namespace Large {
  internal class ClassX6 {
    private readonly ClassX7 _next = new ClassX7();
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
