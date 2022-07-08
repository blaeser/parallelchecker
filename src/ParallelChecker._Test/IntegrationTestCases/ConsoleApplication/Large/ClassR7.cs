namespace Large {
  internal class ClassR7 {
    private readonly ClassR8 _next = new ClassR8();
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
