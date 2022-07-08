namespace Large {
  internal class ClassX7 {
    private readonly ClassX8 _next = new ClassX8();
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
