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

        public bool Withdraw(int amount)
        {
            lock (_sync)
            {
                if (_balance >= amount)
                {
                    _balance -= amount;
                    return true;
                }
                return false;
            }
        }

        public int Balance
        {
            get
            {
                return _balance;
            }
        }
    }
}
