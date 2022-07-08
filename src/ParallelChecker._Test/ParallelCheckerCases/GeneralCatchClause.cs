using System;
using System.Threading.Tasks;

class GeneralCatchClause {
  public static void Main() {
    try {
      throw new Exception();
    } catch (NullReferenceException) {

    } catch {
      Console.Write("Test");
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}
