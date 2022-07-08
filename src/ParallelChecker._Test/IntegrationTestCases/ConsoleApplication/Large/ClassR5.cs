namespace Large {
  internal class ClassR5 {
    private readonly ClassR6 _next = new ClassR6();
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
