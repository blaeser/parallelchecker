using System.Threading.Tasks;

namespace QuickSort
{
    class QuickSort
    {
        public static void Sort(int[] array)
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
                    var temp = array[lower];
                    array[lower] = array[upper];
                    array[upper] = temp;
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
    }
}
