namespace BankTest
{
    public class BankAccount
    {
        private int _balance;
        private object _sync = new object();

        public void Deposit(int amount)
        {
            lock (_sync)
            {
                _balance += amount;
            }
        }

        public void Transfer(BankAccount other, int amount)
        {
            lock (_sync)
            {
                _balance -= amount;
                other.Deposit(amount);
            }
        }

        public int Balance
        {
            get
            {
                lock (_sync)
                {
                    return _balance;
                }
            }
        }
    }
}
