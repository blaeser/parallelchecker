namespace Large {
  internal class ClassX8 {
    private readonly ClassX9 _next = new ClassX9();
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
