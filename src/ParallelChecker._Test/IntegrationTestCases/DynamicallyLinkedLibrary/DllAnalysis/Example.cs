using System.Threading;

namespace DllAnalysis {
  public class Example {
    private int last;
    private static bool flag;

    public void Execute(int value) {
      new Thread(() => Flip()).Start();
      Flip();
      new Thread(() => GetLast()).Start();
      last = value;
    }

    public int GetLast() {
      return last;
    }

    public void Flip() {
      flag = !flag;
    }
  }
}
