namespace Large {
  internal class ClassR6 {
    private readonly ClassR7 _next = new ClassR7();
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
