namespace BankTest
{
    public class BankAccount
    {
        private int balance;
        private readonly object sync = new();

        public void Deposit(int amount)
        {
            lock (sync)
            {
                balance += amount;
            }
        }

        public void Transfer(BankAccount other, int amount)
        {
            lock (sync)
            {
                balance -= amount;
                other.Deposit(amount);
            }
        }

        public int Balance
        {
            get
            {
                lock (sync)
                {
                    return balance;
                }
            }
        }
    }
}
