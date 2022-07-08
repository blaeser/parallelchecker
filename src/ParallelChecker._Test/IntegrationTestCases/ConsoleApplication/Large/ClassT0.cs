namespace Large {
  internal class ClassT0 {
    private readonly ClassT1 _next = new ClassT1();
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
