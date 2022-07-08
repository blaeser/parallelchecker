namespace Large {
  internal class ClassZ5 {
    private readonly ClassZ6 _next = new ClassZ6();
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
