namespace Large {
  internal class ClassJ0 {
    private readonly ClassJ1 _next = new ClassJ1();
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
