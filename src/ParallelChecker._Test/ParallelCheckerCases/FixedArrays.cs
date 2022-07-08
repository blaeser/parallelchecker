using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  unsafe struct Data {
    public fixed int Items[5];
    public int[] RefItems;
  }

  class Program {
    unsafe static void Main() {
      Test(new Data() { RefItems = new int[5] });
    }

    static unsafe void Test(Data data) {
      int* x = data.Items;
      Task.Run(() => x[0]++);
      Console.WriteLine(x[0]);
      int[] y = data.RefItems;
      Task.Run(() => y[0]++);
      Console.WriteLine(y[0]);
    }
  }
}
