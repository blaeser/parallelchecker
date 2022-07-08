namespace Large {
  internal class ClassW7 {
    private readonly ClassW8 _next = new ClassW8();
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
