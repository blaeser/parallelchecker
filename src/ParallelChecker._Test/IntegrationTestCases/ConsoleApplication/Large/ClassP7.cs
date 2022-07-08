namespace Large {
  internal class ClassP7 {
    private readonly ClassP8 _next = new ClassP8();
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
