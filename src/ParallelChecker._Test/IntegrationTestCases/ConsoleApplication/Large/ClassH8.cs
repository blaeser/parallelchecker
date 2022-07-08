namespace Large {
  internal class ClassH8 {
    private readonly ClassH9 _next = new ClassH9();
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
