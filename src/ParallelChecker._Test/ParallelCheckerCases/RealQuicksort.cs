using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class QuickSort {
    private const int Threshold = 1;

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
      if (upper - left > Threshold && right - lower > Threshold) {
        Parallel.Invoke(
            () => _Sort(array, left, lower),
            () => _Sort(array, upper, right)
        );
      } else {
        if (left < upper) _Sort(array, left, upper);
        if (lower < right) _Sort(array, lower, right);
      }
    }

    public static void Main() {
      Sort(new int[] { 1, 5, 3, -2, 6, 0, 19, 7, 1 });
    }
  }
}
