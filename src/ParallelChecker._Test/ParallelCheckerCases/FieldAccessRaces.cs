using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class BankAccount {
    public int balance;
    public static int last;
  }

  public class Program {
    public static void Main(string[] args) {
      var account = new BankAccount();
      var t1 = new Thread(() => {
        account.balance += 100;
        BankAccount.last = account.balance;
      });
      var t2 = new Thread(() => {
        account.balance -= 100;
        BankAccount.last = account.balance;
      });
      t1.Start();
      t2.Start();
      t1.Join();
      t2.Join();
      Console.WriteLine(account.balance);
    }
  }
}
