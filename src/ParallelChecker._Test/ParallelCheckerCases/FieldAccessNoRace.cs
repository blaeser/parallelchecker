using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class BankAccount {
    public int balance;
    public static object myLock = new object();
  }

  public class Program {
    public static void Main(string[] args) {
      var account = new BankAccount();
      var t1 = new Thread(() => {
        lock (BankAccount.myLock) {
          account.balance += 100;
        }
      });
      var t2 = new Thread(() => {
        lock (BankAccount.myLock) {
          account.balance -= 100;
        }
      });
      t1.Start();
      t2.Start();
      t1.Join();
      t2.Join();
      Console.WriteLine(account.balance);
    }
  }
}
