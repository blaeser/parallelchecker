namespace Large {
  internal class ClassS5 {
    private readonly ClassS6 _next = new ClassS6();
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
