namespace Large {
  internal class ClassZ0 {
    private readonly ClassZ1 _next = new ClassZ1();
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
