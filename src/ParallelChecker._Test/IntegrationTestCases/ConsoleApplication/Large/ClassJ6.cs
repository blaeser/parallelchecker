namespace Large {
  internal class ClassJ6 {
    private readonly ClassJ7 _next = new ClassJ7();
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
