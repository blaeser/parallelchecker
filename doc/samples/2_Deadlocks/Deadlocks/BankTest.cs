using System.Threading;

namespace BankTest
{
    public class BankTest
    {
        public static void Main()
        {
            var account1 = new BankAccount();
            var account2 = new BankAccount();
            var account3 = new BankAccount();
            new Thread(() => account1.Transfer(account2, 100)).Start();
            new Thread(() => account2.Transfer(account3, 100)).Start();
            new Thread(() => account3.Transfer(account1, 100)).Start();
        }
    }
}
