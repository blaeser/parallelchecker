namespace Large {
  internal class ClassH6 {
    private readonly ClassH7 _next = new ClassH7();
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
