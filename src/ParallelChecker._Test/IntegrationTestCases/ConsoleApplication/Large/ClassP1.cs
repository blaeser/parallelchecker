namespace Large {
  internal class ClassP1 {
    private readonly ClassP2 _next = new ClassP2();
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
