namespace Large {
  internal class ClassE1 {
    private readonly ClassE2 _next = new ClassE2();
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
