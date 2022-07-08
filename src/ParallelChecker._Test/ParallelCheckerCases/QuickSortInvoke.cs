using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class QuickSort {
    const int Amount = 30;

    static void Main(string[] args) {
      var array = new int[Amount];
      for (int index = 0; index < array.Length; index++) {
        array[index] = -index * index;
      }
      QuickSort.Sort(array);
      Console.WriteLine(string.Join(",", array));
    }

    public static void Sort(int[] array) {
      _Sort(array, 0, array.Length - 1);
    }

    const int Threshold = 10;

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
      if (upper - left > Threshold && right - lower > Threshold) {
        Parallel.Invoke(
            () => _Sort(array, left, lower),
            () => _Sort(array, upper, right)
        );
      } else {
        if (left < upper) _Sort(array, left, lower);
        if (lower < right) _Sort(array, upper, right);
      }
    }
  }
}
