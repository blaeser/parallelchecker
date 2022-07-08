namespace Large {
  internal class ClassR2 {
    private readonly ClassR3 _next = new ClassR3();
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
