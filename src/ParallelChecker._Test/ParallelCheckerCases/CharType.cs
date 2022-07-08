using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class CharType {
    static void Main() {
      char c = 'A';
      c++;
      c--;
      var b = c >= 20;
      int i = c;
      c = (char)i;
      if (c == 'A') {
        Task.Run(() => c = '\0');
      }
      Console.Write(c);
    }
  }
}
