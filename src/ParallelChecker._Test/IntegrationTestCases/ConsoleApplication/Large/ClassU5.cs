namespace Large {
  internal class ClassU5 {
    private readonly ClassU6 _next = new ClassU6();
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
