using System.Collections.Generic;

class Test {
  class Registry {
    public List<string> Names { get; set; }
    public ISet<int> Ages { get; set; }
    public IDictionary<string, double> Entries { get; set; }
  }

  public static void Main() {
    var x = new Registry {
      Names = { "A", "B", "C" },
      Ages = { 1, 2, 3 },
      Entries = { { "X", 1.1 }, { "Y", 2.2 } }
    };
    System.Threading.Tasks.Task.Run(() => { });
  }
}
