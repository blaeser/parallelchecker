namespace Large {
  internal class ClassD5 {
    private readonly ClassD6 _next = new ClassD6();
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
