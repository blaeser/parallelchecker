namespace Large {
  internal class ClassN3 {
    private readonly ClassN4 _next = new ClassN4();
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
