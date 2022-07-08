using System;
using System.Threading.Tasks;

class NestedArray {
  public static void Main() {
    int[,] a = { { 1, 2 }, { 3, 4 } };
    int[][] b = { new int[] { 1, 2 }, new int[] { 3 } };
    if (b[0][1] == 2 && b[1][0] == 3) {
      Task.Run(() => a[0, 1] = 1);
      Console.WriteLine(a[0, 1]);
    }
  }
}
