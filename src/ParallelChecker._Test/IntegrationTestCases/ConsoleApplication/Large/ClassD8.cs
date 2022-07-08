namespace Large {
  internal class ClassD8 {
    private readonly ClassD9 _next = new ClassD9();
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
