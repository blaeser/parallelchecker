namespace Large {
  internal class ClassT3 {
    private readonly ClassT4 _next = new ClassT4();
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
