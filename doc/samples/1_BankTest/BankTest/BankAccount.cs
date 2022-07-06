namespace BankTest
{
    public class BankAccount
    {
        private readonly object sync = new();

        public void Deposit(int amount)
        {
            lock (sync)
            {
                Balance += amount;
            }
        }

        public bool Withdraw(int amount)
        {
            lock (sync)
            {
                if (Balance >= amount)
                {
                    Balance -= amount;
                    return true;
                }
                return false;
            }
        }

        public int Balance { get; private set; }
    }
}
