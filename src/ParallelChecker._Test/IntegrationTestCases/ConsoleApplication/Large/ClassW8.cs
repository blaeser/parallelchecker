namespace Large {
  internal class ClassW8 {
    private readonly ClassW9 _next = new ClassW9();
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
