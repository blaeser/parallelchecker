namespace Large {
  internal class ClassJ7 {
    private readonly ClassJ8 _next = new ClassJ8();
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
