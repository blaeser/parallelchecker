using System;

namespace Large {
  internal class ClassZ9 {
    private static int nofCalls = 0;
        
    public void F() {
      nofCalls++;
      G();
    }

    private void G() {
      Console.WriteLine(nofCalls);
    }
  }
}
