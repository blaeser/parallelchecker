namespace Large {
  internal class ClassE3 {
    private readonly ClassE4 _next = new ClassE4();
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
