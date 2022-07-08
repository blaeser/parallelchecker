namespace Large {
  internal class ClassH7 {
    private readonly ClassH8 _next = new ClassH8();
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
