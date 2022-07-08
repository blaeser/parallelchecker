namespace Large {
  internal class ClassJ3 {
    private readonly ClassJ4 _next = new ClassJ4();
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
