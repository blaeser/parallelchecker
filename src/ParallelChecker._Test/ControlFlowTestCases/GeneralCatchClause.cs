using System;

class GeneralCatchClause {
  public static void Main() {
    try {
      throw new Exception();
    } catch (NullReferenceException) {

    } catch {
      Console.Write("Test");
      throw new NotImplementedException();
    }
  }
}
