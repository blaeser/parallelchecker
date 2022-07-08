namespace Large {
  internal class ClassZ7 {
    private readonly ClassZ8 _next = new ClassZ8();
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
