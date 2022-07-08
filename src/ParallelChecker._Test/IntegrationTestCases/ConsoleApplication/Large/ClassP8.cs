namespace Large {
  internal class ClassP8 {
    private readonly ClassP9 _next = new ClassP9();
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
