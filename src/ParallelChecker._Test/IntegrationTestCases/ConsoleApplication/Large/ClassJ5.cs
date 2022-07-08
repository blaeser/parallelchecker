namespace Large {
  internal class ClassJ5 {
    private readonly ClassJ6 _next = new ClassJ6();
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
