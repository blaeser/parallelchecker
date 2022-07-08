namespace Large {
  internal class ClassJ2 {
    private readonly ClassJ3 _next = new ClassJ3();
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
