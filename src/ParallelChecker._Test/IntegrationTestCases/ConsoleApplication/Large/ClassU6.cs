namespace Large {
  internal class ClassU6 {
    private readonly ClassU7 _next = new ClassU7();
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
