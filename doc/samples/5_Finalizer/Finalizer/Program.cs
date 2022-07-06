using System;

namespace Test
{
    public class Block
    {
        private static int count;

        Block()
        {
            count++;
        }

        ~Block()
        {
            count--;
            Console.WriteLine($"Deconstructed {count}");
        }

        public static void Main()
        {
            _ = new Block();
            _ = new Block();
        }
    }
}
