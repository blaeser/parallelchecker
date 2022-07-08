namespace Large {
  internal class ClassD6 {
    private readonly ClassD7 _next = new ClassD7();
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
