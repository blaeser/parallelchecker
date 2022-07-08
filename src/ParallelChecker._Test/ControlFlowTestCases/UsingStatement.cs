using System;

class Test {
  class Resource : IDisposable {
    public int Value { get; set; }

    public void Dispose() {
    }
  }

  public static void Main() {
    using (var x = new Resource()) {
      x.Value++;
    }
    using var y = new Resource();
    y.Value++;
  }
}
