﻿//https://msdn.microsoft.com/en-us/library/c5kehkcz(v=vs.71).aspx
// statements_lock2.cs
using System;
using System.Threading;

class Account {
  int balance;

  Random r = new Random();

  public Account(int initial) {
    balance = initial;
  }

  int Withdraw(int amount) {

    // This condition will never be true unless the lock statement
    // is commented out:
    if (balance < 0) {
      throw new Exception("Negative Balance");
    }

    // Comment out the next line to see the effect of leaving out 
    // the lock keyword:
    lock (this) {
      if (balance >= amount) {
        Console.WriteLine("Balance before Withdrawal :  " + balance);
        Console.WriteLine("Amount to Withdraw        : -" + amount);
        balance = balance - amount;
        Console.WriteLine("Balance after Withdrawal  :  " + balance);
        return amount;
      } else {
        return 0; // transaction rejected
      }
    }
  }

  public void DoTransactions() {
    for (int i = 0; i < 100; i++) {
      Withdraw(r.Next(1, 100));
    }
  }
}

class Test {
  public static void Main() {
    Thread[] threads = new Thread[10];
    Account acc = new Account(1000);
    for (int i = 0; i < 10; i++) {
      Thread t = new Thread(new ThreadStart(acc.DoTransactions));
      threads[i] = t;
    }
    for (int i = 0; i < 10; i++) {
      threads[i].Start();
    }
  }
}
