namespace Large {
  internal class ClassI9 {
    private readonly ClassJ0 _next = new ClassJ0();
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
