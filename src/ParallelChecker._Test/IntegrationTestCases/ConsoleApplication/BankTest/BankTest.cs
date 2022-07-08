using System;
using System.Threading;

namespace BankTest {
  public class BankTest {
    public static void Main() {
      var account = new BankAccount();
      account.Deposit(100);
      var t1 = new Thread(() => {
        account.Deposit(100);
      });
      var t2 = new Thread(() => {
        var result = account.Withdraw(50);
        Console.WriteLine(result);
      });
      t1.Start();
      t2.Start();
      Console.WriteLine(account.Balance);
      t1.Join();
      t2.Join();
    }
  }
}
