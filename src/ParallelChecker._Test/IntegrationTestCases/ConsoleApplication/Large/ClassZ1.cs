namespace Large {
  internal class ClassZ1 {
    private readonly ClassZ2 _next = new ClassZ2();
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
