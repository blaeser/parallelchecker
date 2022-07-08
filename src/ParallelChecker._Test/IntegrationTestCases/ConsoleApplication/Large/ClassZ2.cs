namespace Large {
  internal class ClassZ2 {
    private readonly ClassZ3 _next = new ClassZ3();
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
