namespace Large {
  internal class ClassS6 {
    private readonly ClassS7 _next = new ClassS7();
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
