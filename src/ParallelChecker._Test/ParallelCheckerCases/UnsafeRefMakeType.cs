namespace ParallelChecker._Test {
  public class Program {
    public unsafe static void Main() {
      Program x = null;
      var y = __makeref(x);
      var z = __refvalue(y, Program);
      var a = __reftype(y);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}
