namespace Large {
  internal class ClassE4 {
    private readonly ClassE5 _next = new ClassE5();
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
