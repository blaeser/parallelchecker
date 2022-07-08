namespace Large {
  internal class ClassT5 {
    private readonly ClassT6 _next = new ClassT6();
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
