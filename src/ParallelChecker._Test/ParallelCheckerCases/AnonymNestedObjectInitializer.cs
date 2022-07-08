using System.Threading.Tasks;

namespace CheckerDevTest {
  class Outer {
    public Inner Sub { get; set; }
  }

  class Inner {
    public int X { get; set; }
  }

  class Program {
    static void Main(string[] args) {
      Task.Run(() => {
        var o = new Outer {
          Sub = {
            X = 1
          }
        };
      });
    }
  }
}
