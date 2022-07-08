namespace Large {
  internal class ClassZ6 {
    private readonly ClassZ7 _next = new ClassZ7();
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
