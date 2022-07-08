using System;

class Test {
  class SubRandom : Random {
    public override int Next() {
      return base.Next();
    }
  }


  public static void Main() {
    new SubRandom().Next();
    System.Threading.Tasks.Task.Run(() => { });
  }
}
