namespace Large {
  internal class ClassZ8 {
    private readonly ClassZ9 _next = new ClassZ9();
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
