namespace Large {
  internal class ClassU2 {
    private readonly ClassU3 _next = new ClassU3();
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
