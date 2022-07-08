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
            var t1 = new Thread(() =>
            {
                account1.Transfer(account2, 100);
            });
            var t2 = new Thread(() =>
            {
                account2.Transfer(account3, 100);
            });
            var t3 = new Thread(() =>
            {
                account3.Transfer(account1, 100);
            });
            t1.Start();
            t2.Start();
            t3.Start();
        }
    }
}
