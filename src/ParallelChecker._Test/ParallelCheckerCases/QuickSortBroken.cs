using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class QuickSort {
    static void Main(string[] args) {
      var array = new int[] { 1, 5, 3, -2, 6, 0, 19, 7, 1 };
      QuickSort.Sort(array);
      Console.WriteLine(string.Join(",", array));
    }

    public static void Sort(int[] array) {
      _Sort(array, 0, array.Length - 1);
    }

    private static void _Sort(int[] array, int left, int right) {
      var pivot = array[(left + right) / 2];
      var lower = left;
      var upper = right;
      do {
        while (array[lower] < pivot) lower++;
        while (array[upper] > pivot) upper--;
        if (lower <= upper) {
          var temp = array[lower];
          array[lower] = array[upper];
          array[upper] = temp;
          lower++;
          upper--;
        }
      } while (lower <= upper);
      var leftTask = Task.Run(() =>
      {
        if (left < upper) _Sort(array, left, lower);
      });
      var rightTask = Task.Run(() =>
      {
        if (lower < right) _Sort(array, upper, right);
      });
      leftTask.Wait();
      rightTask.Wait();
    }
  }
}
