namespace Large {
  internal class ClassS7 {
    private readonly ClassS8 _next = new ClassS8();
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
