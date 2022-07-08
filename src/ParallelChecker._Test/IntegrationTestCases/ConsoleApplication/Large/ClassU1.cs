namespace Large {
  internal class ClassU1 {
    private readonly ClassU2 _next = new ClassU2();
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
