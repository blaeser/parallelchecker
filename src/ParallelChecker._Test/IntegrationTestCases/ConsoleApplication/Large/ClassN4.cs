namespace Large {
  internal class ClassN4 {
    private readonly ClassN5 _next = new ClassN5();
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
