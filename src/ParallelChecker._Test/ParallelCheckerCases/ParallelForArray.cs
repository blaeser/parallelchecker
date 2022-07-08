using System;
using System.Threading.Tasks;

namespace ParallelWithoutLocks {
  class Program {
    static int CountPrimes(int[] array) {
      int count = 0;
      Parallel.For(0, array.Length, index => {
        if (IsPrime(array[index])) {
          count++;
        }
      });
      return count;
    }

    static bool IsPrime(int number) {
      if (number < 2) {
        return false;
      }
      for (int divisor = 2; divisor * divisor <= number; divisor++) {
        if (number % divisor == 0) {
          return false;
        }
      }
      return true;
    }

    static void Main() {
      var array = Initialize(100);
      int last = CountPrimes(array);
      while (true) {
        Console.WriteLine(last);
        var actual = CountPrimes(array);
        if (actual != last) {
          Console.WriteLine(actual);
          Console.WriteLine("Race condition!");
          return;
        }
      }
    }

    static int[] Initialize(int amount) {
      int[] result = new int[amount];
      for (int i = 0; i < amount; i++) {
        result[i] = 3 + 2 * i;
      }
      return result;
    }
  }
}
