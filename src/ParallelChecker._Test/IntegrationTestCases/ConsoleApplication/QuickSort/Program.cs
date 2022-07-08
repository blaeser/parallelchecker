using System;

namespace QuickSort
{
    class Program
    {
        static void Main(string[] args)
        {
            var array = new int[] { 1, 5, 3, -2, 6, 0, 19, 7, 1 }; 
            QuickSort.Sort(array);
            Console.WriteLine(string.Join(",", array));
        }
    }
}
