using System;
using System.Threading.Tasks;

namespace QuickSort
{
    public class QuickSort
    {
        static void Sort(int[] array)
        {
            Sort(array, 0, array.Length - 1);
        }

        private static void Sort(int[] array, int left, int right)
        {
            var pivot = array[(left + right) / 2];
            var lower = left;
            var upper = right;
            do
            {
                while (array[lower] < pivot) lower++;
                while (array[upper] > pivot) upper--;
                if (lower <= upper)
                {
                    (array[upper], array[lower]) = (array[lower], array[upper]);
                    lower++;
                    upper--;
                }
            } while (lower <= upper);
            var leftTask = Task.Run(() =>
            {
                if (left < upper) Sort(array, left, lower);
            });
            var rightTask = Task.Run(() =>
            {
                if (lower < right) Sort(array, upper, right);
            });
            rightTask.Wait();
            leftTask.Wait();
        }

        public static void Main() {
            var array = new int[] { 1, 5, 3, -2, 6, 0, 19, 7, 1 };
            Sort(array);
            Console.WriteLine(string.Join(",", array));
        }
    }
}
