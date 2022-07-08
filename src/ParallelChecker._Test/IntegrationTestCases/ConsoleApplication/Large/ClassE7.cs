namespace Large {
  internal class ClassE7 {
    private readonly ClassE8 _next = new ClassE8();
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
