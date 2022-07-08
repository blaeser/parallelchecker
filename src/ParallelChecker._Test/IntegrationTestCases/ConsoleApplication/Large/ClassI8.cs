namespace Large {
  internal class ClassI8 {
    private readonly ClassI9 _next = new ClassI9();
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
