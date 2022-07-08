namespace Large {
  internal class ClassJ1 {
    private readonly ClassJ2 _next = new ClassJ2();
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
