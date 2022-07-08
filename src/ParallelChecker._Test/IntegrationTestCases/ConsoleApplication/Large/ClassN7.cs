namespace Large {
  internal class ClassN7 {
    private readonly ClassN8 _next = new ClassN8();
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
