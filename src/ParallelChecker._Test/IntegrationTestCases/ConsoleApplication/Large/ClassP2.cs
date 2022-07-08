namespace Large {
  internal class ClassP2 {
    private readonly ClassP3 _next = new ClassP3();
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
