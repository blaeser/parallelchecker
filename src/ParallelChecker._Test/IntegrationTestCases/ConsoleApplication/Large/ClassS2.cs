namespace Large {
  internal class ClassS2 {
    private readonly ClassS3 _next = new ClassS3();
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
