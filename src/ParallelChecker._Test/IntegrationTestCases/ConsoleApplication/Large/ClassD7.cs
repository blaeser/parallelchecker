namespace Large {
  internal class ClassD7 {
    private readonly ClassD8 _next = new ClassD8();
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
