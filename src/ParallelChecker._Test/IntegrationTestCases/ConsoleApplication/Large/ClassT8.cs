namespace Large {
  internal class ClassT8 {
    private readonly ClassT9 _next = new ClassT9();
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
