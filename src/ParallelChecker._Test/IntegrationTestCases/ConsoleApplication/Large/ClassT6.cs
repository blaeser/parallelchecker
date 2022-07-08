namespace Large {
  internal class ClassT6 {
    private readonly ClassT7 _next = new ClassT7();
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
