using System;
using System.Threading;

namespace DoubleCheckedLocking
{
    class Singleton
    {
        private static Singleton instance;
        private static readonly object locker = new();
        public int[] Data { get; private set; }

        private Singleton()
        {
            Data = new int[100];
        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new Singleton();
                        }
                    }
                }
                return instance;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            new Thread(() => Console.WriteLine(Singleton.Instance)).Start();
            new Thread(() => Console.WriteLine(Singleton.Instance)).Start();
        }
    }
}
