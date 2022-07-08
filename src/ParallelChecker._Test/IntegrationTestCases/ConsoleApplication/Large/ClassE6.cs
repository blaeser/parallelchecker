namespace Large {
  internal class ClassE6 {
    private readonly ClassE7 _next = new ClassE7();
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
