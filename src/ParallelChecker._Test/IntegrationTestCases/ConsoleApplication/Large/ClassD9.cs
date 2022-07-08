namespace Large {
  internal class ClassD9 {
    private readonly ClassE0 _next = new ClassE0();
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
