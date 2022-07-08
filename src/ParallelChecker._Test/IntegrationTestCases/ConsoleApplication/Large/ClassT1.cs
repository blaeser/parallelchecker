namespace Large {
  internal class ClassT1 {
    private readonly ClassT2 _next = new ClassT2();
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
