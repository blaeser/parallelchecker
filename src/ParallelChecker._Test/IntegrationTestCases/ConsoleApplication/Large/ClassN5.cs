namespace Large {
  internal class ClassN5 {
    private readonly ClassN6 _next = new ClassN6();
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
