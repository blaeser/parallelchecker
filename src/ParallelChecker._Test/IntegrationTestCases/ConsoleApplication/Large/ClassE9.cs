namespace Large {
  internal class ClassE9 {
    private readonly ClassF0 _next = new ClassF0();
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
