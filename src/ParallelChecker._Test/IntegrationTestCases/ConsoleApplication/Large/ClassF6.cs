namespace Large {
  internal class ClassF6 {
    private readonly ClassF7 _next = new ClassF7();
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
