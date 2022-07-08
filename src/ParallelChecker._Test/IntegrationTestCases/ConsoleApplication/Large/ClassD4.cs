namespace Large {
  internal class ClassD4 {
    private readonly ClassD5 _next = new ClassD5();
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
