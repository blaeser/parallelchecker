namespace Large {
  internal class ClassE8 {
    private readonly ClassE9 _next = new ClassE9();
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
