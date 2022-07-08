namespace Large {
  internal class ClassR3 {
    private readonly ClassR4 _next = new ClassR4();
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
