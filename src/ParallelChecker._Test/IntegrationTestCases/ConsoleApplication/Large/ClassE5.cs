namespace Large {
  internal class ClassE5 {
    private readonly ClassE6 _next = new ClassE6();
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
