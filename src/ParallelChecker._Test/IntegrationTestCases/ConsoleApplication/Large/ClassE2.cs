namespace Large {
  internal class ClassE2 {
    private readonly ClassE3 _next = new ClassE3();
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
