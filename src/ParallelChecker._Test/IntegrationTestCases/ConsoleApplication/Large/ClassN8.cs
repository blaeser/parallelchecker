namespace Large {
  internal class ClassN8 {
    private readonly ClassN9 _next = new ClassN9();
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
