using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      var array = new Task[2];
      array[0] = new Task(() => array[1].Wait());
      array[1] = new Task(() => array[0].Wait());
      array[0].Start();
      array[1].Start();
    }
  }
}
