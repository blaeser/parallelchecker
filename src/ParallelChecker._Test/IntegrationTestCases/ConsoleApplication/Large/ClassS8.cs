namespace Large {
  internal class ClassS8 {
    private readonly ClassS9 _next = new ClassS9();
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
