using System;
using System.Threading.Tasks;

public class Program : IDisposable {
  public static void Main() {
    using var linkedTokenSource = new Program();

    Task.Run(() => linkedTokenSource);
  }

  public void Dispose() {
  }
}
