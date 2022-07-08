using System;
using System.Threading.Tasks;

class ContinuationTest {
  public static void Main() {
    var previous = Task.Run(() => { });
    previous.Wait();
    int race = 0;
    previous.ContinueWith(p => Console.Write(race));
    race = 1;
  }
}
