namespace Large {
  internal class ClassM8 {
    private readonly ClassM9 _next = new ClassM9();
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
