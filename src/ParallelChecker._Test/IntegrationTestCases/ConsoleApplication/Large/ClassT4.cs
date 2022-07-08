namespace Large {
  internal class ClassT4 {
    private readonly ClassT5 _next = new ClassT5();
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
