namespace Large {
  internal class ClassM5 {
    private readonly ClassM6 _next = new ClassM6();
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
