using System;
using System.Threading;

class Program {
    static void Main() {
        var x = 0;
        new Thread(() => {
            x = 1;
        }).Start();
        Console.WriteLine(x);
    }
}
