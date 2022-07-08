using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class BankAccount {
    private int _balance = 0;

    public void Deposit(int amount) {
      _balance += amount;
    }

    public bool Withdraw(int amount) {
      if (_balance >= amount) {
        _balance -= amount;
        return true;
      }
      return false;
    }

    public int GetBalance() {
      return _balance;
    }
  }

  public class Program {
    public static void Main(/*string[] args*/) {
      var account = new BankAccount();
      var t1 = new Thread(() => {
        account.Deposit(100);
        var result = account.Withdraw(100);
        Console.WriteLine(result);
      });
      var t2 = new Thread(() => {
        account.Deposit(50);
        var result = account.Withdraw(50);
        Console.WriteLine(result);
      });
      t1.Start();
      t2.Start();
      t1.Join();
      t2.Join();
      Console.WriteLine(account.GetBalance());
    }
  }
}
