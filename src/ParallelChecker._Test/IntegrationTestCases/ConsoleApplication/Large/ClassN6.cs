namespace Large {
  internal class ClassN6 {
    private readonly ClassN7 _next = new ClassN7();
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
