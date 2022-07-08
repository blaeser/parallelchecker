namespace Large {
  internal class ClassR4 {
    private readonly ClassR5 _next = new ClassR5();
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
