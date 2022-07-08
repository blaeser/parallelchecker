namespace Large {
  internal class ClassP6 {
    private readonly ClassP7 _next = new ClassP7();
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
