namespace Large {
  internal class ClassS3 {
    private readonly ClassS4 _next = new ClassS4();
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
