using System.Threading;

namespace Large
{
    class Program
    {
        public static void Main()
        {
            new Thread(() => new ClassA0().F()).Start();
            new Thread(() => new ClassA0().F()).Start();
        }
    }
}
