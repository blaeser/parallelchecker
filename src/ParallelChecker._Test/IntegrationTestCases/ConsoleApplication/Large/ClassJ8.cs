namespace Large {
  internal class ClassJ8 {
    private readonly ClassJ9 _next = new ClassJ9();
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
