namespace Large {
  internal class ClassT2 {
    private readonly ClassT3 _next = new ClassT3();
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
