namespace Large {
  internal class ClassE0 {
    private readonly ClassE1 _next = new ClassE1();
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
