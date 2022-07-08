namespace Large {
  internal class ClassT7 {
    private readonly ClassT8 _next = new ClassT8();
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
