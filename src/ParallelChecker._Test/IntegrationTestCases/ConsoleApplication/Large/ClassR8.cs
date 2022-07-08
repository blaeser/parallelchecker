namespace Large {
  internal class ClassR8 {
    private readonly ClassR9 _next = new ClassR9();
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
